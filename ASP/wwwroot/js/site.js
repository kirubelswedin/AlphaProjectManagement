document.addEventListener("DOMContentLoaded", () => {
	initSidebar();
	initMobileMenu();
	initFileUploads();
	initPasswordToggle();

	if (window.location.pathname.includes("/projects")) {
		initProjectForm();
		initProjectDeletion();
		initTabScrolling();
		initTabFilter();
		initAvatarVisibility();
		initBudgetFormatter();
	}

	if (window.location.pathname.includes("/clients")) {
		initAddClientForm();
		initClientList();
		initClientDeletion();
	}

	if (window.location.pathname.includes("/members")) {
		initMemberForm();
		initMemberDeletion();
		initMemberToggleAdmin();
	}
});

/*
 * ----------------------------------------------------------------------
 * sidebar resizer : Handles responsive, persistent, and interactive sidebar
 * ----------------------------------------------------------------------
 */
const SIDEBAR_DEFAULT_WIDTH = 20; // rem
const SIDEBAR_MIN_WIDTH = 4; // rem
const SIDEBAR_MOBILE_BREAKPOINT = 50;	// rem

// took some help from chatGPT to get this to work as I wanted
function initSidebar() {
	const sidebar = document.querySelector("aside");
	const resizer = document.querySelector(".sidebar-resizer");
	if (!sidebar || !resizer) return;

	// Helper to set sidebar width, minimized state, and persist to localStorage
	function setSidebarState(widthRem, minimized) {
		document.documentElement.style.setProperty("--sidebar-width", `${widthRem}rem`);
		// sidebar.style.width = `${widthRem}rem`;
		sidebar.classList.toggle("minimized", minimized);
		document.body.classList.toggle("sidebar-minimized", minimized);
		localStorage.setItem("sidebar-width", widthRem);
	}

	// Restore sidebar width from localStorage if available
	const savedWidth = parseFloat(localStorage.getItem("sidebar-width"));
	if (!isNaN(savedWidth)) {
		setSidebarState(savedWidth, savedWidth <= SIDEBAR_MIN_WIDTH);
	}

	// Expand sidebar to default width and update state
	function expandSidebar() {
		setSidebarState(SIDEBAR_DEFAULT_WIDTH, false);
	}

	// Minimize sidebar and update state
	function minimizeSidebar() {
		setSidebarState(SIDEBAR_MIN_WIDTH, true);
	}

	// Toggle sidebar between expanded and minimized
	function toggleSidebar() {
		const currentWidth = parseFloat(getComputedStyle(sidebar).width) / 16;
		if (currentWidth <= SIDEBAR_MIN_WIDTH) {
			expandSidebar();
		} else {
			minimizeSidebar();
		}
	}

	// Adjust sidebar for window resize and mobile breakpoint (rem)
	function handleResize() {
		// Convert px to rem for responsive breakpoint comparison
		const windowWidthRem = window.innerWidth / 16;
		if (windowWidthRem <= SIDEBAR_MOBILE_BREAKPOINT) {
			// On mobile: reset sidebar to default width and remove minimized state
			// sidebar.style.width = "";
			document.documentElement.style.setProperty("--sidebar-width", `${SIDEBAR_DEFAULT_WIDTH}rem`);
			sidebar.classList.remove("minimized");
			document.body.classList.remove("sidebar-minimized");
			
		} else {
			// On desktop: restore saved width or use default
			const width = parseFloat(localStorage.getItem("sidebar-width"));
			if (!isNaN(width)) {
				if (width <= SIDEBAR_MIN_WIDTH) {
					minimizeSidebar();
				} else {
					expandSidebar();
				}
			} else {
				expandSidebar();
			}
		}
	}

	// Listen for clicks on the resizer to toggle sidebar
	resizer.addEventListener("click", (e) => {
		e.preventDefault();
		toggleSidebar();
	});

	// Listen for window resize to adjust sidebar responsively
	window.addEventListener("resize", handleResize);

	handleResize();
}

