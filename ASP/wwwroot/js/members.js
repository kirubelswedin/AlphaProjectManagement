class MemberManager {
	constructor() {
		this.init();
	}

	init() {
		this.initModalEvents();
		this.initDeleteMember();
		this.initToggleAdmin();
	}

	initToggleAdmin() {
		document.querySelectorAll("[data-toggle-admin]").forEach((toggle) => {
			toggle.addEventListener("click", (e) => {
				const userId = toggle.getAttribute("data-id");
				const isAdmin = toggle.getAttribute("data-is-admin") === "true";
				this.toggleAdminRole(userId, !isAdmin);
			});
		});
	}

	initModalEvents() {
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

	async toggleAdminRole(userId, isAdmin) {
		try {
			const response = await fetch(`/members/${userId}/toggle-admin`, {
				method: "POST",
				headers: {
					"Content-Type": "application/json",
				},
				body: JSON.stringify(isAdmin),
			});

			if (!response.ok) throw new Error("Failed to update admin role");

			window.location.reload();
		} catch (error) {
			console.error("Error toggling admin role:", error);
		}
	}

	async editMember(id) {
		try {
			const response = await fetch(`/members/${id}`);
			if (!response.ok) throw new Error("Failed to fetch member");

			const member = await response.json();
			this.populateEditForm(member);

			const modal = document.querySelector("#editMemberModal");
			if (modal) modal.classList.add("show");
		} catch (error) {
			console.error("Error editing member:", error);
		}
	}

	populateEditForm(member) {
		const form = document.querySelector("#editMemberForm");
		if (!form) return;

		form.querySelector("#Id").value = member.id;
		form.querySelector("#FirstName").value = member.firstName;
		form.querySelector("#LastName").value = member.lastName;
		form.querySelector("#Email").value = member.email;
		form.querySelector("#PhoneNumber").value = member.phoneNumber;
		form.querySelector("#JobTitle").value = member.jobTitle;
		form.querySelector("#StreetName").value = member.streetName;
		form.querySelector("#PostalCode").value = member.postalCode;
		form.querySelector("#City").value = member.city;

		const imagePreview = form.querySelector(".image-preview img");
		if (imagePreview && member.imageUrl) {
			imagePreview.src = member.imageUrl;
			imagePreview.classList.remove("hide");
		}
	}

	initDeleteMember() {
		document
			.querySelectorAll('[data-action="delete-member"]')
			.forEach((button) => {
				button.addEventListener("click", async (e) => {
					e.preventDefault();
					const id = button.getAttribute("data-id");
					if (!id) return;

					if (confirm("Are you sure you want to delete this member?")) {
						try {
							const response = await fetch(`/members/${id}`, {
								method: "DELETE",
							});

							if (response.ok) {
								window.location.reload();
							} else {
								throw new Error("Failed to delete member");
							}
						} catch (error) {
							console.error("Error deleting member:", error);
						}
					}
				});
			});
	}
}

// Initialize manager when DOM is loaded
document.addEventListener("DOMContentLoaded", () => {
	window.memberManager = new MemberManager();
});

export default new MemberManager();
