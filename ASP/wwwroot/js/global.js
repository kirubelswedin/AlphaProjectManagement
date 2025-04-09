import ThemeManager from "./theme.js";
import { initWysiwygEditor } from "./wysiwyg.js";
import WindowManager from "./windowManager.js";
import { THEME } from "./constants.js";

// Initialize managers
const initManagers = () => {
	try {
		// Initialize WindowManager first as it's most critical
		if (typeof WindowManager !== "undefined") {
			WindowManager.init();
		}

		// Initialize ThemeManager
		if (typeof ThemeManager !== "undefined") {
			ThemeManager.init();
		}
	} catch (error) {
		console.error("Error initializing managers:", error);
	}
};

// Global functions
window.toggleTheme = () => {
	const currentTheme = document.documentElement.getAttribute("data-theme");
	if (currentTheme === THEME.DARK) {
		ThemeManager.applyLightTheme();
	} else {
		ThemeManager.applyDarkTheme();
	}
};

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

// Window management functions
const openModal = (modalId) => {
	try {
		window.dispatchEvent(new CustomEvent("openModal", { detail: { modalId } }));
	} catch (error) {
		console.error("Error opening modal:", error);
	}
};

const closeModal = (modalId) => {
	try {
		window.dispatchEvent(new CustomEvent("closeModal", { detail: modalId }));
	} catch (error) {
		console.error("Error closing modal:", error);
	}
};

const closeAllModals = () => {
	try {
		if (typeof WindowManager !== "undefined") {
			WindowManager.closeAllWindows();
		}
	} catch (error) {
		console.error("Error closing all modals:", error);
	}
};

// Expose functions globally
window.init = initManagers;
window.openModal = WindowManager.openModal;
window.closeModal = WindowManager.closeModal;
window.closeAllModals = WindowManager.closeAllWindows;
window.initWysiwygEditor = initWysiwygEditor;

// Initialize on DOM ready
document.addEventListener("DOMContentLoaded", initManagers);

export default {
	init() {
		const themeToggle = document.querySelector('[data-toggle="theme"]');
		if (themeToggle) {
			themeToggle.addEventListener("click", window.toggleTheme);
		}

		console.log("Global functions initialized");
	},
};

// Initialize theme from localStorage
document.addEventListener("DOMContentLoaded", () => {
	const savedTheme = localStorage.getItem("theme") || "light";
	document.body.setAttribute("data-theme", savedTheme);
});