/*
 * ----------------------------------------------------------------------
 * Mobile menu : Handles hamburger toggle, overlay, and responsive reset
 * ----------------------------------------------------------------------
 */
function initMobileMenu() {
	const hamburger = document.querySelector(".hamburger-checkbox");
	const mobileMenu = document.querySelector(".mobile-menu");
	const overlay = document.querySelector(".mobile-menu-overlay");

	if (!hamburger || !mobileMenu || !overlay) return;

	// Helper to open or close the mobile menu and update all related states.
	function setMenuState(open) {
		mobileMenu.classList.toggle("open", open);
		overlay.classList.toggle("open", open);
		document.body.classList.toggle("mobile-menu-active", open);
		document.body.style.overflow = open ? "hidden" : "";
		hamburger.checked = open;
	}

	// Toggle menu on hamburger checkbox change
	hamburger.addEventListener("change", () => {
		setMenuState(hamburger.checked);
	});

	// Close menu when clicking overlay
	overlay.addEventListener("click", () => {
		setMenuState(false);
	});

	// Prevent menu clicks from closing menu 
	mobileMenu.addEventListener("click", (e) => e.stopPropagation());

	// Handle screen size changes: close menu if resizing to desktop
	const mediaQuery = window.matchMedia("(min-width: 501px)");
	function handleScreenSizeChange(e) {
		if (e.matches) {
			setMenuState(false);
		}
	}
	mediaQuery.addEventListener("change", handleScreenSizeChange);

	// Initial responsive check
	handleScreenSizeChange(mediaQuery);
}

/*
 * ----------------------------------------------------------------------
 * WYSIWYG Editor functionality : Initializes Quill editor with toolbar and syncs content to textarea
 * ----------------------------------------------------------------------
 */
function initWysiwygEditor(editorSelector, toolbarSelector, textareaSelector, content = ""
) {
	const editor = document.querySelector(editorSelector);
	const toolbar = document.querySelector(toolbarSelector);
	const textarea = document.querySelector(textareaSelector);

	if (!editor || !toolbar || !textarea) return;

	// Initialize Quill editor, syntax highlighting and custom toolbar
	const quill = new Quill(editor, {
		theme: "snow",
		modules: {
			syntax: true,
			toolbar: toolbar,
		},
		placeholder: "Type something",
	});

	// Set initial content if provided
	if (content) {
		quill.root.innerHTML = content;
		textarea.value = content;
	}

	// Sync editor content
	quill.on("text-change", () => {
		textarea.value = quill.root.innerHTML;
	});
}

// Make function globally available
window.initWysiwygEditor = initWysiwygEditor;

/*
 * ----------------------------------------------------------------------
 * File uploads: Handles image preview and container state for custom file inputs
 * ----------------------------------------------------------------------
 */
function initFileUploads() {
// Initializes file upload components: handles image preview, click-to-upload, and state.
	document.querySelectorAll("[data-file-upload]").forEach((container) => {
		const input = container.querySelector('input[type="file"]');
		const preview = container.querySelector("img");
		if (!input || !preview) return;

		// Updates the container's state and preview visibility based on image presence.
		function setImageState(hasImage, src = "") {
			container.classList.toggle("has-image", hasImage);
			preview.classList.toggle("hide", !hasImage);
			preview.src = src;
		}

		// Allow clicking anywhere in the container
		container.addEventListener("click", (e) => {
			if (e.target !== input) {
				input.click();
			}
		});

		// Handle file selection and preview
		input.addEventListener("change", (e) => {
			const file = e.target.files[0];
			if (file && file.type.startsWith("image/")) {
				const reader = new FileReader();
				reader.onload = () => setImageState(true, reader.result);
				reader.readAsDataURL(file);
			} else {
				// reset preview and state if not an image or no file removed
				setImageState(false, "");
			}
		});
	});
}

function updateImagePreviewState(container, hasImage) {
	if (hasImage) {
		container.classList.add("has-image");
	} else {
		container.classList.remove("has-image");
	}
}

/*
 * ----------------------------------------------------------------------
 * Tab scrolling : Enables horizontal drag-to-scroll for project tabs
 * ----------------------------------------------------------------------
 */
