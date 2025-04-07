import { SELECTORS, CLASSES } from "./constants.js";
import WindowManager from "./windowManager.js";

/**
 * Handles project management functionalities
 */

export default class ProjectManager {
	constructor() {
		this.init();
	}

	init() {
		this.initModalEvents();
		this.initImagePreview();
		this.initDeleteProject();
		this.initWysiwygEditors();
	}

	initModalEvents() {
		document.addEventListener("openModal", (e) => {
			if (e.detail.modalId === "editProjectModal" && e.detail.id) {
				this.editProject(e.detail.id);
			}
		});

		const addProjectModal = document.getElementById("addProjectModal");
		if (addProjectModal) {
			addProjectModal.addEventListener("hidden.bs.modal", () =>
				this.resetProjectForm()
			);
		}
	}

	async editProject(id) {
		try {
			const response = await fetch(`/projects/${id}`);
			const data = await response.json();

			if (data.success) {
				const project = data.project;
				this.populateEditForm(project);
				WindowManager.openModal("editProjectModal");
			} else {
				showToast("error", "Failed to load project");
			}
		} catch (error) {
			console.error("Error editing project:", error);
			showToast("error", "Failed to load project");
		}
	}

	populateEditForm(project) {
		const form = document.getElementById("projectForm");
		if (!form) return;

		// Basic info
		form.querySelector("#Id").value = project.id;
		form.querySelector("#Name").value = project.projectName;
		form.querySelector("#Description").value = project.description || "";
		form.querySelector("#StartDate").value = this.formatDate(project.startDate);
		form.querySelector("#EndDate").value = this.formatDate(project.endDate);
		form.querySelector("#Budget").value = project.budget || "";

		// Status
		const statusSelect = form.querySelector("#StatusId");
		if (statusSelect && project.statusId) {
			statusSelect.value = project.statusId;
		}

		// Client
		const clientSelect = document.getElementById("projectClientSelect");
		const clientContainer = clientSelect
			?.closest(".form-field")
			.querySelector(".selected-items-list");
		const clientInput = form.querySelector('input[name="ClientId"]');

		if (clientSelect && clientInput && project.clientId) {
			clientInput.value = project.clientId;
			this.updateSelected(clientSelect, clientContainer, clientInput, false);
		}

		// Members
		const memberSelect = document.getElementById("projectMemberSelect");
		const memberContainer = memberSelect
			?.closest(".form-field")
			.querySelector(".selected-items-list");
		const memberInput = form.querySelector('input[name="MemberIds"]');

		if (memberSelect && project.teamMembers && memberContainer && memberInput) {
			// Reset all selections
			Array.from(memberSelect.options).forEach((option) => {
				option.selected = false;
			});

			// Select team members
			project.teamMembers.forEach((member) => {
				const option = memberSelect.querySelector(`option[value="${member.id}"]`);
				if (option) {
					option.selected = true;
				}
			});

			// Update display
			this.updateSelected(memberSelect, memberContainer, memberInput, true);
		}

		// Project image
		const projectImage = form.querySelector(".project-image-placeholder img");
		if (projectImage && project.imageUrl) {
			projectImage.src = project.imageUrl;
			projectImage.style.display = "block";
		} else if (projectImage) {
			projectImage.style.display = "none";
		}

		// Initialize WYSIWYG editor with project description
		if (typeof window.initWysiwygEditor === "function") {
			window.initWysiwygEditor(
				"#edit-project-description-editor",
				"#edit-project-description-toolbar",
				"#Description",
				project.description || ""
			);
		}
	}

	formatDate(dateString) {
		if (!dateString) return "";
		const date = new Date(dateString);
		return date.toISOString().split("T")[0];
	}

