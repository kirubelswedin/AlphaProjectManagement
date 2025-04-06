class MemberManager {
	constructor() {
		this.initModalEvents();
		this.initDeleteMember();
	}

	initModalEvents() {
		document.addEventListener("openModal", (e) => {
			if (e.detail.modalId === "editMemberModal" && e.detail.id) {
				this.editMember(e.detail.id);
			}
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

			const data = await response.json();
			if (data.success) {
				showToast("success", data.message);
			} else {
				showToast("error", data.message || "Failed to update admin status");
			}
		} catch (error) {
			console.error("Error toggling admin role:", error);
			showToast("error", "Failed to update admin status");
		}
	}

	async editMember(id) {
		try {
			const response = await fetch(`/members/${id}`);
			const data = await response.json();

			if (data.success && data.member) {
				this.populateEditForm(data.member);
			}
		} catch (error) {
			console.error("Error fetching member:", error);
			showToast("error", "Failed to load member details");
		}
	}

	populateEditForm(member) {
		const form = document.getElementById("editMemberForm");
		if (!form) return;

		// Basic info
		form.querySelector("#memberId").value = member.id;
		form.querySelector("#FirstName").value = member.firstName || "";
		form.querySelector("#LastName").value = member.lastName || "";
		form.querySelector("#Email").value = member.email || "";
		form.querySelector("#PhoneNumber").value = member.phoneNumber || "";
		form.querySelector("#JobTitle").value = member.jobTitle || "";

		// Address fields
		if (member.address) {
			form.querySelector("#Address_StreetName").value =
				member.address.streetName || "";
			form.querySelector("#Address_PostalCode").value =
				member.address.postalCode || "";
			form.querySelector("#Address_City").value = member.address.city || "";
			form.querySelector("#Address_Country").value =
				member.address.country || "Sverige";
		}

		// Birth date
		if (member.birthDate) {
			const birthDate = new Date(member.birthDate);
			const day = String(birthDate.getDate()).padStart(2, "0");
			const month = String(birthDate.getMonth() + 1).padStart(2, "0");
			const year = birthDate.getFullYear();

			form.querySelector("#BirthDay").value = day;
			form.querySelector("#BirthMonth").value = month;
			form.querySelector("#BirthYear").value = year;
		}

		// Avatar
		const memberImage = form.querySelector("#memberImage");
		if (memberImage) {
			if (member.avatar) {
				memberImage.src = member.avatar;
				memberImage.style.display = "block";
			} else {
				memberImage.style.display = "none";
			}
		}
	}

	initDeleteMember() {
		window.deleteMember = async (id) => {
			if (confirm("Are you sure you want to delete this member?")) {
				try {
					const response = await fetch(`/members/${id}`, {
						method: "DELETE",
					});

					const data = await response.json();
					if (data.success) {
						showToast("success", data.message);
						const memberCard = document.querySelector(`[data-member-id="${id}"]`);
						if (memberCard) {
							memberCard.remove();
						}
					} else {
						showToast("error", data.message || "Failed to delete member");
					}
				} catch (error) {
					console.error("Error deleting member:", error);
					showToast("error", "Failed to delete member");
				}
			}
		};
	}
}

// Initialize the member manager when the DOM is loaded
document.addEventListener("DOMContentLoaded", () => {
	window.memberManager = new MemberManager();
});

export default new MemberManager();