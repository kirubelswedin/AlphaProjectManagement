document.addEventListener("DOMContentLoaded", () => {
	initSidebarState();
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
 * Set sidebar state
 * ----------------------------------------------------------------------
 */
function initSidebarState() {
	const savedWidth = localStorage.getItem("sidebar-width");
	if (savedWidth) {
		const width = parseFloat(savedWidth);
		const minWidth = 4;
		
		// Set sidebar width
		document.documentElement.style.setProperty("--sidebar-width", `${width}rem`);
		
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
 * WYSIWYG Editor functionality
 * ----------------------------------------------------------------------
 */
function initWysiwygEditor(
	editorSelector,
	toolbarSelector,
	textareaSelector,
	content = ""
) {
	const editor = document.querySelector(editorSelector);
	const toolbar = document.querySelector(toolbarSelector);
	const textarea = document.querySelector(textareaSelector);

	if (!editor || !toolbar || !textarea) return;

	const quill = new Quill(editor, {
		theme: "snow",
		modules: {
			syntax: true,
			toolbar: toolbar,
		},
		placeholder: "Type something",
	});

	if (content) {
		quill.root.innerHTML = content;
	}

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
function updateImagePreviewState(container, hasImage) {
  if (hasImage) {
    container.classList.add("has-image");
  } else {
    container.classList.remove("has-image");
  }
}

function initFileUploads() {
  document.querySelectorAll("[data-file-upload]").forEach((container) => {
    const input = container.querySelector('input[type="file"]');
    const preview = container.querySelector("img");
    container.addEventListener("click", (e) => {
      if (e.target !== input) {
        input?.click();
      }
    });
    input?.addEventListener("change", (e) => {
      const file = e.target.files[0];
      if (file && file.type.startsWith("image/")) {
        const reader = new FileReader();
        reader.onload = () => {
          preview.src = reader.result;
          preview.classList.remove("hide");
          updateImagePreviewState(container, true);
        };
        reader.readAsDataURL(file);
      } else {
        updateImagePreviewState(container, false);
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

	// Initial overflow check
	const hasOverflow = tabsContainer.scrollWidth > tabsContainer.clientWidth;
	tabsContainer.classList.toggle("has-overflow", hasOverflow);
}

/*
 * ----------------------------------------------------------------------
 * Tab Filter
 * ----------------------------------------------------------------------
 */
function initTabFilter() {
	const tabsContainer = document.querySelector(".projects-tabs");
	if (!tabsContainer) return;

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
				const response = await fetch(`/projects/list?tab=${status}`);
				const html = await response.text();

				const projectsGrid = document.querySelector(".projects-grid");
				if (projectsGrid) {
					projectsGrid.innerHTML = html;
				}
			} catch (error) {
				console.error("Error fetching filtered projects:", error);
				showToast("error", "Failed to load projects");
			}
		});
	});
}


/*
* ----------------------------------------------------------------------
* Avatar visibility
* ----------------------------------------------------------------------
*/
function initAvatarVisibility() {
	function updateAvatarVisibility() {
		document.querySelectorAll('.team-members').forEach(container => {

			const avatars = Array.from(container.querySelectorAll('.avatar:not(.more-members)'));
			const moreDiv = container.querySelector('.more-members');

			if (!avatars.length) {
				if (moreDiv) moreDiv.style.display = 'none';
				return;
			}

			// reset all avatars to visible
			avatars.forEach(a => a.style.display = '');
			if (moreDiv) moreDiv.style.display = 'none';

			// calculate the maximum number of avatars that can fit
			const containerWidth = container.offsetWidth;
			const avatarWidth = avatars[0].offsetWidth;
			const moreWidth = moreDiv ? moreDiv.offsetWidth : 0;
			const overlapPerAvatar = 8; 

			// safety margin for the counter
			const safetyMargin = 20;

			// reserve space for the counter
			const spaceNeededForCounter = moreWidth + safetyMargin;

			// calculate how many avatars can fit
			const availableWidth = containerWidth - spaceNeededForCounter;
			const effectiveAvatarWidth = avatarWidth - overlapPerAvatar; // account for overlap

			// calculate the maximum number of avatars that can fit
			let maxFit = Math.floor(availableWidth / effectiveAvatarWidth) - 1;
			if (maxFit < 1) maxFit = 1;
			if (maxFit > 8) maxFit = 8;

			// check if all avatars fit
			const totalWidthNeeded = avatars.length * effectiveAvatarWidth + overlapPerAvatar;
			if (totalWidthNeeded <= containerWidth && avatars.length <= 8) {
				avatars.forEach(a => a.style.display = '');
				if (moreDiv) moreDiv.style.display = 'none';
				return;
			}

			// otherwise, hide the excess avatars
			avatars.forEach((a, i) => {
				a.style.display = i < maxFit ? '' : 'none';
			});

			// show counter with the number of hidden avatars
			const hiddenCount = avatars.length - maxFit;
			if (moreDiv) {
				if (hiddenCount > 0) {
					moreDiv.style.display = '';
					moreDiv.querySelector('span').textContent = `+${hiddenCount}`;
				} else {
					moreDiv.style.display = 'none';
				}
			}
		});
	}

	// run directly on resize
	updateAvatarVisibility();
	window.addEventListener('resize', updateAvatarVisibility);

	// run after a short delay to allow for any layout changes
	setTimeout(updateAvatarVisibility, 100);
}

/*
 * ----------------------------------------------------------------------
 * Project form / Add
 * ----------------------------------------------------------------------
 */
function initProjectForm() {
	const addProjectForm = document.getElementById("addProjectForm");
	
	if (addProjectForm) {
		addProjectForm.addEventListener("submit", async (e) => {
			e.preventDefault();
			console.log("ProjectForm submitted...");

			// make sure all selected members is in the select
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

			const formData = new FormData(addProjectForm);

			try {
				const response = await fetch(addProjectForm.action, {
					method: "POST",
					body: formData,
					headers: { Accept: "application/json" }
				});

				const result = await response.json();

				if (result.success) {
					const modal = document.getElementById("addprojectmodal");
					if (modal) {
						modal.classList.remove("show");
					}
					location.reload();
				} else {
					if (result.errors) {
						addProjectForm
							.querySelectorAll(".field-error")
							.forEach((el) => (el.textContent = ""));
						Object.entries(result.errors).forEach(([field, errors]) => {
							const errorElement = addProjectForm.querySelector(
								`[data-valmsg-for="${field}"]`
							);
							if (errorElement) {
								errorElement.textContent = errors[0];
							}
						});
					} else if (result.error) {
						console.error(result.error);
					}
				}
			} catch (error) {
				console.error("Error:", error);
				console.error("An error occurred while submitting the form");
			}
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
}

/*
 * ----------------------------------------------------------------------
 * Project form / Edit
 * ----------------------------------------------------------------------
 */
window.addEventListener("openModal", function (e) {
	const { modalId, projectId } = e.detail || {};
	if (modalId === "editprojectmodal" && projectId) {
		const form = document.querySelector("#editprojectmodal form");
		if (!form) {
			console.error("Edit form not found in DOM");
			return;
		}

		fetch(`/projects/${projectId}`)
			.then((response) => response.json())
			.then((data) => {
				if (!(data.success && data.project)) {
					console.error("Failed to fetch project data.");
					return;
				}

				// fill clients
				const clientSelect = form.querySelector("[name='ClientId']");
				if (clientSelect && data.clients) {
					clientSelect.innerHTML = "";
					data.clients.forEach(c => {
						const option = document.createElement("option");
						option.value = c.value;
						option.textContent = c.text;
						if (c.value === data.project.clientId) option.selected = true;
						clientSelect.appendChild(option);
					});
				}

				// fill statuses
				const statusSelect = form.querySelector("[name='StatusId']");
				if (statusSelect && data.statuses) {
					statusSelect.innerHTML = "";
					data.statuses.forEach(s => {
						const option = document.createElement("option");
						option.value = s.value;
						option.textContent = s.text;
						if (String(s.value) === String(data.project.status?.id || data.project.statusId)) option.selected = true;
						statusSelect.appendChild(option);
					});
				}

				// Project fields
				form.querySelector("[name='Id']").value = data.project.id ?? "";
				form.querySelector("[name='ProjectName']").value = data.project.projectName ?? "";
				form.querySelector("[name='StartDate']").value = data.project.startDate ? data.project.startDate.split("T")[0] : "";
				form.querySelector("[name='EndDate']").value = data.project.endDate ? data.project.endDate.split("T")[0] : "";
				form.querySelector("[name='Budget']").value = data.project.budget ?? "";

				// Project image
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

				// Member Selector
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

				// Ajax submit handler
				form.onsubmit = async function (e) {
					e.preventDefault();

					// sync Quill to textarea
					const quill = Quill.find(form.querySelector("#edit-project-description-editor"));
					if (quill) {
						form.querySelector("[name='Description']").value = quill.root.innerHTML;
					}

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

					const formData = new FormData(form);

					try {
						const response = await fetch('/projects/EditProject', {
							method: 'POST',
							body: formData
						});
						const result = await response.json();
						if (result.success) {
							WindowManager.closeModal("editprojectmodal");
							location.reload();
						} else if (result.errors) {
							for (const [field, errors] of Object.entries(result.errors)) {
								const errorElem = form.querySelector(`.field-error[data-valmsg-for='${field}']`);
								if (errorElem) errorElem.textContent = errors.join(", ");
							}
						} else if (result.error) {
							alert(result.error);
						}
					} catch (err) {
						console.error("Error submitting edit client form:", err);
					}
				};
			})
			.catch(error => {
				console.error("Error fetching client data:", error);
			});
	}
});

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
				if (!id) return;

				if (confirm("Are you sure you want to delete this project?")) {
					try {
						const response = await fetch(`/projects/${id}`, {
							method: "DELETE",
						});

						const result = await response.json();

						if (result.success) {
							window.location.reload();
						} else {
							showToast("error", result.error || "Failed to delete project");
						}
					} catch (error) {
						console.error("Error deleting project:", error);
						showToast("error", "An error occurred while deleting the project");
					}
				}
			});
		});
}

/*
 * ----------------------------------------------------------------------
 * Client form / Add
 * ----------------------------------------------------------------------
 */
function initAddClientForm() {
	const addClientForm = document.getElementById("addClientForm");

	if (addClientForm) {
		addClientForm.addEventListener("submit", async (e) => {
			e.preventDefault();
			console.log("ClientForm submitted...");
			
			const formData = new FormData(addClientForm);
	
			try {
				const response = await fetch(addClientForm.action, {
					method: "POST",
					body: formData,
					headers: {
						Accept: "application/json",
					},
				});
	
				const result = await response.json();
	
				if (result.success) {
					// Close modal and reload page
					const modal = document.getElementById("addclientmodal");
					if (modal) {
						modal.classList.remove("show");
					}
					location.reload();
				} else {
					// Handle validation errors
					if (result.errors) {
						// Clear previous errors
						document
							.querySelectorAll(".field-error")
							.forEach((el) => (el.textContent = ""));
						// Display new errors
						Object.entries(result.errors).forEach(([field, errors]) => {
							const errorElement = document.querySelector(
								`[data-valmsg-for="${field}"]`
							);
							if (errorElement) {
								errorElement.textContent = errors[0];
							}
						});
					} else if (result.error) {
						console.error(result.error);
					}
				}
			} catch (error) {
				console.error("Error:", error);
				console.error("An error occurred while submitting the form");
			}
		});
	}
}

/*
 * ----------------------------------------------------------------------
 * Client form / Edit
 * ----------------------------------------------------------------------
 */
window.addEventListener("openModal", function (e) {
	const { modalId, clientId } = e.detail || {};
	if (modalId === "editclientmodal" && clientId) {
		const form = document.querySelector("#editclientmodal form");
		if (!form) {
			console.error("Edit client form not found in DOM");
			return;
		}

		fetch(`/clients/${clientId}`)
			.then(response => response.json())
			.then(data => {
				console.log('Clientdata:', data);

				if (!(data.success && data.client)) {
					console.error("Failed to fetch client data.");
					return;
				}

				const client = data.client;

				// fill the contact person field
				const firstName = form.querySelector("[name='FirstName']").value.trim();
				const lastName = form.querySelector("[name='LastName']").value.trim();
				const contactPersonInput = form.querySelector("[name='ContactPerson']");
				if (contactPersonInput) {
					contactPersonInput.value = (firstName + " " + lastName).trim();
				}

				// Fill in the rest of the fields
				const fields = [
					['Id', 'id'],
					['ClientName', 'clientName'],
					['FirstName', 'firstName'],
					['LastName', 'lastName'],
					['Email', 'email'],
					['PhoneNumber', 'phoneNumber'],
					['StreetAddress', 'streetAddress'],
					['PostalCode', 'postalCode'],
					['City', 'city']
				];
				fields.forEach(([inputName, clientKey]) => {
					const input = form.querySelector(`[name='${inputName}']`);
					if (input && input.value !== (client[clientKey] ?? "")) {
						input.value = client[clientKey] ?? "";
					}
				});

				// Image preview and container
				const imgPreview = form.querySelector(".image-preview-container img");
				const container = form.querySelector(".image-preview-container");
				if (imgPreview) {
					if (client.imageUrl) {
						if (imgPreview.src !== client.imageUrl) imgPreview.src = client.imageUrl;
						imgPreview.classList.remove("hide");
						if (typeof updateImagePreviewState === "function" && container) {
							updateImagePreviewState(container, true);
						}
					} else {
						if (imgPreview.src !== "") imgPreview.src = "";
						imgPreview.classList.add("hide");
						if (typeof updateImagePreviewState === "function" && container) {
							updateImagePreviewState(container, false);
						}
					}
				}

				// Reset file input
				const fileInput = form.querySelector("input[type='file'][name='ImageFile']");
				if (fileInput) fileInput.value = "";

				// Reset validation errors
				form.querySelectorAll(".field-error").forEach(el => { if (el.textContent !== "") el.textContent = ""; });

				// Ajax submit handler
				form.onsubmit = async function (ev) {
					ev.preventDefault();
					
					const formData = new FormData(form);
					// const clientId = formData.get("Id");
					try {
						const response = await fetch(`/clients/EditClient`, {
							method: "POST",
							body: formData
						});
						const result = await response.json();
						if (result.success) {
							WindowManager.closeModal("editclientmodal");
							location.reload();
						} else if (result.errors) {
							for (const [field, errors] of Object.entries(result.errors)) {
								const errorElem = form.querySelector(`.field-error[data-valmsg-for='${field}']`);
								if (errorElem) errorElem.textContent = errors.join(", ");
							}
						} else if (result.error) {
							alert(result.error);
						}
					} catch (err) {
						console.error("Error submitting edit client form:", err);
					}
				};
			})
			.catch(error => {
				console.error("Error fetching client data:", error);
			});
	}
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
	document
		.querySelectorAll('[data-action="delete-client"]')
		.forEach((button) => {
			button.addEventListener("click", async function (e) {
				e.preventDefault();
				const id = this.getAttribute("data-id");
				if (!id) return;

				if (confirm("Are you sure you want to delete this client?")) {
					try {
						const response = await fetch(`/clients/${id}`, {
							method: "DELETE",
						});

						const result = await response.json();

						if (result.success) {
							window.location.reload();
						} else {
							showToast("error", result.error || "Failed to delete client");
						}
					} catch (error) {
						console.error("Error deleting client:", error);
						showToast("error", "An error occurred while deleting the client");
					}
				}
			});
		});
}

/*
 * ----------------------------------------------------------------------
 * Member form / Add
 * ----------------------------------------------------------------------
 */
function initMemberForm() {
	const addMemberForm = document.getElementById("addMemberForm");
	
	if (addMemberForm) {
		addMemberForm.addEventListener("submit", async (e) => {
			e.preventDefault();
			console.log("MemberForm submitted...");
	
			const formData = new FormData(addMemberForm);
	
			try {
				const response = await fetch(addMemberForm.action, {
					method: "POST",
					body: formData,
					headers: {
						Accept: "application/json",
					},
				});
	
				const result = await response.json();
	
				if (result.success) {
					const modal = document.getElementById("addmembermodal");
					if (modal) {
						modal.classList.remove("show");
					}
					window.location.reload();
				} else {
					if (result.errors) {
						document
							.querySelectorAll(".field-error")
							.forEach((el) => (el.textContent = ""));
						Object.entries(result.errors).forEach(([field, errors]) => {
							const errorElement = document.querySelector(
								`[data-valmsg-for="${field}"]`
							);
							if (errorElement) {
								errorElement.textContent = errors[0];
							}
						});
					} else if (result.error) {
						console.error(result.error);
					}
				}
			} catch (error) {
				console.error("Error:", error);
				console.error("An error occurred while submitting the form");
			}
		});
	}
}

/*
 * ----------------------------------------------------------------------
 * Member form / Edit
 * ----------------------------------------------------------------------
 */
window.addEventListener("openModal", function (e) {
	const { modalId, memberId } = e.detail || {};
	if (modalId === "editmembermodal" && memberId) {
		const form = document.querySelector("#editmembermodal form");
		if (!form) {
			console.error("Edit member form not found in DOM");
			return;
		}

		fetch(`/members/${memberId}`)
			.then(response => response.json())
			.then(data => {
				if (!(data.success && data.member)) {
					console.error("Failed to fetch member data.");
					return;
				}

				const member = data.member;

				// Fill in the fields
				const fields = [
					['Id', 'id'],
					['FirstName', 'firstName'],
					['LastName', 'lastName'],
					['Email', 'email'],
					['PhoneNumber', 'phoneNumber'],
					['JobTitle', 'jobTitle'],
					['StreetAddress', 'streetAddress'],
					['PostalCode', 'postalCode'],
					['City', 'city']
				];
				fields.forEach(([inputName, memberKey]) => {
					const input = form.querySelector(`[name='${inputName}']`);
					if (input && input.value !== (member[memberKey] ?? "")) {
						input.value = member[memberKey] ?? "";
					}
				});

				// Image preview and container
				const imgPreview = form.querySelector(".image-preview-container img");
				const container = form.querySelector(".image-preview-container");
				if (imgPreview) {
					if (member.imageUrl) {
						if (imgPreview.src !== member.imageUrl) imgPreview.src = member.imageUrl;
						imgPreview.classList.remove("hide");
						if (typeof updateImagePreviewState === "function" && container) {
							updateImagePreviewState(container, true);
						}
					} else {
						if (imgPreview.src !== "") imgPreview.src = "";
						imgPreview.classList.add("hide");
						if (typeof updateImagePreviewState === "function" && container) {
							updateImagePreviewState(container, false);
						}
					}
				}

				// Reset file input
				const fileInput = form.querySelector("input[type='file'][name='ImageFile']");
				if (fileInput) fileInput.value = "";

				// Reset validation errors
				form.querySelectorAll(".field-error").forEach(el => { if (el.textContent !== "") el.textContent = ""; });

				// Ajax submit handler
				form.onsubmit = async function (ev) {
					ev.preventDefault();

					const formData = new FormData(form);
					try {
						const response = await fetch(`/users/EditMember`, {
							method: "POST",
							body: formData
						});
						const result = await response.json();
						if (result.success) {
							WindowManager.closeModal("editmembermodal");
							location.reload();
						} else if (result.errors) {
							for (const [field, errors] of Object.entries(result.errors)) {
								const errorElem = form.querySelector(`.field-error[data-valmsg-for='${field}']`);
								if (errorElem) errorElem.textContent = errors.join(", ");
							}
						} else if (result.error) {
							alert(result.error);
						}
					} catch (err) {
						console.error("Error submitting edit member form:", err);
					}
				};
			})
			.catch(error => {
				console.error("Error fetching member data:", error);
			});
	}
});

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

						const result = await response.json();

						if (result.success) {
							window.location.reload();
						} else {
							showToast("error", result.error || "Failed to delete member");
						}
					} catch (error) {
						console.error("Error deleting member:", error);
						showToast("error", "An error occurred while deleting the member");
					}
				}
			});
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