// took some help from chatGPT to get this to work as I wanted
function initTabScrolling() {
	const tabsContainer = document.querySelector(".projects-tabs");
	if (!tabsContainer) return;

	// State for drag-to-scroll
	let isDown = false;
	let startX = 0;
	let scrollLeft = 0;

	// Mouse down: start drag
	tabsContainer.addEventListener("mousedown", (e) => {
		isDown = true;
		tabsContainer.classList.add("active");
		startX = e.pageX - tabsContainer.offsetLeft;
		scrollLeft = tabsContainer.scrollLeft;
	});

	// Mouse leave/up: end drag
	["mouseleave", "mouseup"].forEach(event =>
		tabsContainer.addEventListener(event, () => {
			isDown = false;
			tabsContainer.classList.remove("active");
		})
	);

	// Mouse move: perform drag
	tabsContainer.addEventListener("mousemove", (e) => {
		if (!isDown) return;
		e.preventDefault();
		const x = e.pageX - tabsContainer.offsetLeft;
		const walk = (x - startX) * 2; // scroll speed
		tabsContainer.scrollLeft = scrollLeft - walk;
	});

	// Update overflow on resize and scroll
	function updateOverflow() {
		const hasOverflow = tabsContainer.scrollWidth > tabsContainer.clientWidth;
		tabsContainer.classList.toggle("has-overflow", hasOverflow);

		const isAtEnd =
			tabsContainer.scrollLeft + tabsContainer.clientWidth >=
			tabsContainer.scrollWidth - 10;
		tabsContainer.classList.toggle("has-overflow", hasOverflow && !isAtEnd);
	}
	window.addEventListener("resize", updateOverflow);
	tabsContainer.addEventListener("scroll", updateOverflow);

	updateOverflow();
}

/*
 * ----------------------------------------------------------------------
 * Tab Filter : Handles tab selection, URL sync, and AJAX project filtering
 * ----------------------------------------------------------------------
 */
function initTabFilter() {
	const tabsContainer = document.querySelector(".projects-tabs");
	if (!tabsContainer) return;

	// Highlight active tab based on URL parameter
	const url = new URL(window.location);
	const activeStatus = url.searchParams.get("tab") || "ALL";
	const tabs = Array.from(tabsContainer.querySelectorAll(".tab"));
	const activeTab = tabs.find(
		(tab) => tab.textContent.trim().split("[")[0].trim() === activeStatus
	);
	if (activeTab) {
		activeTab.classList.add("active");
	}

	// Handle tab clicks
	tabs.forEach((tab) => {
		tab.addEventListener("click", async (e) => {
			e.preventDefault();
			const status = tab.textContent.trim().split("[")[0].trim();

			// Update URL without reloading page
			const url = new URL(window.location);
			url.searchParams.set("tab", status);
			window.history.pushState({}, "", url);

			// Update active tab styling
			tabs.forEach((t) => t.classList.remove("active"));
			tab.classList.add("active");

			// Fetch and update projectlist via AJAX
			try {
				const response = await fetch(`/projects/list?tab=${status}`);
				const html = await response.text();
				const projectsGrid = document.querySelector(".projects-grid");
				if (projectsGrid) {
					projectsGrid.innerHTML = html;
				}
			} catch (err) {
				console.error("Error fetching filtered projects:", err);
			}
		});
	});
}

/*
* ----------------------------------------------------------------------
* Avatar visibility : Dynamically shows/hides avatars and counter based on container width
* ----------------------------------------------------------------------
*/
// took some help from chatGPT to get this to work as I wanted
const AVATAR_OVERLAP = 8;
const MAX_VISIBLE_AVATARS = 8;
const AVATAR_DEFAULT_WIDTH = 40;
const COUNTER_WIDTH = 32;
const SAFETY_MARGIN = 20;

