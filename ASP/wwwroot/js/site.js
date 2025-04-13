document.addEventListener("DOMContentLoaded", () => {
	initSidebarState();
	initSidebar();
	initMobileMenu();
	initFileUploads();
	initFormSubmissions();
	initWysiwygEditors();

	if (window.location.pathname.includes("/projects")) {
		initProjectEditing();
		initProjectDeletion();
		initTabScrolling();
	}

	if (window.location.pathname.includes("/clients")) {
		initClientDeletion();
		initClientList();
	}

	if (window.location.pathname.includes("/members")) {
		initMemberDeletion();
		initMemberModalEvents();
		initMemberToggleAdmin();
	}
});

/*
 * ----------------------------------------------------------------------
 * Set sidebar state
 * ----------------------------------------------------------------------
 */

function initSidebarState() {
	const savedWidth = localStorage.getItem("sidebar-width");
	if (savedWidth) {
		const width = parseFloat(savedWidth);
		const minWidth = 4;

		// Set width via CSS variable
		document.documentElement.style.setProperty("--sidebar-width", `${width}rem`);

		// Add minimized class after DOM is ready
		if (width <= minWidth) {
			const sidebar = document.querySelector("aside");
			if (sidebar) sidebar.classList.add("minimized");
		}
	}
}

/*
 * ----------------------------------------------------------------------
 * Sidebar
 * ----------------------------------------------------------------------
 */

function initSidebar() {
	const sidebar = document.querySelector("aside");
	const resizer = document.querySelector(".sidebar-resizer");

	if (!sidebar || !resizer) return;

	// Initialize sidebar state
	const savedWidth = localStorage.getItem("sidebar-width");
	const defaultWidth = 20;
	const minWidth = 5;
	const mobileBreakpoint = 800;

	function expandSidebar() {
		document.documentElement.style.setProperty(
			"--sidebar-width",
			`${defaultWidth}rem`
		);
		sidebar.style.width = `${defaultWidth}rem`;
		sidebar.classList.remove("minimized");
		localStorage.setItem("sidebar-width", defaultWidth);
	}

	function minimizeSidebar() {
		document.documentElement.style.setProperty(
			"--sidebar-width",
			`${minWidth}rem`
		);
		sidebar.style.width = `${minWidth}rem`;
		sidebar.classList.add("minimized");
		localStorage.setItem("sidebar-width", minWidth);
	}

	function toggleSidebar() {
		const currentWidth = parseFloat(getComputedStyle(sidebar).width) / 16;
		currentWidth <= minWidth ? expandSidebar() : minimizeSidebar();
	}

	function handleResize() {
		if (window.innerWidth <= mobileBreakpoint) {
			sidebar.style.width = "";
			sidebar.classList.remove("minimized");
			document.documentElement.style.setProperty(
				"--sidebar-width",
				`${defaultWidth}rem`
			);
		} else if (savedWidth) {
			const width = parseFloat(savedWidth);
			width <= minWidth ? minimizeSidebar() : expandSidebar();
		} else {
			expandSidebar();
		}
	}

	// Event listeners
	resizer.addEventListener("click", (e) => {
		e.preventDefault();
		toggleSidebar();
	});

	window.addEventListener("resize", handleResize);

	// Initial setup
	handleResize();
}

/*
 * ----------------------------------------------------------------------
 * Mobile menu
 * ----------------------------------------------------------------------
 */

