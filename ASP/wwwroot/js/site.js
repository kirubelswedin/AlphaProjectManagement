// Initialize all core functionality
document.addEventListener("DOMContentLoaded", () => {
	initSidebarState();
	initSidebar();
	initMobileMenu();

	// Initialize project functionality if on projects page
	if (window.location.pathname.includes("/projects")) {
		initFileUploads();
		initProjectEditing();
		initProjectDeletion();
		initWysiwygEditors();
		initTabScrolling();
		initFormSubmissions();
	}
});

// Set sidebar state
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

// Initialize sidebar
function initSidebar() {
	const sidebar = document.querySelector("aside");
	const resizer = document.querySelector(".sidebar-resizer");

	if (!sidebar || !resizer) return;

	// Initialize sidebar state
	const savedWidth = localStorage.getItem("sidebar-width");
	const defaultWidth = 20; // rem
	const minWidth = 5; // rem
	const mobileBreakpoint = 800; // px

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

// Initialize mobile menu
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

document.addEventListener("closeModal", (e) => {
	WindowManager.closeWindow(e.detail);
});

// Global functions
window.toggleAdminRole = async (userId, isAdmin) => {
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
};

/*
 * ----------------------------------------------------------------------
 * Project-specific functions
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
					iconContainer.classList.add("selected");
					icon.classList.replace("fa-camera", "fa-pen-to-square");
				};
				reader.readAsDataURL(file);
			}
		});
	});
}

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

function initProjectEditing() {
	document.querySelectorAll('[data-action="edit-project"]').forEach((button) => {
		button.addEventListener("click", async (e) => {
			e.preventDefault();
			const id = button.getAttribute("data-id");
			if (id) {
				try {
					const response = await fetch(`/api/projects/${id}`);
					if (!response.ok) throw new Error("Failed to fetch project");

					const project = await response.json();
					const form = document.querySelector("#editProjectForm");
					if (form) {
						form.querySelector("#Id").value = project.id;
						form.querySelector("#ProjectName").value = project.projectName;
						form.querySelector("#ClientId").value = project.clientId;
						form.querySelector("#Description").value = project.description;
						form.querySelector("#StartDate").value = formatDate(project.startDate);
						form.querySelector("#EndDate").value = formatDate(project.endDate);
						form.querySelector("#Budget").value = project.budget;
						form.querySelector("#StatusId").value = project.statusId;

						const imagePreview = form.querySelector(".image-preview img");
						if (imagePreview && project.imageUrl) {
							imagePreview.src = project.imageUrl;
							imagePreview.classList.remove("hide");
						}
					}

					const modal = document.querySelector("#editProjectModal");
					if (modal) modal.classList.add("show");
				} catch (error) {
					console.error("Error editing project:", error);
				}
			}
		});
	});
}

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

function initFormSubmissions() {
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

	// Logga FormData för att se vad som skickas
	for (let pair of formData.entries()) {
		console.log(pair[0] + ": " + pair[1]);
	}

	const response = await fetch(url, {
		method: "POST",
		body: formData,
	});

	if (response.ok) {
		window.location.reload();
	} else {
		console.error("Error submitting form:", await response.text());
	}
}