// Helper to calculate total width needed for N avatars (and counter if needed)
function calculateAvatarsWidth(avatarCount, avatarWidth, overlap, showCounter) {
	const avatarsWidth = avatarWidth + (avatarCount - 1) * (avatarWidth - overlap);
	const counterWidth = showCounter ? (COUNTER_WIDTH + SAFETY_MARGIN) : 0;
	return avatarsWidth + counterWidth;
}

function initAvatarVisibility() {
	function updateAvatarVisibility() {
		document.querySelectorAll('.team-members').forEach(container => {
			const allAvatars = Array.from(container.querySelectorAll('.avatar:not(.more-members)'));
			const visibleAvatars = allAvatars.filter(a => !a.classList.contains('hide'));
			const counter = container.querySelector('.more-members');
			if (!visibleAvatars.length) {
				if (counter) counter.style.display = 'none';
				return;
			}

			// Reset all avatars to visible, hide counter
			visibleAvatars.forEach(a => a.style.display = '');
			if (counter) counter.style.display = 'none';

			const containerWidth = container.offsetWidth;
			const avatarWidth = visibleAvatars[0]?.offsetWidth || AVATAR_DEFAULT_WIDTH;
			const maxPossible = Math.min(MAX_VISIBLE_AVATARS, visibleAvatars.length);

			// Try to fit as many avatars as possible, adding the counter only if needed
			let visibleCount = 1;
			for (let n = maxPossible; n >= 1; n--) {
				const needsCounter = visibleAvatars.length > n;
				const totalWidth = calculateAvatarsWidth(n, avatarWidth, AVATAR_OVERLAP, needsCounter);
				if (totalWidth <= containerWidth) {
					visibleCount = n;
					break;
				}
			}

			// Show only the avatars that fit
			visibleAvatars.forEach((a, i) => {
				a.style.display = i < visibleCount ? '' : 'none';
			});

			// Show the counter if avatars are hidden
			const hiddenCount = allAvatars.length - visibleCount;
			if (counter) {
				if (hiddenCount > 0) {
					counter.style.display = '';
					const span = counter.querySelector('span');
					if (span) span.textContent = `+${hiddenCount}`;
				} else {
					counter.style.display = 'none';
				}
			}
		});
	}

	updateAvatarVisibility();
	window.addEventListener('resize', updateAvatarVisibility);
	setTimeout(updateAvatarVisibility, 100);
}

/*
 * ----------------------------------------------------------------------
 * Project budget formatting: allows only numbers, max 8 digits, spaces as thousands separator
 * ----------------------------------------------------------------------
 */
function initBudgetFormatter() {
	// console.log("BudgetFormatter initialized");

	document.querySelectorAll('input[data-type="budget"]').forEach(input => {
		input.addEventListener("input", handleBudgetInput);
		formatBudgetInputValue(input);
	});
}

function handleBudgetInput(e) {
	formatBudgetInputValue(e.target);
}

// took some help from chatGPT to get this to work as I wanted
function formatBudgetInputValue(input) {
	let value = input.value.replace(/[^\d.]/g, ""); // Allow only numbers and dot
	let [whole, decimal] = value.split(".");

	if (whole.length > 8) {
		whole = whole.substring(0, 8);
	}
	whole = whole.replace(/\B(?=(\d{3})+(?!\d))/g, " ");
	if (decimal !== undefined) {
		decimal = decimal.replace(/\./g, "");
		value = `${whole}.${decimal}`;
	} else {
		value = whole;
	}
	input.value = value;
}

/*
 * ----------------------------------------------------------------------
 * Forms helper functions
 * ----------------------------------------------------------------------
 */
// Shows validation errors for a form
function displayValidationErrors(form, errors) {
	form.querySelectorAll(".field-error").forEach(el => el.textContent = "");
	Object.entries(errors).forEach(([field, msgs]) => {
		const el = form.querySelector(`[data-valmsg-for="${field}"]`);
		if (el) el.textContent = msgs[0];
	});
}

// Closes modal by id and reloads the page
function closeModalAndReload(modalId) {
	const modal = document.getElementById(modalId);
	if (modal) modal.classList.remove("show");
	location.reload();
}

