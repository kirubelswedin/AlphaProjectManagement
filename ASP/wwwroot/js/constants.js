export const SELECTORS = {
	// Generic
	MENU_BUTTONS: '[data-type="menu"]',
	MODAL_BUTTONS: '[data-type="modal"]',
	CLOSE_BUTTONS: '[data-type="close"]',
	TOGGLES: '[data-type="toggle"]',

	// Profile
	PROFILE_AVATAR: "#profileAvatar",
	PROFILE_DROPDOWN: "#profileDropdown",

	// Layout
	SIDEBAR: "#sidebar",
	MOBILE_MENU: ".mobile-menu",
	MOBILE_OVERLAY: ".mobile-menu-overlay",
	HAMBURGER_TOGGLE: "#hamburger-toggle",

	// Theme
	DARK_MODE_TOGGLE: "#darkModeToggle",

	// Forms
	NUMERIC_INPUTS: 'input[data-type="numeric"]',
	BUDGET_INPUTS: 'input[data-type="budget"]',

	// Clients
	CLIENT_CHECKBOX: ".client-checkbox",
	SELECT_ALL_CLIENTS: "#selectAllClients",
	SELECTED_CLIENTS_INFO: ".selected-clients-info",
	SELECTED_CLIENTS_COUNT: "#selectedClientsCount",

	MODAL: ".modal",
	MODAL_TRIGGER: '[data-modal="true"]',
	MODAL_CLOSE: '[data-close="true"]',
};

export const CLASSES = {
	SHOW: "show",
	ACTIVE: "active",
	OPEN: "open",
	MINIMIZED: "minimized",
	HAS_OVERFLOW: "has-overflow",
	MOBILE_MENU_ACTIVE: "mobile-menu-active",
	INPUT_ERROR: "input-error",
	FIELD_ERROR: "field-error",
};

export const THEME = {
	KEY: "theme",
	DARK: "dark",
	LIGHT: "light",
	ATTRIBUTE: "data-theme",
};

export const SIDEBAR = {
	STORAGE_KEY: "sidebar-width",
	DEFAULT_WIDTH: 16.875,
	MIN_WIDTH: 4,
	MOBILE_BREAKPOINT: 800,
};