function initMobileMenu() {
	const hamburger = document.querySelector(".hamburger-checkbox");
	const mobileMenu = document.querySelector(".mobile-menu");
	const overlay = document.querySelector(".mobile-menu-overlay");

	if (!hamburger || !mobileMenu || !overlay) return;

	// Toggle menu on checkbox change
	hamburger.addEventListener("change", () => {
		const isOpen = hamburger.checked;
		mobileMenu.classList.toggle("open", isOpen);
		overlay.classList.toggle("open", isOpen);
		document.body.classList.toggle("mobile-menu-active", isOpen);
		document.body.style.overflow = isOpen ? "hidden" : "";

		if (!isOpen) {
			hamburger.checked = false;
		}
	});

	// Close menu when clicking overlay
	overlay.addEventListener("click", () => {
		hamburger.checked = false;
		mobileMenu.classList.remove("open");
		overlay.classList.remove("open");
		document.body.classList.remove("mobile-menu-active");
		document.body.style.overflow = "";
	});

	// Prevent menu clicks from closing menu
	mobileMenu.addEventListener("click", (e) => e.stopPropagation());

	// Handle screen size changes
	const mediaQuery = window.matchMedia("(min-width: 501px)");
	const handleScreenSizeChange = (e) => {
		if (e.matches && mobileMenu.classList.contains("open")) {
			hamburger.checked = false;
			mobileMenu.classList.remove("open");
			overlay.classList.remove("open");
			document.body.classList.remove("mobile-menu-active");
			document.body.style.overflow = "";
		}
	};

	mediaQuery.addEventListener("change", handleScreenSizeChange);
	handleScreenSizeChange(mediaQuery);
}

/*
 * ----------------------------------------------------------------------
 * Wysiwyg editors
 * ----------------------------------------------------------------------
 */

function initWysiwygEditors() {
	document.querySelectorAll(".wysiwyg-editor").forEach((editor) => {
		const toolbar = editor.previousElementSibling;
		const textarea = document.querySelector(editor.dataset.target);
		if (textarea) {
			const quill = new Quill(editor, {
				theme: "snow",
				modules: { toolbar: toolbar },
			});
			quill.on("text-change", () => (textarea.value = quill.root.innerHTML));
			if (textarea.value) quill.root.innerHTML = textarea.value;
		}
	});
}

/*
 * ----------------------------------------------------------------------
 * WYSIWYG Editor functionality
 * ----------------------------------------------------------------------
 */

function initWysiwygEditor(
	editorSelector,
	toolbarSelector,
	textareaSelector,
	initialContent = ""
) {
	const editor = document.querySelector(editorSelector);
	const toolbar = document.querySelector(toolbarSelector);
	const textarea = document.querySelector(textareaSelector);

	if (!editor || !toolbar || !textarea) return;

	const quill = new Quill(editor, {
		theme: "snow",
		modules: {
			toolbar: toolbar,
		},
	});

	// Set initial content if provided
	if (initialContent) {
		quill.root.innerHTML = initialContent;
	}

	// Update textarea when content changes
	quill.on("text-change", () => {
		textarea.value = quill.root.innerHTML;
	});
}

// Make function globally available
window.initWysiwygEditor = initWysiwygEditor;

/*
 * ----------------------------------------------------------------------
 * File uploads
 * ----------------------------------------------------------------------
 */

function initFileUploads() {
	document.querySelectorAll("[data-file-upload]").forEach((container) => {
		const input = container.querySelector('input[type="file"]');
		const preview = container.querySelector("img");
		const iconContainer = container.querySelector(".circle");
		const icon = iconContainer?.querySelector("i");

		container.addEventListener("click", () => input?.click());

		input?.addEventListener("change", (e) => {
			const file = e.target.files[0];
			if (file && file.type.startsWith("image/")) {
				const reader = new FileReader();
				reader.onload = () => {
					preview.src = reader.result;
					preview.classList.remove("hide");
					iconContainer.classList.add("circle-gray", "selected");
					icon?.classList.replace("fa-camera", "fa-pen-to-square");
				};
				reader.readAsDataURL(file);
			}
		});
	});
}

/*
 * ----------------------------------------------------------------------
 * Project editing
 * ----------------------------------------------------------------------
 */

