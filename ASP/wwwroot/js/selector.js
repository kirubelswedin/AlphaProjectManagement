/**
 * Handles client and member selection functionality
 */

class SelectorManager {
	constructor() {
		this.init();
	}

	init() {
		this.initSelectors();
		this.initClickOutside();
	}

	initSelectors() {
		document.querySelectorAll(".selector-field").forEach((field) => {
			const select = field.querySelector("select");
			const container = field.querySelector(".selected-items-list");
			const searchInput = field.querySelector(".search-input input");
			const hiddenInput = field.querySelector('input[type="hidden"]');
			const optionsDropdown = field.querySelector(".options-dropdown");

			if (!select || !container || !searchInput || !optionsDropdown) return;

			// Initial setup
			this.updateSelected(select, container, hiddenInput);

			// Search input focus event
			searchInput.addEventListener("focus", (e) => {
				e.stopPropagation();
				this.showOptions(select, optionsDropdown, searchInput.value);
			});

			// Search input event
			searchInput.addEventListener("input", (e) => {
				e.stopPropagation();
				this.showOptions(select, optionsDropdown, searchInput.value);
			});

			// Handle option selection
			optionsDropdown.addEventListener("click", (e) => {
				e.stopPropagation();
				const optionItem = e.target.closest(".option-item");
				if (!optionItem) return;

				const value = optionItem.dataset.value;
				const option = select.querySelector(`option[value="${value}"]`);

				if (!option) return;

				if (select.multiple) {
					// Toggle selection for multiple select
					option.selected = !option.selected;
					optionItem.classList.toggle("selected");
				} else {
					// Single select behavior
					Array.from(select.options).forEach((opt) => {
						opt.selected = opt.value === value;
					});

					// Close dropdown for single select
					optionsDropdown.style.display = "none";
					searchInput.value = "";
				}

				// Update the visual display and hidden input
				this.updateSelected(select, container, hiddenInput);
			});

			// Prevent clicks inside selector from bubbling
			field.addEventListener("click", (e) => {
				if (!e.target.closest(".remove-btn")) {
					e.stopPropagation();
				}
			});
		});
	}

	initClickOutside() {
		document.addEventListener("click", (e) => {
			if (e.target.closest(".selector-field")) {
				return;
			}

			document.querySelectorAll(".options-dropdown").forEach((dropdown) => {
				dropdown.style.display = "none";
			});

			document.querySelectorAll(".search-input input").forEach((input) => {
				input.value = "";
			});
		});
	}

	closeAllDropdowns() {
		document.querySelectorAll(".selector-field").forEach((field) => {
			field.classList.remove("active");
			const dropdown = field.querySelector(".options-dropdown");
			if (dropdown) {
				dropdown.style.display = "none";
			}
		});
	}

	showOptions(select, optionsDropdown, searchText) {
		if (!optionsDropdown) return;

		const options = Array.from(select.options).filter((opt) => opt.value);
		optionsDropdown.innerHTML = "";

		options.forEach((option) => {
			if (
				!searchText ||
				option.text.toLowerCase().includes(searchText.toLowerCase())
			) {
				const item = document.createElement("div");
				item.className = "option-item";
				item.dataset.value = option.value;

				if (select.multiple && option.selected) {
					item.classList.add("selected");
				}

				const image =
					option.dataset.image ||
					(select.id === "projectClientSelect"
						? "/images/project/Image-1.svg"
						: "/images/avatars/Avatar-1.svg");

				item.innerHTML = `
					<img src="${image}" alt="${option.text}" />
					<span>${option.text}</span>
					${select.multiple ? '<i class="fa-solid fa-check"></i>' : ""}
				`;

				optionsDropdown.appendChild(item);
			}
		});

		optionsDropdown.style.display = options.length ? "block" : "none";
	}

	updateSelected(select, selectedDisplay, hiddenInput) {
		if (!selectedDisplay) return;

		const selectedOptions = Array.from(select.selectedOptions);
		selectedDisplay.innerHTML = "";

		// Show placeholder if nothing selected
		if (selectedOptions.length === 0) {
			selectedDisplay.innerHTML = `<div class="selected-item placeholder">Select ${
				select.multiple ? "members" : "a client"
			}</div>`;
			if (hiddenInput) {
				hiddenInput.value = "";
			}
			return;
		}

		// Create a display item for each selected option
		selectedOptions.forEach((option) => {
			const item = document.createElement("div");
			item.className = "selected-item";

			const image =
				option.dataset.image ||
				(select.id === "projectClientSelect"
					? "/images/project/Image-1.svg"
					: "/images/avatars/Avatar-1.svg");

			// Create HTML for the selected item
			item.innerHTML = `
				<img src="${image}" alt="${option.text}" />
				<span>${option.text}</span>
				${
					select.multiple
						? '<button type="button" class="remove-selected" data-value="' +
						  option.value +
						  '"><i class="fa-solid fa-xmark"></i></button>'
						: ""
				}
			`;

			// Add event listener for remove button (if multiple)
			if (select.multiple) {
				const removeBtn = item.querySelector(".remove-selected");
				removeBtn.addEventListener("click", (e) => {
					e.stopPropagation();
					const value = e.currentTarget.dataset.value;
					const option = select.querySelector(`option[value="${value}"]`);
					if (option) {
						option.selected = false;
						this.updateSelected(select, selectedDisplay, hiddenInput);

						// Update dropdown item if visible
						const dropdown = selectedDisplay
							.closest(".selector-field")
							.querySelector(".options-dropdown");
						if (dropdown && dropdown.style.display === "block") {
							const dropdownItem = dropdown.querySelector(`[data-value="${value}"]`);
							if (dropdownItem) {
								dropdownItem.classList.remove("selected");
							}
						}
					}
				});
			}

			selectedDisplay.appendChild(item);
		});

		// Update hidden input with selected values
		if (hiddenInput) {
			hiddenInput.value = selectedOptions.map((opt) => opt.value).join(",");
		}
	}

	handleOptionClick(option) {
		const field = option.closest(".selector-field");
		const select = field.querySelector("select");
		const value = option.dataset.value;
		const label = option.textContent;
		const image = option.dataset.image;

		// Uppdatera valt värde
		select.value = value;

		// Uppdatera visad text och bild
		const selectedText = field.querySelector(".selected-text");
		const selectedImage = field.querySelector(".selected-image");

		if (selectedText) selectedText.textContent = label;
		if (selectedImage && image) {
			selectedImage.src = image;
			selectedImage.style.display = "block";
		}

		// Stäng dropdown
		this.closeAllDropdowns();
	}
}

export default new SelectorManager();
