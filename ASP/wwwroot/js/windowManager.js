
// took some help from chatGPT to get this to work as I wanted
document.addEventListener("DOMContentLoaded", () => {
    document.body.addEventListener("click", handleBodyClick);
    document.body.addEventListener("keydown", handleKeydown);
});

// Modal handling 
function openModal(modalId, data = null) {
    closeAllDropdowns();
    const modal = document.getElementById(modalId);
    if (!modal) return;
    if (data) modal.dispatchEvent(new CustomEvent("modalData", { detail: data }));
    modal.classList.add("show");
    document.body.style.overflow = "hidden";
}

function closeModal(modalId) {
    const modal = document.getElementById(modalId);
    if (!modal) return;
    modal.classList.remove("show");
    document.body.style.overflow = "";
}

// Dropdown handling
function toggleDropdown(dropdownId, trigger) {
    const dropdown = document.getElementById(dropdownId);
    if (!dropdown) return;
    
    // If the dropdown is already active, close it and return (toggle)
    if (dropdown.classList.contains("active")) {
        dropdown.classList.remove("active");
        return;
    }
    
    // otherwise, close all dropdowns and open the clicked one
    closeAllDropdowns();
    closeAllModals();
    if (!dropdown) return;

    // positioning for the clientlist dropdown
    if (dropdownId.startsWith("client-dropdown-")) {
        positionDropdown(dropdown, trigger);
    }
    dropdown.classList.toggle("active");
}

function closeAllDropdowns() {
    document.querySelectorAll(".dropdown-wrapper.active").forEach(d => d.classList.remove("active"));
}

function closeAllModals() {
    document.querySelectorAll(".modal.show").forEach(m => closeModal(m.id));
}

// positioning for the clientlist dropdown
// couldn't find a better way to fix the overflow-x problem in CSS....
function positionDropdown(dropdown, trigger) {
    const tableWrapper = trigger.closest(".table-wrapper");
    if (!tableWrapper) return;
    const rect = trigger.getBoundingClientRect();
    const tableRect = tableWrapper.getBoundingClientRect();
    const offsetX = 0, offsetY = 2;
    dropdown.style.position = "absolute";
    dropdown.style.top = `${rect.top - tableRect.top + rect.height + offsetY}px`;
    dropdown.style.left = `${rect.left - tableRect.left - dropdown.offsetWidth + rect.width + offsetX}px`;
    dropdown.style.visibility = "visible";
    dropdown.style.display = "block";
}

// Event delegation
function handleBodyClick(e) {
    // console.log('Clicked:', e.target);
    // Modal open
    const modalTrigger = e.target.closest("[data-window='modal']");
    if (modalTrigger) {
        const modalId = modalTrigger.dataset.target?.replace("#", "") || modalTrigger.dataset.modal;
        const data = modalTrigger.dataset.data ? JSON.parse(modalTrigger.dataset.data) : null;
        openModal(modalId, data);
        e.preventDefault();
        return;
    }
    // Dropdown open
    const dropdownTrigger = e.target.closest("[data-window='dropdown']");
    if (dropdownTrigger) {
        const dropdownId = dropdownTrigger.dataset.target?.replace("#", "");
        if (dropdownId) {
            toggleDropdown(dropdownId, dropdownTrigger);
            e.preventDefault();
            e.stopPropagation();
        }
        return;
    }
    // Modal close (click outside)
    if (e.target.classList.contains("modal")) {
        closeModal(e.target.id);
        return;
    }
    // Dropdown close (click outside)
    if (!e.target.closest(".dropdown-wrapper")) {
        closeAllDropdowns();
    }
}

function handleKeydown(e) {
    if (e.key === "Escape") {
        closeAllDropdowns();
        closeAllModals();
    }
}

// Global event for closing modals 
document.addEventListener("closeModal", e => {
    if (e.detail) closeModal(e.detail);
});

// Global event for opening modals 
window.addEventListener("openModal", e => {
    const { modalId, ...data } = e.detail || {};
    if (modalId) openModal(modalId, Object.keys(data).length ? data : null);
});

window.WindowManager = { openModal, closeModal };