// Generic AJAX form handler
function handleAjaxForm(form, { beforeSubmit, onSuccess, modalId }) {
	form.addEventListener("submit", async (e) => {
		e.preventDefault();
		if (beforeSubmit) beforeSubmit(form);
		const formData = new FormData(form);
		try {
			const response = await fetch(form.action, {
				method: "POST",
				body: formData,
				headers: { Accept: "application/json" }
			});
			const result = await response.json();

			if (result.success) {
				if (onSuccess) onSuccess(result);
				else if (modalId) closeModalAndReload(modalId);
			} else if (result.errors) {
				displayValidationErrors(form, result.errors);
			} else if (result.error) {
				console.error(result.error);
			}
		} catch (err) {
			console.error("An error occurred while submitting the form", err);
		}
	});
}

// Fills a select element 
function fillSelect(select, options, selectedValue) {
	select.innerHTML = "";
	options.forEach(opt => {
		const option = document.createElement("option");
		option.value = opt.value;
		option.textContent = opt.text;
		if (String(opt.value) === String(selectedValue)) option.selected = true;
		select.appendChild(option);
	});
}

// Fills input fields 
function fillInputs(form, data, fieldMap) {
	fieldMap.forEach(([inputName, dataKey]) => {
		const input = form.querySelector(`[name='${inputName}']`);
		if (input) input.value = data[dataKey] ?? "";
	});
}

// Generic deletion handler
function initDeletionHandler({ selector, urlPrefix, confirmMessage, errorMessage }) {
	document.querySelectorAll(selector).forEach((button) => {
		button.addEventListener("click", async (e) => {
			e.preventDefault();
			const id = button.getAttribute("data-id");
			if (!id) return;

			if (confirm(confirmMessage)) {
				try {
					const response = await fetch(`/${urlPrefix}/${id}`, {
						method: "DELETE",
					});
					const result = await response.json();

					if (result.success) {
						window.location.reload();
					} else {
						console.error("Delete failed:", result.error || errorMessage);
					}
				} catch (err) {
					console.error("Failed to delete item:", err);

				}
			}
		});
	});
}

/*
 * ----------------------------------------------------------------------
 * Project form / Add
 * ----------------------------------------------------------------------
 */
function initProjectForm() {
	const form = document.getElementById("addProjectForm");
	if (!form) return;

	handleAjaxForm(form, {
		beforeSubmit: (form) => {
			// Sync selected members to select
			let selectedMembers = [];
			if (window.getTagSelectorSelectedIds) {
				selectedMembers = window.getTagSelectorSelectedIds('selected-members-add');
			}
			const select = document.getElementById('SelectedMemberIdsAdd');
			select.innerHTML = '';
			selectedMembers.forEach(id => {
				const option = document.createElement('option');
				option.value = id;
				option.selected = true;
				select.appendChild(option);
			});
		},
		modalId: "addprojectmodal"
	});

	initWysiwygEditor(
		'#add-project-description-editor',
		'#add-project-description-toolbar',
		'#add-project-description',
		''
	);
	initTagSelector({
		containerId: 'selected-members-add',
		inputId: 'member-search-add',
		resultsId: 'search-results-add',
		searchUrl: (query) => '/projects/members/search?term=' + encodeURIComponent(query),
		displayProperty: 'fullName',
		imageProperty: 'imageUrl',
		tagClass: 'member-tag',
		emptyMessage: 'No members found.',
		preSelectedItems: [],
		selectedInputIds: 'SelectedMemberIdsAdd',
		allItems: []
	});
}

/*
 * ----------------------------------------------------------------------
 * Project form / Edit
 * ----------------------------------------------------------------------
 */
