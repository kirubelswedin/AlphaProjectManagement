import { SELECTORS, CLASSES } from './constants.js';

/**
 * Handles opening/closing of mobile menu
 */


class MobileMenuManager {
	constructor() {
		this.init();
	}

	init() {
		this.checkbox = document.querySelector(SELECTORS.HAMBURGER_TOGGLE);
		this.menu = document.querySelector(SELECTORS.MOBILE_MENU);
		this.overlay = document.querySelector(SELECTORS.MOBILE_OVERLAY);

		if (!this.checkbox || !this.menu || !this.overlay) return;

		this.bindEvents();
		this.initMediaQuery();
	}

	bindEvents() {
		this.checkbox.addEventListener('change', () => this.toggleMenu(this.checkbox.checked));
		this.overlay.addEventListener('click', () => this.toggleMenu(false));
		this.menu.addEventListener('click', (e) => e.stopPropagation());
	}

	toggleMenu(isOpen) {
		this.menu.classList.toggle(CLASSES.OPEN, isOpen);
		this.overlay.classList.toggle(CLASSES.OPEN, isOpen);
		document.body.classList.toggle(CLASSES.MOBILE_MENU_ACTIVE, isOpen);
		document.body.style.overflow = isOpen ? "hidden" : "";

		if (!isOpen) {
			this.checkbox.checked = false;
		}
	}

	initMediaQuery() {
		const mediaQuery = window.matchMedia("(min-width: 501px)");
		const handleScreenSizeChange = (e) => {
			if (e.matches && this.menu.classList.contains(CLASSES.OPEN)) {
				this.toggleMenu(false);
			}
		};

		mediaQuery.addEventListener("change", handleScreenSizeChange);
		handleScreenSizeChange(mediaQuery);
	}
}

export default new MobileMenuManager();