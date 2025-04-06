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
        document.querySelectorAll('.selector-field').forEach(field => {
            const select = field.querySelector('select');
            const container = field.querySelector('.selected-items-list');
            const searchInput = field.querySelector('.search-input input');
            const hiddenInput = field.querySelector('input[type="hidden"]');
            const optionsDropdown = field.querySelector('.options-dropdown');

            if (!select || !container || !searchInput || !optionsDropdown) return;

            // Initial setup
            this.updateSelected(select, container, hiddenInput);

            // Search input focus event
            searchInput.addEventListener('focus', (e) => {
                e.stopPropagation();
                this.showOptions(select, optionsDropdown, searchInput.value);
            });

            // Search input event
            searchInput.addEventListener('input', (e) => {
                e.stopPropagation();
                this.showOptions(select, optionsDropdown, searchInput.value);
            });

            // Handle option selection
            optionsDropdown.addEventListener('click', (e) => {
                e.stopPropagation();
                const optionItem = e.target.closest('.option-item');
                if (!optionItem) return;

                const value = optionItem.dataset.value;
                const option = select.querySelector(`option[value="${value}"]`);

                if (select.multiple) {
                    option.selected = !option.selected;
                } else {
                    select.value = value;
                }

                this.updateSelected(select, container, hiddenInput);
                if (!select.multiple) {
                    optionsDropdown.style.display = 'none';
                    searchInput.value = '';
                }
            });

            // Prevent clicks inside selector from bubbling
            field.addEventListener('click', (e) => {
                if (!e.target.closest('.remove-btn')) {
                    e.stopPropagation();
                }
            });
        });
    }

    initClickOutside() {
        document.addEventListener('click', (e) => {
            // Ignorera klick på själva selectorn och dess innehåll
            if (e.target.closest('.selector-field')) return;
            
            // Ignorera klick på modaler
            if (e.target.closest('.modal')) return;
            
            // Stäng alla dropdowns
            this.closeAllDropdowns();
        });
    }

    closeAllDropdowns() {
        document.querySelectorAll('.selector-field').forEach(field => {
            field.classList.remove('active');
            const dropdown = field.querySelector('.options-dropdown');
            if (dropdown) {
                dropdown.style.display = 'none';
            }
        });
    }

    showOptions(select, optionsDropdown, searchText) {
        if (!optionsDropdown) return;

        const options = Array.from(select.options).filter(opt => opt.value);
        optionsDropdown.innerHTML = '';

        options.forEach(option => {
            if (!searchText || option.text.toLowerCase().includes(searchText.toLowerCase())) {
                const item = document.createElement('div');
                item.className = 'option-item';
                item.dataset.value = option.value;
                
                const image = option.dataset.image || (select.id === 'projectClientSelect' 
                    ? '/images/project/Image-1.svg' 
                    : '/images/avatars/Avatar-1.svg');

                item.innerHTML = `
                    <img src="${image}" alt="${option.text}" />
                    <span>${option.text}</span>
                `;
                optionsDropdown.appendChild(item);
            }
        });

        optionsDropdown.style.display = options.length ? 'block' : 'none';
    }

    updateSelected(select, container, hiddenInput) {
        if (!container || !select) return;
        
        container.innerHTML = '';
        const selectedOptions = select.multiple ? Array.from(select.selectedOptions) : [select.options[select.selectedIndex]];
        
        const selectedValues = selectedOptions
            .filter(opt => opt.value)
            .map(opt => opt.value);
            
        if (hiddenInput) {
            hiddenInput.value = selectedValues.join(',');
        }
        
        selectedOptions.forEach(option => {
            if (!option.value) return;
            
            const chip = document.createElement('div');
            chip.className = 'chip';
            
            const defaultImage = select.id === 'projectClientSelect' ? '/images/project/Image-1.svg' : '/images/avatars/Avatar-1.svg';
            const imageUrl = option.dataset.image || defaultImage;
            
            chip.innerHTML = `
                <img src="${imageUrl}" alt="${option.text}" />
                <span>${option.text}</span>
                <button type="button" class="remove-btn">
                    <i class="fa-solid fa-times"></i>
                </button>
            `;
            
            chip.querySelector('.remove-btn').addEventListener('click', (e) => {
                e.stopPropagation();
                if (select.multiple) {
                    option.selected = false;
                } else {
                    select.value = '';
                }
                this.updateSelected(select, container, hiddenInput);
            });
            
            container.appendChild(chip);
        });
    }

    handleOptionClick(option) {
        const field = option.closest('.selector-field');
        const select = field.querySelector('select');
        const value = option.dataset.value;
        const label = option.textContent;
        const image = option.dataset.image;
        
        // Uppdatera valt värde
        select.value = value;
        
        // Uppdatera visad text och bild
        const selectedText = field.querySelector('.selected-text');
        const selectedImage = field.querySelector('.selected-image');
        
        if (selectedText) selectedText.textContent = label;
        if (selectedImage && image) {
            selectedImage.src = image;
            selectedImage.style.display = 'block';
        }
        
        // Stäng dropdown
        this.closeAllDropdowns();
    }
}

export default new SelectorManager(); 