function initProjectEditing() {
	document.addEventListener("click", async (e) => {
		const editTrigger = e.target.closest("[data-modal='editProjectModal']");
		if (!editTrigger) return;

		const projectId = editTrigger.dataset.id;
		if (!projectId) return;

		try {
			const response = await fetch(`/projects/${projectId}`);
			const data = await response.json();

			if (!data.success) {
				showToast("error", data.error || "Failed to load project details");
				return;
			}

			const project = data.project;

			// Populate form fields
			document.querySelector("#editProjectModal [name='Id']").value = project.id;
			document.querySelector("#editProjectModal [name='ProjectName']").value =
				project.projectName;
			document.querySelector("#editProjectModal [name='ClientId']").value =
				project.clientId;
			document.querySelector("#editProjectModal [name='Description']").value =
				project.description || "";
			document.querySelector("#editProjectModal [name='StartDate']").value =
				project.startDate?.split("T")[0] || "";
			document.querySelector("#editProjectModal [name='EndDate']").value =
				project.endDate?.split("T")[0] || "";
			document.querySelector("#editProjectModal [name='Budget']").value =
				project.budget || "";
			document.querySelector("#editProjectModal [name='StatusName']").value =
				project.statusName;

			// Initialize Quill editor with project description
			const quill = Quill.find(
				document.querySelector("#edit-project-description-editor")
			);
			quill.root.innerHTML = project.description || "";

			// Open modal
			WindowManager.openModal("editProjectModal");
		} catch (error) {
			console.error("Error fetching project details:", error);
			showToast("error", "Failed to load project details");
		}
	});
}

/*
 * ----------------------------------------------------------------------
 * Project deletion
 * ----------------------------------------------------------------------
 */

function initProjectDeletion() {
	document
		.querySelectorAll('[data-action="delete-project"]')
		.forEach((button) => {
			button.addEventListener("click", async (e) => {
				e.preventDefault();
				const id = button.getAttribute("data-id");
				if (id && confirm("Are you sure you want to delete this project?")) {
					try {
						const response = await fetch(`/api/projects/${id}`, { method: "DELETE" });
						if (response.ok) window.location.reload();
					} catch (error) {
						console.error("Error deleting project:", error);
					}
				}
			});
		});
}

/*
 * ----------------------------------------------------------------------
 * Tab scrolling
 * ----------------------------------------------------------------------
 */

function initTabScrolling() {
	const tabsContainer = document.querySelector(".projects-tabs");
	if (!tabsContainer) return;

	// Initialize horizontal scroll functionality
	let isDown = false;
	let startX;
	let scrollLeft;

	tabsContainer.addEventListener("mousedown", (e) => {
		isDown = true;
		tabsContainer.classList.add("active");
		startX = e.pageX - tabsContainer.offsetLeft;
		scrollLeft = tabsContainer.scrollLeft;
	});

	tabsContainer.addEventListener("mouseleave", () => {
		isDown = false;
		tabsContainer.classList.remove("active");
	});

	tabsContainer.addEventListener("mouseup", () => {
		isDown = false;
		tabsContainer.classList.remove("active");
	});

	tabsContainer.addEventListener("mousemove", (e) => {
		if (!isDown) return;
		e.preventDefault();
		const x = e.pageX - tabsContainer.offsetLeft;
		const walk = (x - startX) * 2;
		tabsContainer.scrollLeft = scrollLeft - walk;
	});

	// Handle window resize
	window.addEventListener("resize", () => {
		const hasOverflow = tabsContainer.scrollWidth > tabsContainer.clientWidth;
		tabsContainer.classList.toggle("has-overflow", hasOverflow);
	});

	// Handle scroll events
	tabsContainer.addEventListener("scroll", () => {
		const isAtEnd =
			tabsContainer.scrollLeft + tabsContainer.clientWidth >=
			tabsContainer.scrollWidth - 10;
		tabsContainer.classList.toggle("has-overflow", !isAtEnd);
	});

	// Initialize active tab from URL
	const url = new URL(window.location);
	const activeStatus = url.searchParams.get("tab") || "ALL";
	const activeTab = Array.from(tabsContainer.querySelectorAll(".tab")).find(
		(tab) => tab.textContent.trim().split("[")[0].trim() === activeStatus
	);
	if (activeTab) {
		activeTab.classList.add("active");
	}

	// Handle tab clicks
	tabsContainer.querySelectorAll(".tab").forEach((tab) => {
		tab.addEventListener("click", async (e) => {
			e.preventDefault();
			const status = tab.textContent.trim().split("[")[0].trim();

			// Update URL
			const url = new URL(window.location);
			url.searchParams.set("tab", status);
			window.history.pushState({}, "", url);

			// Update active state
			tabsContainer.querySelectorAll(".tab").forEach((t) => {
				t.classList.remove("active");
			});
			tab.classList.add("active");

			// Fetch filtered projects
			try {
				const response = await fetch(`/projects/list?tab=${status}`, {
					headers: {
						"X-Requested-With": "XMLHttpRequest",
					},
				});

				if (!response.ok) throw new Error("Failed to fetch projects");

				const projectsGrid = document.querySelector(".projects-grid");
				if (projectsGrid) {
					const html = await response.text();
					projectsGrid.innerHTML = html;
				}
			} catch (error) {
				console.error("Error fetching filtered projects:", error);
			}
		});
	});

	// Initial overflow check
	const hasOverflow = tabsContainer.scrollWidth > tabsContainer.clientWidth;
	tabsContainer.classList.toggle("has-overflow", hasOverflow);
}

