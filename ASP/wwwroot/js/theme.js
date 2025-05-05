
document.addEventListener("DOMContentLoaded", () => {
	// initialize theme from localStorage or system
	const systemPrefersDark = window.matchMedia("(prefers-color-scheme: dark)");
	let theme = localStorage.getItem("theme") || (systemPrefersDark.matches ? "dark" : "light");
	setTheme(theme);
	
	// listen for theme toggles
	document.querySelectorAll(".theme-toggle, #darkModeToggle").
	forEach(toggle => {
		const eventType = toggle.id === "darkModeToggle" ? "change" : "click";
		toggle.addEventListener(eventType, toggleTheme);
	});
	
	// listen for custom theme change events
	window.addEventListener("themeChanged", e => {
		document.documentElement.setAttribute("data-theme", e.detail.theme);
		document.documentElement.style.colorScheme = e.detail.theme;
		const darkModeToggle = document.getElementById("darkModeToggle");
		if (darkModeToggle) darkModeToggle.checked = e.detail.theme === "dark";
	});
	
	// listen for system theme changes
	systemPrefersDark.addEventListener("change", e => {
		if (!localStorage.getItem("theme")) setTheme(e.matches ? "dark" : "light");
	});
});
// switch theme 
function toggleTheme() {
	const current = document.documentElement.getAttribute("data-theme");
	setTheme(current === "dark" ? "light" : "dark");
}

// set and save theme, notify the rest of the site of the change.
function setTheme(theme) {
	document.documentElement.setAttribute("data-theme", theme);
	document.documentElement.style.colorScheme = theme;
	localStorage.setItem("theme", theme);
	window.dispatchEvent(new CustomEvent("themeChanged", { detail: { theme } }));
}