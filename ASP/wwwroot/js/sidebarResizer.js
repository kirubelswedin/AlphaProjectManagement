import { SELECTORS, CLASSES, SIDEBAR } from "./constants.js";

/**
 * Handles the sidebar's width and visibility based on screen size and user interaction.
 */

class SidebarManager {
	constructor() {
		this.init();
	}

	init() {
		this.sidebar = document.querySelector("aside");
		this.resizer = document.querySelector(".sidebar-resizer");

		if (!this.sidebar || !this.resizer) return;

		this.initSidebar();
		this.bindEvents();
	}

	bindEvents() {
		this.resizer.addEventListener("click", (e) => {
			e.preventDefault();
			this.toggleSidebar();
		});

		window.addEventListener("resize", () => this.initSidebar());
	}

	expandSidebar() {
		document.documentElement.style.setProperty(
			"--sidebar-width",
			`${SIDEBAR.DEFAULT_WIDTH}rem`
		);
		this.sidebar.style.width = `${SIDEBAR.DEFAULT_WIDTH}rem`;
		this.sidebar.classList.remove(CLASSES.MINIMIZED);
		localStorage.setItem(SIDEBAR.STORAGE_KEY, SIDEBAR.DEFAULT_WIDTH);
	}

	minimizeSidebar() {
		document.documentElement.style.setProperty(
			"--sidebar-width",
			`${SIDEBAR.MIN_WIDTH}rem`
		);
		this.sidebar.style.width = `${SIDEBAR.MIN_WIDTH}rem`;
		this.sidebar.classList.add(CLASSES.MINIMIZED);
		localStorage.setItem(SIDEBAR.STORAGE_KEY, SIDEBAR.MIN_WIDTH);
	}

	toggleSidebar() {
		const currentWidth = parseFloat(getComputedStyle(this.sidebar).width) / 16;
		currentWidth <= SIDEBAR.MIN_WIDTH
			? this.expandSidebar()
			: this.minimizeSidebar();
	}

	initSidebar() {
		const savedWidth = localStorage.getItem(SIDEBAR.STORAGE_KEY);

		if (window.innerWidth <= SIDEBAR.MOBILE_BREAKPOINT) {
			this.sidebar.style.width = "";
			this.sidebar.classList.remove(CLASSES.MINIMIZED);
			document.documentElement.style.setProperty(
				"--sidebar-width",
				`${SIDEBAR.DEFAULT_WIDTH}rem`
			);
		} else if (savedWidth) {
			const width = parseFloat(savedWidth);
			width <= SIDEBAR.MIN_WIDTH ? this.minimizeSidebar() : this.expandSidebar();
		} else {
			this.expandSidebar();
		}
	}
}

export default new SidebarManager();