	resetProjectForm() {
		const form = document.getElementById("projectForm");
		if (form) {
			form.reset();

			// Reset member selection
			const memberSelect = document.getElementById("projectMemberSelect");
			const memberContainer = memberSelect
				?.closest(".form-field")
				.querySelector(".selected-items-list");
			const memberInput = form.querySelector('input[name="MemberIds"]');

			if (memberSelect && memberContainer && memberInput) {
				Array.from(memberSelect.options).forEach((option) => {
					option.selected = false;
				});
				this.updateSelected(memberSelect, memberContainer, memberInput, true);
			}

			// Reset client selection
			const clientSelect = document.getElementById("projectClientSelect");
			const clientContainer = clientSelect
				?.closest(".form-field")
				.querySelector(".selected-items-list");
			const clientInput = form.querySelector('input[name="ClientId"]');

			if (clientContainer && clientInput && clientSelect) {
				clientInput.value = "";
				this.updateSelected(clientSelect, clientContainer, clientInput, false);
			}

			// Reset image preview
			const projectImage = form.querySelector(".project-image-placeholder img");
			if (projectImage) {
				projectImage.style.display = "none";
				projectImage.src = "";
			}
		}
	}

	initDeleteProject() {
		window.deleteProject = async (id) => {
			if (confirm("Are you sure you want to delete this project?")) {
				try {
					const response = await fetch(`/projects/${id}`, {
						method: "DELETE",
					});

					const data = await response.json();
					if (data.success) {
						const projectCard = document.querySelector(
							`.project-card[data-id="${id}"]`
						);
						if (projectCard) {
							projectCard.remove();
						}
						showToast("success", data.message);
					} else {
						showToast("error", data.message || "Failed to delete project");
					}
				} catch (error) {
					console.error("Error deleting project:", error);
					showToast("error", "Failed to delete project");
				}
			}
		};
	}

	initImagePreview() {
		const imageInput = document.getElementById("add-project-image");
		const placeholder = document.querySelector(".project-image-placeholder");

		imageInput?.addEventListener("change", function (e) {
			const file = e.target.files[0];
			if (file) {
				const reader = new FileReader();
				reader.onload = function (e) {
					// Create or update preview image
					let previewImg = placeholder.querySelector("img.preview-image");
					if (!previewImg) {
						previewImg = document.createElement("img");
						previewImg.classList.add("preview-image");
						placeholder.insertBefore(previewImg, placeholder.firstChild);
					}
					previewImg.src = e.target.result;

					// Hide the upload overlay when image is selected
					const overlay = placeholder.querySelector(".upload-overlay");
					if (overlay) {
						overlay.style.display = "none";
					}
				};
				reader.readAsDataURL(file);
			}
		});
	}

	updateSelected(select, container, input, isMultiple) {
		if (!select || !container || !input) return;

		// Clear existing items
		container.innerHTML = "";

		// Get selected options
		const selectedOptions = Array.from(select.selectedOptions);

		if (selectedOptions.length === 0) {
			container.innerHTML = `<span class="placeholder">${select.getAttribute(
				"data-placeholder"
			)}</span>`;
			input.value = "";
			return;
		}

		// Add selected items to container
		selectedOptions.forEach((option) => {
			const item = document.createElement("div");
			item.className = "selected-item";
			item.textContent = option.textContent;
			container.appendChild(item);
		});

		// Update hidden input value
		if (isMultiple) {
			input.value = selectedOptions.map((option) => option.value).join(",");
		} else {
			input.value = selectedOptions[0].value;
		}
	}

	initWysiwygEditors() {
		const editors = document.querySelectorAll(SELECTORS.WYSIWYG_EDITOR);
		if (editors.length > 0 && typeof window.initWysiwygEditor === "function") {
			editors.forEach((editor) => {
				const toolbar = editor.querySelector(SELECTORS.WYSIWYG_TOOLBAR);
				const textarea = editor.querySelector(SELECTORS.WYSIWYG_TEXTAREA);
				if (toolbar && textarea) {
					window.initWysiwygEditor(editor, toolbar, textarea);
				}
			});
		}
	}
}