/*
 * ----------------------------------------------------------------------
 * Form submissions
 * ----------------------------------------------------------------------
 */

function initFormSubmissions() {
	// Hantera alla formulär
	document.querySelectorAll("form").forEach((form) => {
		form.addEventListener("submit", async (e) => {
			e.preventDefault();
			await handleFormSubmit(form);
		});
	});

	// Specialhantering för projektformulär om de finns
	const addProjectForm = document.querySelector("#add-project-modal form");
	const editProjectForm = document.querySelector("#edit-project-modal form");

	if (addProjectForm) {
		addProjectForm.addEventListener("submit", async function (e) {
			e.preventDefault();
			await handleProjectFormSubmit(e.target, "/Projects/AddProject/");
		});
	}

	if (editProjectForm) {
		editProjectForm.addEventListener("submit", async function (e) {
			e.preventDefault();
			await handleProjectFormSubmit(e.target, "/Projects/UpdateProject/");
		});
	}
}

async function handleProjectFormSubmit(form, url) {
	const formData = new FormData(form);

	try {
		const response = await fetch(url, {
			method: "POST",
			body: formData,
		});

		const result = await response.json();

		if (result.success) {
			window.location.reload();
		} else {
			showValidationErrors(form, result.errors);
		}
	} catch (error) {
		console.error("Error submitting form:", error);
		handleFormError(form);
	}
}

async function handleFormSubmit(form) {
	clearFormErrorMessages(form);
	const formData = new FormData(form);

	// Debug logging
	console.log("Form data being sent:");
	for (let pair of formData.entries()) {
		console.log(pair[0] + ": " + pair[1]);
	}

	try {
		const response = await submitForm(form, formData);

		// Check if the response is JSON
		const contentType = response.headers.get("content-type");
		if (contentType && contentType.includes("application/json")) {
			const jsonResponse = await response.json();
			await handleFormResponse(jsonResponse, form);
		} else {
			// For HTML responses (like login/auth), handle redirect
			if (response.redirected) {
				window.location.href = response.url;
			} else if (response.ok) {
				// If no redirect but successful, reload the page
				window.location.reload();
			} else {
				// If error, show the error message
				const text = await response.text();
				const parser = new DOMParser();
				const doc = parser.parseFromString(text, "text/html");
				const errorMessage =
					doc.querySelector(".alert-notification.error")?.textContent ||
					"Ett fel uppstod. Kontrollera dina uppgifter och försök igen.";
				handleFormError(form, errorMessage);
			}
		}
	} catch (error) {
		console.error("Form submission error:", error);
		handleFormError(form);
	}
}

async function submitForm(form, formData) {
	const token = document.querySelector(
		'input[name="__RequestVerificationToken"]'
	).value;

	const response = await fetch(form.action, {
		method: form.method,
		headers: {
			RequestVerificationToken: token,
		},
		body: formData,
	});
	return response;
}