// took some help from chatGPT to get this to work 
window.addEventListener("openModal", function (e) {
	const { modalId, projectId } = e.detail || {};
	if (modalId !== "editprojectmodal" || !projectId)
		return;

	const form = document.querySelector("#editprojectmodal form");
	if (!form) {
		// console.error("Edit project form not found")
		return;
	}

	// fetch project data
	fetch(`/projects/${projectId}`)
		.then(response => response.json())
		.then(data => {
			if (!(data.success && data.project)) {
				// console.error("Failed to fetch project data.");
				return;
			}

			// fill select fields using helper
			fillSelect(form.querySelector("[name='ClientId']"), data.clients, data.project.clientId);
			fillSelect(form.querySelector("[name='StatusId']"), data.statuses, data.project.status?.id || data.project.statusId);

			// fill inputs using helper
			fillInputs(form, data.project, [
				["Id", "id"],
				["ProjectName", "projectName"],
				["StartDate", "startDate"],
				["EndDate", "endDate"],
				["Budget", "budget"]
			]);
			// format budget
			const budgetInput = form.querySelector("[name='Budget']");
			formatBudgetInputValue(budgetInput);

			// project image
			const imgPreview = form.querySelector(".image-preview-container img");
			const container = form.querySelector(".image-preview-container");
			if (data.project.imageUrl && imgPreview) {
				imgPreview.src = data.project.imageUrl;
				imgPreview.classList.remove("hide");
				updateImagePreviewState(container, true);
			} else if (imgPreview) {
				imgPreview.src = "";
				imgPreview.classList.add("hide");
				updateImagePreviewState(container, false);
			}

			// Quill Description
			if (form.querySelector("#edit-project-description-editor").__quill) {
				form.querySelector("#edit-project-description-editor").__quill = null;
			}
			const quillInstance = new Quill(form.querySelector("#edit-project-description-editor"), {
				modules: { toolbar: '#edit-project-description-toolbar' },
				theme: 'snow'
			});
			quillInstance.setContents([]);
			if (data.project.description) {
				quillInstance.clipboard.dangerouslyPasteHTML(data.project.description);
			}

			// member Selector
			const selectedMembersContainer = document.getElementById('selected-members-edit');
			if (selectedMembersContainer) selectedMembersContainer.innerHTML = '';
			const projectMembersSelect = document.getElementById('SelectedMemberIds');
			if (projectMembersSelect) projectMembersSelect.innerHTML = '';

			const preSelected = (data.project.allMembers || []).map(m => ({
				id: m.id,
				fullName: m.fullName || [m.firstName, m.lastName].filter(Boolean).join(" "),
				imageUrl: m.imageUrl
			}));

			initTagSelector({
				containerId: 'selected-members-edit',
				inputId: 'member-search-edit',
				resultsId: 'search-results-edit',
				searchUrl: (query) => '/projects/members/search?term=' + encodeURIComponent(query),
				displayProperty: 'fullName',
				imageProperty: 'imageUrl',
				tagClass: 'member-tag',
				emptyMessage: 'No members found.',
				preSelectedItems: preSelected,
				selectedInputIds: 'SelectedMemberIdsEdit',
				allItems: data.members
			});

			// Ajax form handler
			handleAjaxForm(form, {
				modalId: "editprojectmodal",
				beforeSubmit: (form) => {
					// sync Quill to textarea
					const quill = Quill.find(form.querySelector("#edit-project-description-editor"));
					if (quill) {
						form.querySelector("[name='Description']").value = quill.root.innerHTML;
					}
					// Sync tag-selector to select
					let selectedMembers;
					if (window.getTagSelectorSelectedIds) {
						selectedMembers = window.getTagSelectorSelectedIds('selected-members-edit');
					} else {
						selectedMembers = Array.from(document.getElementById('SelectedMemberIds').options)
							.filter(opt => opt.selected)
							.map(opt => opt.value);
					}
					const select = document.getElementById('SelectedMemberIdsEdit');
					select.innerHTML = '';
					selectedMembers.forEach(id => {
						const option = document.createElement('option');
						option.value = id;
						option.selected = true;
						select.appendChild(option);
					});
				},
				onSuccess: () => {
					WindowManager.closeModal("editprojectmodal");
					location.reload();
				}
			});
		})
		.catch(err => {
			console.error("Error fetching project data:", err);
		});
});

/*
 * ----------------------------------------------------------------------
 * Project deletion
 * ----------------------------------------------------------------------
 */
