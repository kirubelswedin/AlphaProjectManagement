document.addEventListener("DOMContentLoaded", () => {
	// Initialisera alla fönsterhanterare
	initDropdownHandling();
	initModalHandling();
});

/*
 * ------------------------------------------------------------------------------------------------
 * Modal-hantering
 * ------------------------------------------------------------------------------------------------
 */
const WindowManager = {
	// Modal-hantering
	openModal(modalId, data = null) {
		// Stäng alla dropdowns först
		closeAllDropdowns();

		const modal = document.getElementById(modalId);
		if (!modal) return;

		if (data) {
			const event = new CustomEvent("modalData", { detail: data });
			modal.dispatchEvent(event);
		}

		modal.classList.add("show");
		document.body.style.overflow = "hidden";
	},

	closeModal(modalId) {
		const modal = document.getElementById(modalId);
		if (!modal) return;

		modal.classList.remove("show");
		document.body.style.overflow = "";
	},

	handleModalTrigger(trigger) {
		const modalId =
			trigger.dataset.target?.replace("#", "") || trigger.dataset.modal;
		if (!modalId) return;

		const data = trigger.dataset.data ? JSON.parse(trigger.dataset.data) : null;
		this.openModal(modalId, data);
	},

	// Generell fönsterhantering
	closeWindow(targetId) {
		// Kontrollera om det är en dropdown
		const dropdownWrapper = document
			.querySelector(`[data-target="#${targetId}"]`)
			?.closest(".dropdown-wrapper");
		if (dropdownWrapper) {
			closeDropdown(targetId);
			return;
		}

		// Kontrollera om det är en modal
		const modal = document.getElementById(targetId);
		if (modal?.classList.contains("modal")) {
			this.closeModal(targetId);
		}
	},
};

// Gör WindowManager tillgänglig globalt
window.WindowManager = WindowManager;

/*
 * ------------------------------------------------------------------------------------------------
 * Dropdown-hantering
 * ------------------------------------------------------------------------------------------------
 */

// Huvudfunktioner för dropdowns
function toggleDropdown(targetId, trigger) {
	// Stäng alla modaler först
	const activeModals = document.querySelectorAll(".modal.show");
	activeModals.forEach((modal) => WindowManager.closeModal(modal.id));

	const dropdownWrapper = document.getElementById(targetId);
	if (!dropdownWrapper) return;

	const isClientListDropdown = targetId.startsWith("client-dropdown-");

	// Om denna dropdown redan är aktiv, stäng den bara
	if (dropdownWrapper.classList.contains("active")) {
		closeDropdown(targetId);
		return;
	}

	// Stäng alla andra dropdowns först
	closeAllDropdowns();

	// Öppna den klickade dropdowm
	dropdownWrapper.classList.add("active");

	// Hantera specifika dropdown typer
	if (isClientListDropdown) {
		handleClientListDropdown(dropdownWrapper, trigger);
	}
}

