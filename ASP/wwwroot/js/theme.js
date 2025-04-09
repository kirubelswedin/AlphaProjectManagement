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
		document.documentElement.setAttribute("data-theme", THEME.DARK);
		document.documentElement.style.colorScheme = "dark";
		localStorage.setItem(THEME.KEY, THEME.DARK);

		// Dispatch event for other components that might need to react to theme changes
		window.dispatchEvent(
			new CustomEvent("themeChanged", { detail: { theme: THEME.DARK } })
		);
	}

	applyLightTheme() {
		document.documentElement.setAttribute("data-theme", THEME.LIGHT);
		document.documentElement.style.colorScheme = "light";
		localStorage.setItem(THEME.KEY, THEME.LIGHT);

		// Dispatch event for other components that might need to react to theme changes
		window.dispatchEvent(
			new CustomEvent("themeChanged", { detail: { theme: THEME.LIGHT } })
		);
	}
}

const themeManager = new ThemeManager();

export default {
	init: themeManager.init.bind(themeManager),
	applyDarkTheme: themeManager.applyDarkTheme.bind(themeManager),
	applyLightTheme: themeManager.applyLightTheme.bind(themeManager),
};
