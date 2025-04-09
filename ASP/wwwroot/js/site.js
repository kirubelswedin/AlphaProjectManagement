import ThemeManager from "./theme.js";
import WindowManager from "./windowManager.js";
import SidebarResizer from "./sidebarResizer.js";
import MobileMenu from "./mobileMenu.js";
import Notifications from "./notifications.js";
import TabScroll from "./tabScroll.js";
import ClientList from "./clientList.js";
import FormManager from "./forms.js";
import TagSelector from "./tags.js";
import SelectorManager from "./selector.js";
import ProjectManager from "./projects.js";
import Members from "./members.js";
import Clients from "./clients.js";
import Global from "./global.js";

const managers = {
	ThemeManager,
	WindowManager,
	SidebarResizer,
	MobileMenu,
	Notifications,
	TabScroll,
	ClientList,
	FormManager,
	TagSelector,
	SelectorManager,
	ProjectManager,
	Members,
	Clients,
	Global,
};

// Initialize each manager
document.addEventListener("DOMContentLoaded", () => {
	// Initialize global functions first
	Global.init();

	// Core managers that should always be initialized
	ThemeManager.init();
	SidebarResizer.init();
	MobileMenu.init();
	Notifications.init();
	FormManager.init();
	SelectorManager.init();

	// Feature-specific managers that should only be initialized if their elements exist
	const currentPath = window.location.pathname.toLowerCase();

	if (currentPath.includes("/admin/projects")) {
		TabScroll.init();
		window.projectManager = new ProjectManager();
	}

	if (currentPath.includes("/admin/members")) {
		window.memberManager = Members;
	}

	if (currentPath.includes("/admin/clients")) {
		Clients.init();
		ClientList.init();
	}
});

// Event listeners for manager interactions
document.addEventListener("openModal", (e) => {
	const { modalId, id } = e.detail;
	if (modalId === "editProjectModal" && id) {
		window.projectManager?.editProject(id);
	} else if (modalId === "editMemberModal" && id) {
		window.memberManager?.editMember(id);
	} else if (modalId === "editClientModal" && id) {
		window.clientManager?.editClient(id);
	} else {
		WindowManager.openModal(modalId);
	}
});

document.addEventListener("closeModal", (e) => {
	WindowManager.closeWindow(e.detail);
});

export default managers;