function initProjectDeletion() {
	initDeletionHandler({
		selector: '[data-action="delete-project"]',
		urlPrefix: 'projects',
		confirmMessage: 'Are you sure you want to delete this project?',
		errorMessage: 'Failed to delete project'
	});
}

/*
 * ----------------------------------------------------------------------
 * Client form / Add
 * ----------------------------------------------------------------------
 */
function initAddClientForm() {
	const form = document.getElementById("addClientForm");
	if (!form) return;

	handleAjaxForm(form, {
		modalId: "addclientmodal"
	});
}

/*
 * ----------------------------------------------------------------------
 * Client form / Edit
 * ----------------------------------------------------------------------
 */
window.addEventListener("openModal", function (e) {
	const { modalId, clientId } = e.detail || {};
	if (modalId !== "editclientmodal" || !clientId) return;

	const form = document.querySelector("#editclientmodal form");
	if (!form) {
		// console.error("Edit client form not found");
		return;
	}

	fetch(`/clients/${clientId}`)
		.then(response => response.json())
		.then(data => {
			if (!(data.success && data.client)) {
				// console.error("Failed to fetch client data.");
				return;
			}

			const client = data.client;

			// fill inputs using helper
			fillInputs(form, client, [
				['Id', 'id'],
				['ClientName', 'clientName'],
				['FirstName', 'firstName'],
				['LastName', 'lastName'],
				['Email', 'email'],
				['PhoneNumber', 'phoneNumber'],
				['StreetAddress', 'streetAddress'],
				['PostalCode', 'postalCode'],
				['City', 'city'],
				['ContactPerson', 'contactPerson']
			]);

			// image preview and container
			const imgPreview = form.querySelector(".image-preview-container img");
			const container = form.querySelector(".image-preview-container");
			if (imgPreview) {
				if (client.imageUrl) {
					imgPreview.src = client.imageUrl;
					imgPreview.classList.remove("hide");
					if (typeof updateImagePreviewState === "function" && container) {
						updateImagePreviewState(container, true);
					}
				} else {
					imgPreview.src = "";
					imgPreview.classList.add("hide");
					if (typeof updateImagePreviewState === "function" && container) {
						updateImagePreviewState(container, false);
					}
				}
			}

			// reset file input
			const fileInput = form.querySelector("input[type='file'][name='ImageFile']");
			if (fileInput) fileInput.value = "";

			// reset validation errors
			form.querySelectorAll(".field-error").forEach(el => { el.textContent = ""; });

			// Ajax form handler
			handleAjaxForm(form, {
				modalId: "editclientmodal",
				onSuccess: () => {
					WindowManager.closeModal("editclientmodal");
					location.reload();
				}
			});
		})
		.catch(err => {
				console.error("Error fetching client data:", err);
			}
		);
});

/*
 * ----------------------------------------------------------------------
 * Client list
 * ----------------------------------------------------------------------
 */
function initClientList() {
	const selectAllCheckbox = document.querySelector("#selectAllClients");
	const selectedInfo = document.querySelector(".selected-clients-info");
	const selectedCount = document.querySelector("#selectedClientsCount");
	const unselectAllButton = document.querySelector(".unselect-all");
	let selectedClients = [];

	// manage "select all" checkbox
	if (selectAllCheckbox) {
		selectAllCheckbox.addEventListener("change", () => {
			const checkboxes = document.querySelectorAll(".client-checkbox");
			checkboxes.forEach((checkbox) => {
				checkbox.checked = selectAllCheckbox.checked;
				const clientId = checkbox.getAttribute("data-id");
				if (selectAllCheckbox.checked) {
					if (!selectedClients.includes(clientId)) {
						selectedClients.push(clientId);
					}
				} else {
					selectedClients = selectedClients.filter((id) => id !== clientId);
				}
			});
			updateSelectedClientsInfo();
		});
	}

	// manage individual checkboxes
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

	// manage "unselect all" button
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

	// update selected clients info
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
}

/*
 * ----------------------------------------------------------------------
 * Client deletion
 * ----------------------------------------------------------------------
 */
