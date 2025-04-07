import { SELECTORS, CLASSES } from "./constants.js";

/**
 * Centralized window management for both modals and dropdowns
 */
class WindowManager {
	constructor() {
		this.initialized = false;
		this.openWindows = new Set();
	}

	init() {
		if (this.initialized) return;

		this.initWindowTriggers();
		this.initWindowCloseTriggers();
		this.initWindowOutsideClicks();
		this.initWindowEscapeKey();

		this.initialized = true;
	}

	initWindowTriggers() {
		document.addEventListener("click", (e) => {
			const trigger = e.target.closest("[data-window]");
			if (!trigger) return;

			const type = trigger.dataset.window;
			const targetId = trigger.dataset.target?.replace("#", "");

			if (type === "modal") {
				this.openModal(targetId);
			} else if (type === "dropdown") {
				this.toggleDropdown(targetId, trigger);
			}
		});
	}

	initWindowCloseTriggers() {
		document.querySelectorAll("[data-close]").forEach((button) => {
			button.addEventListener("click", (e) => {
				e.preventDefault();
				const targetId = button.dataset.target?.replace("#", "");
				if (targetId) {
					this.closeWindow(targetId);
				}
			});
		});
	}

	initWindowOutsideClicks() {
		document.addEventListener("click", (e) => {
			// Skip if clicking a dropdown item with onclick attribute
			const dropdownItem = e.target.closest(".dropdown-item[onclick]");
			if (dropdownItem) {
				// Close the dropdown after executing the onclick
				setTimeout(() => {
					this.closeAllDropdowns();
				}, 100);
				return;
			}

			// Ignorera klick på triggers
			const clickedTrigger = e.target.closest("[data-window]");
			if (clickedTrigger) return;

			// Hantera modaler
			const clickedModal = e.target.closest(".modal");
			if (clickedModal) {
				// Om vi klickade inuti en modal men utanför dess innehåll
				const clickedModalContent = e.target.closest(".form-modal-content");
				if (!clickedModalContent) {
					this.closeWindow(clickedModal.id);
				}
				return;
			}

			// Hantera dropdowns
			const clickedDropdownContent = e.target.closest(".dropdown-content");
			if (!clickedDropdownContent) {
				// Stäng alla aktiva dropdowns
				document.querySelectorAll(".dropdown-wrapper.active").forEach((wrapper) => {
					const triggerId = wrapper
						.querySelector("[data-target]")
						?.dataset.target?.replace("#", "");
					if (triggerId) {
						this.closeWindow(triggerId);
					}
					wrapper.classList.remove("active");
				});

				// Stäng alla externa dropdown-content
				document
					.querySelectorAll(".dropdown-content-wrapper.active")
					.forEach((content) => {
						content.classList.remove("active");
						const dropdown = content.querySelector(".dropdown-content");
						if (dropdown) {
							dropdown.style.opacity = "0";
							dropdown.style.visibility = "hidden";
							dropdown.style.transform = "translateY(-10px)";
						}
					});
			}
		});
	}

	initWindowEscapeKey() {
		document.addEventListener("keydown", (e) => {
			if (e.key === "Escape") {
				this.closeAllWindows();
			}
		});
	}

	openModal(modalId) {
		// Stäng alla dropdowns först
		this.closeAllDropdowns();

		const modal = document.getElementById(modalId);
		if (!modal) {
			console.warn(`Modal with id ${modalId} not found`);
			return;
		}

		modal.classList.add("show");
		modal.style.display = "flex";
		document.body.style.overflow = "hidden";

		// Fokusera på första input-fältet om det finns
		const firstInput = modal.querySelector("input, select, textarea");
		if (firstInput) {
			firstInput.focus();
		}
	}

