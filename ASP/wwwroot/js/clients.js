// Clients functionality
document.addEventListener("DOMContentLoaded", () => {
	initModals();
	initFileUploads();
	initClientDeletion();
	initClientList();
});

function initModals() {
	// Initialize modal events
	document.querySelectorAll('[data-type="modal"]').forEach((trigger) => {
		const target = document.querySelector(trigger.getAttribute("data-target"));
		trigger.addEventListener("click", () => {
			target?.classList.add("show");
		});
	});

	document.querySelectorAll('[data-type="close"]').forEach((btn) => {
		const target = document.querySelector(btn.getAttribute("data-target"));
		btn.addEventListener("click", () => {
			target?.classList.remove("show");
		});
	});
}

function initFileUploads() {
	document.querySelectorAll('input[type="file"]').forEach((input) => {
		const preview = input
			.closest(".image-preview-container")
			?.querySelector("img");
		if (preview) {
			input.addEventListener("change", function () {
				if (this.files && this.files[0]) {
					const reader = new FileReader();
					reader.onload = (e) => {
						preview.src = e.target.result;
						preview.classList.remove("hide");
					};
					reader.readAsDataURL(this.files[0]);
				}
			});
		}
	});
}

function initClientDeletion() {
	document
		.querySelectorAll('[data-action="delete-client"]')
		.forEach((button) => {
			button.addEventListener("click", async function (e) {
				e.preventDefault();
				const id = this.getAttribute("data-id");
				if (id && confirm("Are you sure you want to delete this client?")) {
					try {
						const response = await fetch(`/api/clients/${id}`, {
							method: "DELETE",
						});
						if (response.ok) {
							window.location.reload();
						}
					} catch (error) {
						console.error("Error deleting client:", error);
					}
				}
			});
		});
}

function initClientList() {
	const selectAllCheckbox = document.querySelector("#selectAllClients");
	const selectedInfo = document.querySelector(".selected-clients-info");
	const selectedCount = document.querySelector(".selected-clients-count");
	const unselectAllButton = document.querySelector(".unselect-all");
	let selectedClients = [];

	if (selectAllCheckbox) {
		selectAllCheckbox.addEventListener("change", () => {
			const checkboxes = document.querySelectorAll(".client-checkbox");
			checkboxes.forEach((checkbox) => {
				checkbox.checked = selectAllCheckbox.checked;
				if (selectAllCheckbox.checked) {
					selectedClients.push(checkbox.getAttribute("data-id"));
				} else {
					selectedClients = [];
				}
			});
			updateSelectedClientsInfo();
		});
	}

	document.querySelectorAll(".client-checkbox").forEach((checkbox) => {
		checkbox.addEventListener("change", () => {
			const clientId = checkbox.getAttribute("data-id");
			if (checkbox.checked) {
				if (!selectedClients.includes(clientId)) {
					selectedClients.push(clientId);
				}
			} else {
				selectedClients = selectedClients.filter((id) => id !== clientId);
				if (selectAllCheckbox) {
					selectAllCheckbox.checked = false;
				}
			}
			updateSelectedClientsInfo();
		});
	});

	if (unselectAllButton) {
		unselectAllButton.addEventListener("click", () => {
			document.querySelectorAll(".client-checkbox").forEach((checkbox) => {
				checkbox.checked = false;
			});
			if (selectAllCheckbox) {
				selectAllCheckbox.checked = false;
			}
			selectedClients = [];
			updateSelectedClientsInfo();
		});
	}

	function updateSelectedClientsInfo() {
		if (selectedInfo && selectedCount) {
			if (selectedClients.length > 0) {
				selectedInfo.style.display = "flex";
				selectedCount.textContent = `(${selectedClients.length} selected)`;
			} else {
				selectedInfo.style.display = "none";
			}
		}
	}

	// Initialize edit client functionality
	document.addEventListener("editClient", async (e) => {
		try {
			const response = await fetch(`/clients/${e.detail}`);
			if (!response.ok) throw new Error("Client not found");

			const client = await response.json();
			populateEditForm(client);
		} catch (error) {
			console.error("Error fetching client:", error);
			alert("Could not load client data");
		}
	});

	function populateEditForm(client) {
		const fields = ["id", "clientName", "contactPerson", "email", "phone"];
		fields.forEach((field) => {
			const element = document.getElementById(field);
			if (element) element.value = client[field] || "";
		});

		const modalLabel = document.getElementById("addclientmodalLabel");
		if (modalLabel) modalLabel.textContent = "Add Client";

		const event = new CustomEvent("openModal", { detail: "addclientmodal" });
		document.dispatchEvent(event);
	}

	// Initialize form reset
	const form = document.getElementById("addClientForm");
	if (form) {
		form.addEventListener("reset", () => {
			document.getElementById("clientId").value = "";
			document.getElementById("addclientmodalLabel").textContent = "Add Client";
		});
	}
}