function initClientDeletion() {
	initDeletionHandler({
		selector: '[data-action="delete-client"]',
		urlPrefix: 'clients',
		confirmMessage: 'Are you sure you want to delete this client?',
		errorMessage: 'Failed to delete client'
	});
}

/*
 * ----------------------------------------------------------------------
 * Member form / Add
 * ----------------------------------------------------------------------
 */
function initMemberForm() {
	const form = document.getElementById("addMemberForm");
	if (!form) return;

	handleAjaxForm(form, {
		modalId: "addmembermodal"
	});
}

/*
 * ----------------------------------------------------------------------
 * Member form / Edit
 * ----------------------------------------------------------------------
 */
window.addEventListener("openModal", function (e) {
	const { modalId, memberId } = e.detail || {};
	if (modalId !== "editmembermodal" || !memberId) return;

	const form = document.querySelector("#editmembermodal form");
	if (!form) {
		// console.error("Edit member form not found");
		return;
	}

	fetch(`/members/${memberId}`)
		.then(response => response.json())
		.then(data => {
			if (!(data.success && data.member)) {
				// console.error("Failed to fetch member data.");
				return;
			}

			const member = data.member;

			// fill inputs using helper
			fillInputs(form, member, [
				['Id', 'id'],
				['FirstName', 'firstName'],
				['LastName', 'lastName'],
				['Email', 'email'],
				['PhoneNumber', 'phoneNumber'],
				['JobTitle', 'jobTitle'],
				['StreetAddress', 'streetAddress'],
				['PostalCode', 'postalCode'],
				['City', 'city']
			]);

			// image preview and container
			const imgPreview = form.querySelector(".image-preview-container img");
			const container = form.querySelector(".image-preview-container");
			if (imgPreview) {
				if (member.imageUrl) {
					imgPreview.src = member.imageUrl;
					imgPreview.classList.remove("hide");
					if (typeof updateImagePreviewState === "function" && container) {
						updateImagePreviewState(container, true);
					}
				} else {
					imgPreview.src = "";
					imgPreview.classList.add("hide");
					if (typeof updateImagePreviewState === "function" && container) {
						updateImagePreviewState(container, false);
					}
				}
			}

			// reset file input
			const fileInput = form.querySelector("input[type='file'][name='ImageFile']");
			if (fileInput) fileInput.value = "";

			// reset validation errors
			form.querySelectorAll(".field-error").forEach(el => { el.textContent = ""; });

			// Ajax form handler
			handleAjaxForm(form, {
				modalId: "editmembermodal",
				onSuccess: () => {
					WindowManager.closeModal("editmembermodal");
					location.reload();
				}
			});
		})
		.catch(err => {
				console.error("Error fetching member data:", err);
			}
		);
});

/*
 * ----------------------------------------------------------------------
 * Member deletion
 * ----------------------------------------------------------------------
 */
function initMemberDeletion() {
	initDeletionHandler({
		selector: '[data-action="delete-member"]',
		urlPrefix: 'members',
		confirmMessage: 'Are you sure you want to delete this member?',
		errorMessage: 'Failed to delete member'
	});
}

/*
 * ----------------------------------------------------------------------
 * Toggle admin role - not working yet...
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
			console.log("Admin status updated:", data.message);
		} else {
			console.error("Failed to update admin status:", data.message || data.error);
		}
	} catch (err) {
		console.error("Error toggling admin role:", err);
	}
}

/*
 * ----------------------------------------------------------------------
 * toggle-password eye
 * ----------------------------------------------------------------------
 */
function initPasswordToggle() {
	document.querySelectorAll('.password-toggle').forEach(function (el) {
		el.addEventListener('click', function () {
			const input = el.closest('.password-group').querySelector('input[type="password"], input[type="text"]');
			if (!input) return;
			if (input.type === "password") {
				input.type = "text";
				el.innerHTML = '<i class="fa-sharp-duotone fa-solid fa-eye"></i>';
			} else {
				input.type = "password";
				el.innerHTML = '<i class="fa-sharp-duotone fa-solid fa-eye-slash"></i>';
			}
		});
	});
}