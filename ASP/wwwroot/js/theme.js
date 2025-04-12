/**
 * Handles theme switching and system theme detection.
 */

document.addEventListener("DOMContentLoaded", () => {
	console.log("Theme script loaded");

	// Set theme on html element
	const savedTheme = localStorage.getItem("theme") || "light";
	document.documentElement.setAttribute("data-theme", savedTheme);
	document.documentElement.style.colorScheme = savedTheme;

	// System theme detection
	const prefersDark = window.matchMedia("(prefers-color-scheme: dark)").matches;
	if (!localStorage.getItem("theme") && prefersDark) {
		applyDarkTheme();
	}

	// Theme toggle event listeners
	const themeToggles = document.querySelectorAll(
		".theme-toggle, #darkModeToggle"
	);
	themeToggles.forEach((toggle) => {
		toggle.addEventListener(
			toggle.id === "darkModeToggle" ? "change" : "click",
			() => {
				console.log("Theme toggle clicked");
				toggleTheme();
			}
		);
	});

	// Theme changed event listener
	window.addEventListener("themeChanged", (e) => {
		console.log("Theme changed event received", e.detail.theme);
		document.querySelectorAll("[data-theme]").forEach((el) => {
			el.setAttribute("data-theme", e.detail.theme);
		});

		// Update the checkbox in dropdown menu
		const darkModeToggle = document.getElementById("darkModeToggle");
		if (darkModeToggle) {
			darkModeToggle.checked = e.detail.theme === "dark";
		}
	});

	// System theme change listener
	window
		.matchMedia("(prefers-color-scheme: dark)")
		.addEventListener("change", (e) => {
			if (!localStorage.getItem("theme")) {
				e.matches ? applyDarkTheme() : applyLightTheme();
			}
		});
});

function toggleTheme() {
	console.log("Toggle theme called");
	const currentTheme = document.documentElement.getAttribute("data-theme");
	const newTheme = currentTheme === "dark" ? "light" : "dark";
	console.log("Current theme:", currentTheme, "New theme:", newTheme);

	document.documentElement.setAttribute("data-theme", newTheme);
	document.documentElement.style.colorScheme = newTheme;
	localStorage.setItem("theme", newTheme);

	// Dispatch theme changed event
	window.dispatchEvent(
		new CustomEvent("themeChanged", { detail: { theme: newTheme } })
	);
}

function applyDarkTheme() {
	console.log("Applying dark theme");
	document.documentElement.setAttribute("data-theme", "dark");
	document.documentElement.style.colorScheme = "dark";
	localStorage.setItem("theme", "dark");
	window.dispatchEvent(
		new CustomEvent("themeChanged", { detail: { theme: "dark" } })
	);
}

function applyLightTheme() {
	console.log("Applying light theme");
	document.documentElement.setAttribute("data-theme", "light");
	document.documentElement.style.colorScheme = "light";
	localStorage.setItem("theme", "light");
	window.dispatchEvent(
		new CustomEvent("themeChanged", { detail: { theme: "light" } })
	);
}
