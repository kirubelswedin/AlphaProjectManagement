
// tag selector for selecting multiple items ex. members.
// searching, keyboard navigation, tag removal
function initTagSelector(config) {
	let activeIndex = -1;
	let selectedIds = [];

	const container = document.getElementById(config.containerId);
	const searchInput = document.getElementById(config.inputId);
	const searchResults = document.getElementById(config.resultsId);
	const selectElement = document.getElementById(config.selectedInputIds);

	if (!selectElement) return;

	// Clear previous tags and selected state
	container.innerHTML = '';
	selectElement.innerHTML = '';
	selectedIds = [];

	// Add any pre-selected items as tags
	if (Array.isArray(config.preSelectedItems)) {
		config.preSelectedItems.forEach((item) => addTag(item));
	}

	searchInput.addEventListener("focus", () => {
		searchResults.classList.add("show");
	});

	searchInput.addEventListener("blur", () => {
		setTimeout(() => {
			searchResults.classList.remove("show");
		}, 200);
	});

	// Handle input, fetch and render search results
	searchInput.addEventListener('input', () => {
		const query = searchInput.value.trim();
		activeIndex = -1;

		if (query.length === 0) {
			searchResults.style.display = 'none';
			searchResults.innerHTML = '';
			return;
		}

		fetch(config.searchUrl(query))
			.then(r => r.json())
			.then(data => {
				renderSearchResults(data);
			});
	});

	// Keyboard navigation in search results
	searchInput.addEventListener("keydown", (e) => {
		const items = searchResults.querySelectorAll(".search-item");

		switch (e.key) {
			case "ArrowDown":
				e.preventDefault();
				if (items.length > 0) {
					activeIndex = (activeIndex + 1) % items.length;
					updateActiveItem(items);
				}
				break;
			case "ArrowUp":
				e.preventDefault();
				if (items.length > 0) {
					activeIndex = (activeIndex - 1 + items.length) % items.length;
					updateActiveItem(items);
				}
				break;
			case "Enter":
				e.preventDefault();
				if (activeIndex >= 0 && items[activeIndex]) {
					items[activeIndex].click();
				}
				break;
			case "Backspace":
				if (searchInput.value === "") {
					removeLastTag();
				}
				break;
		}
	});
	
	function updateActiveItem(items) {
		items.forEach((item) => item.classList.remove("active"));
		if (items[activeIndex]) {
			items[activeIndex].classList.add("active");
			items[activeIndex].scrollIntoView({ block: "nearest" });
		}
	}

	// Renders search results in the dropdown
	function renderSearchResults(data) {
		searchResults.innerHTML = "";

		if (!data || data.length === 0) {
			const noResult = document.createElement("div");
			noResult.classList.add("search-item");
			noResult.textContent = config.emptyMessage || "No results found";
			searchResults.appendChild(noResult);
		} else {
			data.forEach((item) => {
				if (!selectedIds.includes(item.id)) {
					const resultItem = document.createElement("div");
					resultItem.classList.add("search-item");
					resultItem.dataset.id = item.id;
					resultItem.innerHTML = `
						<img src="${item[config.imageProperty] || "/images/avatars/default-avatar.svg"}" alt="">
						<span>${item[config.displayProperty]}</span>
					`;
					resultItem.addEventListener("click", () => addTag(item));
					searchResults.appendChild(resultItem);
				}
			});
		}

		searchResults.classList.add("show");
	}

	// Adds tag for a selected item
	function addTag(item) {
		if (selectedIds.includes(item.id)) return;
		selectedIds.push(item.id);

		const tag = document.createElement("div");
		tag.classList.add("member-tag");
		tag.innerHTML = `
			<img src="${item[config.imageProperty] || "/images/avatars/default-avatar.svg"}" alt="">
			<span>${item[config.displayProperty]}</span>
			<button type="button" class="btn-remove" data-id="${item.id}">
				<i class="fa-solid fa-xmark"></i>
			</button>
		`;

		const removeBtn = tag.querySelector(".btn-remove");
		removeBtn.addEventListener("click", () => {
			selectedIds = selectedIds.filter((id) => id !== item.id);
			tag.remove();
			updateSelectedIdsInput();
		});

		container.appendChild(tag);
		searchInput.value = "";
		searchResults.innerHTML = "";
		searchResults.classList.remove("show");

		updateSelectedIdsInput();
	}

	// Removes the last tag if backspace is pressed on empty input
	function removeLastTag() {
		const tags = container.querySelectorAll(".member-tag");
		if (tags.length === 0) return;

		const lastTag = tags[tags.length - 1];
		const lastId = lastTag.querySelector(".btn-remove").dataset.id;
		selectedIds = selectedIds.filter((id) => id !== lastId);
		lastTag.remove();
		updateSelectedIdsInput();
	}

	function updateSelectedIdsInput() {
		const select = document.getElementById(config.selectedInputIds);
		if (!select) return;
		select.innerHTML = '';
		selectedIds.forEach(id => {
			const option = document.createElement('option');
			option.value = id;
			option.selected = true;
			option.textContent = id;
			select.appendChild(option);
		});
	}
	
	window.getTagSelectorSelectedIds = function() {
		return selectedIds.slice();
	};
}