async function handleFormResponse(response, form) {
	if (response.success) {
		// Close modal if exists
		const modal = form.closest(".modal");
		if (modal) {
			modal.classList.remove("show");
		}

		// Reset the form
		form.reset();

		// Visa framgångsmeddelande
		showToast("success", "Operation completed successfully");

		// Refresh the page or update UI as needed
		if (form.dataset.reload !== "false") {
			window.location.reload();
		}
	} else {
		if (response.errors) {
			showValidationErrors(form, response.errors);
		} else {
			handleFormError(form);
		}
	}
}

function showToast(type, message) {
	const toast = document.createElement("div");
	toast.className = `toast toast-${type}`;
	toast.textContent = message;

	document.body.appendChild(toast);

	setTimeout(() => {
		toast.remove();
	}, 3000);
}

/*
 * ----------------------------------------------------------------------
 * Form validation
 * ----------------------------------------------------------------------
 */

function showValidationErrors(form, errors) {
	if (!errors) return;

	// Rensa tidigare fel
	clearFormErrorMessages(form);

	Object.entries(errors).forEach(([key, messages]) => {
		const input = form.querySelector(`[name="${key}"]`);
		if (input) {
			input.classList.add("input-error");
		}

		const span = form.querySelector(`[data-valmsg-for="${key}"]`);
		if (span) {
			span.innerText = Array.isArray(messages) ? messages.join(" ") : messages;
			span.classList.add("field-error");
		}
	});
}

function handleFormError(form, message) {
	// Skapa ett felmeddelande-element om det inte finns
	let errorElement = form.querySelector(".field-error");
	if (!errorElement) {
		errorElement = document.createElement("div");
		errorElement.className = "field-error";
		form.insertBefore(errorElement, form.firstChild);
	}

	// Sätt felmeddelandet
	errorElement.textContent =
		message ||
		"Ett fel uppstod när formuläret skulle skickas. Kontrollera att alla fält är korrekt ifyllda och försök igen.";

	// Lägg till visuell feedback
	form.classList.add("form-error");

	// Ta bort felmeddelandet efter 5 sekunder
	setTimeout(() => {
		errorElement.remove();
		form.classList.remove("form-error");
	}, 5000);
}

function clearFormErrorMessages(form) {
	// Rensa input-fel
	form.querySelectorAll('[data-val="true"]').forEach((input) => {
		input.classList.remove("input-error");
		input.removeAttribute("aria-invalid");
	});

	// Rensa alla felmeddelanden
	form.querySelectorAll("[data-valmsg-for]").forEach((span) => {
		span.innerText = "";
		span.classList.remove("field-error");
		span.removeAttribute("role");
	});

	// Rensa eventuella generella felmeddelanden
	const generalErrors = form.querySelectorAll(
		".field-error:not([data-valmsg-for])"
	);
	generalErrors.forEach((error) => error.remove());

	// Rensa form-klasser
	form.classList.remove("form-error");
}

/*
 * ----------------------------------------------------------------------
 * Client deletion
 * ----------------------------------------------------------------------
 */

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

/*
 * ----------------------------------------------------------------------
 * Client list
 * ----------------------------------------------------------------------
 */

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

/*
 * ----------------------------------------------------------------------
 * Member deletion
 * ----------------------------------------------------------------------
 */

