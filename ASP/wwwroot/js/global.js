import ThemeManager from "./theme.js";
import { initWysiwygEditor } from "./wysiwyg.js";
import WindowManager from "./windowManager.js";

// Global functions
window.toggleTheme = () => {
	const currentTheme = document.body.getAttribute("data-theme");
	const isDarkTheme = currentTheme === "dark";

	if (isDarkTheme) {
		ThemeManager.applyLightTheme();
	} else {
		ThemeManager.applyDarkTheme();
	}
};

window.toggleAdminRole = async (userId, isAdmin) => {
	try {
		const response = await fetch(`/members/${userId}/toggle-admin`, {
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

const init = () => {
	WindowManager.init();
};

const openModal = (modalId) => {
	window.dispatchEvent(new CustomEvent("openModal", { detail: { modalId } }));
};

const closeModal = (modalId) => {
	window.dispatchEvent(new CustomEvent("closeModal", { detail: modalId }));
};

const closeAllModals = () => {
	WindowManager.closeAllWindows();
};

// Expose functions globally
window.init = init;
window.openModal = openModal;
window.closeModal = closeModal;
window.closeAllModals = closeAllModals;

// Initialize on DOM ready
document.addEventListener("DOMContentLoaded", init);

window.initWysiwygEditor = initWysiwygEditor;

export default {
	init() {
		const themeToggle = document.querySelector('[data-toggle="theme"]');
		if (themeToggle) {
			themeToggle.addEventListener("click", window.toggleTheme);
		}

		console.log("Global functions initialized");
	},
};
