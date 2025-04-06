class ClientManager {
	constructor() {
		this.init();
	}

	init() {
		this.initModalEvents();
		this.initDeleteClient();
	}

	initModalEvents() {
		document.addEventListener("openModal", (e) => {
			if (e.detail.modalId === "editClientModal" && e.detail.id) {
				this.editClient(e.detail.id);
			}
		});
	}

	async editClient(id) {
		try {
			const response = await fetch(`/clients/${id}`);
			const data = await response.json();

			if (data.success) {
				const client = data.client;
				this.populateEditForm(client);
				openModal("editClientModal");
			} else {
				showToast("error", "Failed to load client");
			}
		} catch (error) {
			console.error("Error editing client:", error);
			showToast("error", "Failed to load client");
		}
	}

	populateEditForm(client) {
		const form = document.getElementById("editClientForm");
		if (!form) return;

		// Basic info
		form.querySelector("#clientId").value = client.id;
		form.querySelector("#clientName").value = client.clientName;
		form.querySelector("#contactPerson").value = client.contactPerson;
		form.querySelector("#email").value = client.email;
		form.querySelector("#phoneNumber").value = client.phoneNumber || "";
		form.querySelector("#website").value = client.website || "";

		// Address
		if (client.address) {
			form.querySelector("#streetName").value = client.address.streetName || "";
			form.querySelector("#postalCode").value = client.address.postalCode || "";
			form.querySelector("#city").value = client.address.city || "";
			form.querySelector("#country").value = client.address.country || "";
		}

		// Client logo
		const clientImage = form.querySelector(
			".client-image-placeholder img.preview-image"
		);
		if (clientImage && client.image) {
			clientImage.src = client.image;
			clientImage.style.display = "block";
		} else if (clientImage) {
			clientImage.style.display = "none";
		}
	}

	resetClientForm() {
		const form = document.getElementById("addClientForm");
		if (form) {
			form.reset();
		}
	}

	initDeleteClient() {
		window.deleteClient = async (id) => {
			if (confirm("Are you sure you want to delete this client?")) {
				try {
					const response = await fetch(`/clients/${id}`, {
						method: "DELETE",
					});

					const data = await response.json();
					if (data.success) {
						const clientRow = document.querySelector(`tr[data-id="${id}"]`);
						if (clientRow) {
							clientRow.remove();
						}
						showToast("success", data.message);
					} else {
						showToast("error", data.message || "Failed to delete client");
					}
				} catch (error) {
					console.error("Error deleting client:", error);
					showToast("error", "Failed to delete client");
				}
			}
		};
	}
}

// Initialize the client manager when the DOM is loaded
document.addEventListener("DOMContentLoaded", () => {
	window.clientManager = new ClientManager();
});

export default new ClientManager();