function initMemberDeletion() {
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

/*
 * ----------------------------------------------------------------------
 * Member modal handling
 * ----------------------------------------------------------------------
 */

function initMemberModalEvents() {
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

async function editMember(id) {
	try {
		const response = await fetch(`/members/${id}`);
		if (!response.ok) throw new Error("Failed to fetch member");

		const member = await response.json();
		if (member.success) {
			populateEditForm(member.member);
			const modal = document.querySelector("#editMemberModal");
			if (modal) modal.classList.add("show");
		} else {
			showToast("error", "Failed to load member data");
		}
	} catch (error) {
		console.error("Error editing member:", error);
		showToast("error", "Failed to load member data");
	}
}

function populateEditForm(member) {
	const form = document.querySelector("#editMemberForm");
	if (!form) return;

	// Sätt ID och grundläggande information
	form.querySelector("#Id").value = member.id;
	form.querySelector("#FirstName").value = member.firstName;
	form.querySelector("#LastName").value = member.lastName;
	form.querySelector("#Email").value = member.email;
	form.querySelector("#PhoneNumber").value = member.phoneNumber || "";
	form.querySelector("#JobTitle").value = member.jobTitle || "";

	// Sätt adressinformation om den finns
	if (member.address) {
		form.querySelector("#StreetAddress").value =
			member.address.streetAddress || "";
		form.querySelector("#PostalCode").value = member.address.postalCode || "";
		form.querySelector("#City").value = member.address.city || "";
	}

	// Hantera profilbild
	const imagePreview = form.querySelector(".image-preview img");
	if (imagePreview && member.imageUrl) {
		imagePreview.src = member.imageUrl;
		imagePreview.classList.remove("hide");

		// Uppdatera cirkel-ikonen
		const circle = form.querySelector(".circle");
		const icon = circle?.querySelector("i");
		if (circle && icon) {
			circle.classList.add("circle-gray", "selected");
			icon.classList.replace("fa-camera", "fa-pen-to-square");
		}
	}
}

// Lägg till event listener för redigeringsformuläret
const editMemberForm = document.querySelector("#editMemberForm");
if (editMemberForm) {
	editMemberForm.addEventListener("submit", async (e) => {
		e.preventDefault();
		const formData = new FormData(editMemberForm);
		const id = formData.get("Id");

		try {
			const response = await fetch(`/Users/UpdateMember/${id}`, {
				method: "PUT",
				body: formData,
			});

			const result = await response.json();
			if (result.success) {
				// Stäng modalen
				const modal = editMemberForm.closest(".modal");
				if (modal) {
					modal.classList.remove("show");
				}

				showToast("success", "Member updated successfully");
				window.location.reload();
			} else {
				showValidationErrors(editMemberForm, result.errors);
			}
		} catch (error) {
			console.error("Error updating member:", error);
			handleFormError(editMemberForm);
		}
	});
}

/*
 * ----------------------------------------------------------------------
 * Members functionality
 * ----------------------------------------------------------------------
 */

function initMemberToggleAdmin() {
	document.querySelectorAll("[data-toggle-admin]").forEach((toggle) => {
		toggle.addEventListener("change", (e) => {
			const userId = toggle.getAttribute("data-id");
			const isAdmin = toggle.checked;
			toggleAdminRole(userId, isAdmin);
		});
	});
}

async function toggleAdminRole(userId, isAdmin) {
	try {
		const response = await fetch(`/Users/${userId}/toggle-admin`, {
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

async function deleteProject(id) {
	try {
		const response = await fetch(`/projects/${id}`, {
			method: "DELETE",
			headers: {
				"Content-Type": "application/json",
			},
		});

		const data = await response.json();

		if (!data.success) {
			showToast("error", data.error || "Failed to delete project");
			return;
		}

		// Close modal
		WindowManager.closeModal(`deleteProjectModal-${id}`);

		// Show success message
		showToast("success", "Project deleted successfully");

		// Reload projects list
		const activeTab =
			document.querySelector(".tab.active")?.textContent?.trim() || "ALL";
		const projectsResponse = await fetch(`/projects/list?tab=${activeTab}`);
		const projectsData = await projectsResponse.json();

		if (projectsData.success) {
			document.querySelector(".projects-grid").innerHTML = projectsData.projects
				.map((project) => {
					return `
						<div class="project-card">
							<!-- Project card content will be rendered server-side -->
						</div>
					`;
				})
				.join("");

			// Reload the page to get fresh server-side rendered content
			window.location.reload();
		}
	} catch (error) {
		console.error("Error deleting project:", error);
		showToast("error", "Failed to delete project");
	}
}
