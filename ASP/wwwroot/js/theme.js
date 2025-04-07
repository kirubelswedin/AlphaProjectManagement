import { THEME } from "./constants.js";

/**
 * Handles theme switching and system theme detection.
 */

class ThemeManager {
	constructor() {
		// Remove init call from constructor
	}

	init() {
		this.initializeTheme();
		this.initSystemThemeListener();
	}

	initializeTheme() {
		const savedTheme = localStorage.getItem(THEME.KEY);
		const prefersDark = window.matchMedia("(prefers-color-scheme: dark)").matches;

		if (savedTheme === THEME.DARK || (!savedTheme && prefersDark)) {
			this.applyDarkTheme();
		} else {
			this.applyLightTheme();
		}
	}

	initSystemThemeListener() {
		window
			.matchMedia("(prefers-color-scheme: dark)")
			.addEventListener("change", (e) => {
				if (!localStorage.getItem(THEME.KEY)) {
					e.matches ? this.applyDarkTheme() : this.applyLightTheme();
				}
			});
	}

	applyDarkTheme() {
		document.body.setAttribute("data-theme", THEME.DARK);
		localStorage.setItem(THEME.KEY, THEME.DARK);
		document.documentElement.style.colorScheme = "dark";
	}

	applyLightTheme() {
		document.body.setAttribute("data-theme", THEME.LIGHT);
		localStorage.setItem(THEME.KEY, THEME.LIGHT);
		document.documentElement.style.colorScheme = "light";
	}
}

const themeManager = new ThemeManager();

export default {
	init: themeManager.init.bind(themeManager),
	applyDarkTheme: themeManager.applyDarkTheme.bind(themeManager),
	applyLightTheme: themeManager.applyLightTheme.bind(themeManager),
};
