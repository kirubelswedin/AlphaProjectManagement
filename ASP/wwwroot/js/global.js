import ThemeManager from "./theme.js";
import { initWysiwygEditor } from "./wysiwyg.js";
import WindowManager from "./windowManager.js";

// Global functions
window.toggleTheme = () => {
	const body = document.body;
	const currentTheme = body.getAttribute("data-theme");
	const newTheme = currentTheme === "dark" ? "light" : "dark";

	body.setAttribute("data-theme", newTheme);
	localStorage.setItem("theme", newTheme);
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

// Initialize theme from localStorage
document.addEventListener("DOMContentLoaded", () => {
	const savedTheme = localStorage.getItem("theme") || "light";
	document.body.setAttribute("data-theme", savedTheme);
});
