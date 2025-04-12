document.addEventListener("DOMContentLoaded", () => {
	initPhoneInputs();
	initBudgetInputs();
});

function initPhoneInputs() {
	document.querySelectorAll('input[type="tel"]').forEach((input) => {
		input.addEventListener("input", (e) => {
			let value = e.target.value;
			value = value.replace(/[^\d\s+().\-]/g, "");

			if (value.indexOf("+") > 0) {
				value = value.replace(/(?!^)\+/g, "");
			}
			if (value.startsWith("+")) {
				value = "+" + value.substring(1).replace(/\+/g, "");
			}
			e.target.value = value;
		});
	});
}

function initBudgetInputs() {
	document.querySelectorAll('input[data-type="budget"]').forEach((input) => {
		input.addEventListener("input", (e) => {
			let value = e.target.value.replace(/[^\d.]/g, "");

			// Only one decimal point
			const parts = value.split(".");
			if (parts.length > 2) {
				value = parts[0] + "." + parts.slice(1).join("");
			}

			// Only 8 digits before the decimal point
			if (parts[0].length > 8) {
				parts[0] = parts[0].substring(0, 8);
				value = parts.length > 1 ? parts[0] + "." + parts[1] : parts[0];
			}

			// Add spaces every three digits
			let wholeNumber = parts[0];
			let formattedWholeNumber = "";

			for (let i = wholeNumber.length - 1, count = 0; i >= 0; i--, count++) {
				if (count > 0 && count % 3 === 0) {
					formattedWholeNumber = " " + formattedWholeNumber;
				}
				formattedWholeNumber = wholeNumber[i] + formattedWholeNumber;
			}

			// Combine
			value =
				parts.length > 1
					? formattedWholeNumber + "." + parts[1]
					: formattedWholeNumber;

			e.target.value = value;
		});
	});
}

/**
 * Formats a date string to YYYY-MM-DD format
 * @param {string} dateString - The date string to format
 * @returns {string} Formatted date string or empty string if input is invalid
 */
function formatDate(dateString) {
	if (!dateString) return "";
	const date = new Date(dateString);
	return date.toISOString().split("T")[0];
}