function handleClientListDropdown(dropdownWrapper, trigger) {
	// Stäng alla andra dropdowns först
	closeAllDropdowns();

	// Offset för finjustering av position
	const offsetX = 0; // Negativ flyttar åt vänster, positiv åt höger
	const offsetY = 2; // Negativ flyttar uppåt, positiv nedåt

	const tableWrapper = trigger.closest(".table-wrapper");
	const rect = trigger.getBoundingClientRect();
	const tableRect = tableWrapper.getBoundingClientRect();

	// Sätt initial position
	dropdownWrapper.style.position = "absolute";
	dropdownWrapper.style.top = "0";
	dropdownWrapper.style.left = "0";
	dropdownWrapper.style.visibility = "hidden";
	dropdownWrapper.style.display = "block";

	// Vänta på att DOM ska uppdateras
	requestAnimationFrame(() => {
		const dropdownWidth = dropdownWrapper.offsetWidth;

		// Beräkna position
		const top = rect.top - tableRect.top + rect.height + offsetY;
		const left =
			rect.left - tableRect.left - dropdownWidth + rect.width + offsetX;

		// Applicera position
		dropdownWrapper.style.top = `${top}px`;
		dropdownWrapper.style.left = `${left}px`;
		dropdownWrapper.style.visibility = "visible";
		dropdownWrapper.classList.add("active");

		// Uppdatera position vid scroll
		const updatePosition = () => {
			const newRect = trigger.getBoundingClientRect();
			const newTableRect = tableWrapper.getBoundingClientRect();
			dropdownWrapper.style.top = `${
				newRect.top - newTableRect.top + newRect.height + offsetY
			}px`;
			dropdownWrapper.style.left = `${
				newRect.left - newTableRect.left - dropdownWidth + newRect.width + offsetX
			}px`;
		};

		// Lägg till event listeners
		window.addEventListener("scroll", updatePosition);
		tableWrapper.addEventListener("scroll", updatePosition);

		// Stoppa event propagation för att förhindra att dropdown stängs direkt
		dropdownWrapper.addEventListener("click", (e) => {
			e.stopPropagation();
		});

		// Stäng dropdown när man klickar utanför
		const closeOnClickOutside = (e) => {
			if (!dropdownWrapper.contains(e.target) && !trigger.contains(e.target)) {
				closeDropdown(dropdownWrapper.id);
				document.removeEventListener("click", closeOnClickOutside);
			}
		};

		// Vänta en kort stund innan vi lägger till click-event för att undvika att dropdown stängs direkt
		setTimeout(() => {
			document.addEventListener("click", closeOnClickOutside);
		}, 100);

		// Cleanup funktion
		const cleanup = () => {
			window.removeEventListener("scroll", updatePosition);
			tableWrapper.removeEventListener("scroll", updatePosition);
			document.removeEventListener("click", closeOnClickOutside);
		};

		// Lägg till cleanup när dropdown stängs
		dropdownWrapper.addEventListener("close", cleanup);
	});
}

function closeDropdown(targetId) {
	const dropdownWrapper = document.getElementById(targetId);
	if (!dropdownWrapper) return;
	dropdownWrapper.classList.remove("active");
}

function closeAllDropdowns() {
	document.querySelectorAll(".dropdown-wrapper.active").forEach((dropdown) => {
		dropdown.classList.remove("active");
	});
}

/*
 * ------------------------------------------------------------------------------------------------
 * Initialization Functions
 * ------------------------------------------------------------------------------------------------
 */
function initDropdownHandling() {
	// Hantera dropdown triggers med capture phase för att säkerställa att vi fångar events först
	document.addEventListener(
		"click",
		(e) => {
			const dropdownTrigger = e.target.closest("[data-window='dropdown']");
			if (!dropdownTrigger) return;

			const targetId = dropdownTrigger.dataset.target?.replace("#", "");
			if (targetId) {
				e.preventDefault();
				e.stopPropagation();
				toggleDropdown(targetId, dropdownTrigger);
			}
		},
		true
	); // Använd capture phase

	// Stäng dropdowns när man klickar utanför
	document.addEventListener("click", (e) => {
		if (
			!e.target.closest(".dropdown-wrapper") &&
			!e.target.closest("[data-window='dropdown']")
		) {
			closeAllDropdowns();
		}
	});

	// Stäng dropdowns med Escape
	document.addEventListener("keydown", (e) => {
		if (e.key === "Escape") {
			closeAllDropdowns();
		}
	});
}

function initModalHandling() {
	// Hantera modal triggers
	document.addEventListener("click", (e) => {
		const modalTrigger = e.target.closest("[data-window='modal']");
		if (!modalTrigger) return;
		WindowManager.handleModalTrigger(modalTrigger);
	});

	// Stäng modal när man klickar utanför
	document.addEventListener("click", (e) => {
		if (e.target.classList.contains("modal")) {
			WindowManager.closeModal(e.target.id);
		}
	});

	// Stäng modal med Escape
	document.addEventListener("keydown", (e) => {
		if (e.key === "Escape") {
			const activeModal = document.querySelector(".modal.show");
			if (activeModal) {
				WindowManager.closeModal(activeModal.id);
			}
		}
	});
}