	toggleDropdown(targetId, trigger) {
		// Stäng alla andra dropdowns först
		const currentWrapper = trigger.closest(".dropdown-wrapper");
		document.querySelectorAll(".dropdown-wrapper.active").forEach((wrapper) => {
			if (wrapper !== currentWrapper) {
				wrapper.classList.remove("active");
			}
		});

		document
			.querySelectorAll(".dropdown-content-wrapper.active")
			.forEach((content) => {
				if (content.getAttribute("data-for") !== targetId) {
					content.classList.remove("active");
					const dropdown = content.querySelector(".dropdown-content");
					if (dropdown) {
						dropdown.style.opacity = "0";
						dropdown.style.visibility = "hidden";
						dropdown.style.transform = "translateY(-10px)";
					}
				}
			});

		const wrapper = trigger.closest(".dropdown-wrapper");
		const isActive = wrapper.classList.contains("active");

		// Om denna dropdown redan är aktiv, stäng den
		if (isActive) {
			this.closeWindow(targetId);
			return;
		}

		// Öppna denna dropdown
		wrapper.classList.add("active");
		trigger.setAttribute("aria-expanded", "true");

		// Hantera intern dropdown
		const regularDropdown = wrapper.querySelector(".dropdown-content");
		if (regularDropdown) return;

		// Hantera extern dropdown (som i ClientList)
		const dropdownContent = document.querySelector(
			`.dropdown-content-wrapper[data-for="${targetId}"]`
		);
		if (!dropdownContent) return;

		dropdownContent.classList.add("active");
		const dropdown = dropdownContent.querySelector(".dropdown-content");
		if (!dropdown) return;

		dropdown.style.opacity = "1";
		dropdown.style.visibility = "visible";
		dropdown.style.transform = "translateY(0)";

		// Special hantering för ClientList
		const clientListContainer = document.querySelector(".client-list-container");
		if (clientListContainer) {
			const rect = trigger.getBoundingClientRect();
			const containerRect = clientListContainer.getBoundingClientRect();
			const rightOffset = containerRect.right - rect.right;

			clientListContainer.appendChild(dropdownContent);
			dropdownContent.style.position = "absolute";
			dropdownContent.style.top = rect.bottom - containerRect.top + 8 + "px";
			dropdownContent.style.right = rightOffset + "px";
			dropdownContent.style.left = "auto";
			dropdownContent.style.zIndex = "1000000";
		}
	}

	closeWindow(windowId) {
		const element = document.getElementById(windowId);
		if (!element) return;

		if (element.classList.contains("modal")) {
			element.classList.remove("show");
			element.style.display = "none";
			document.body.style.overflow = "";
		} else {
			// Om det är en dropdown, stäng den och dess wrapper
			const wrapper = document
				.querySelector(`[data-target="${windowId}"]`)
				?.closest(".dropdown-wrapper");
			if (wrapper) {
				wrapper.classList.remove("active");
			}

			const dropdownContent = document.querySelector(
				`.dropdown-content-wrapper[data-for="${windowId}"]`
			);
			if (dropdownContent) {
				dropdownContent.classList.remove("active");
				const dropdown = dropdownContent.querySelector(".dropdown-content");
				if (dropdown) {
					dropdown.style.opacity = "0";
					dropdown.style.visibility = "hidden";
					dropdown.style.transform = "translateY(-10px)";
				}
			}
		}
	}

	closeAllWindows() {
		// Stäng alla modaler
		document.querySelectorAll(".modal.show").forEach((modal) => {
			modal.classList.remove("show");
			modal.style.display = "none";
		});
		document.body.style.overflow = "";

		this.closeAllDropdowns();
	}

	closeAllDropdowns() {
		// Stäng alla interna dropdowns
		document.querySelectorAll(".dropdown-wrapper.active").forEach((wrapper) => {
			wrapper.classList.remove("active");
		});

		// Stäng alla externa dropdowns
		document
			.querySelectorAll(".dropdown-content-wrapper.active")
			.forEach((content) => {
				content.classList.remove("active");
				const dropdown = content.querySelector(".dropdown-content");
				if (dropdown) {
					dropdown.style.opacity = "0";
					dropdown.style.visibility = "hidden";
					dropdown.style.transform = "translateY(-10px)";
				}
			});
	}
}

export default new WindowManager();
