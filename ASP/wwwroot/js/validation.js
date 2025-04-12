document.addEventListener("DOMContentLoaded", () => {
	// Validation functions are available globally
});

function clearFormErrorMessages(form) {
	form
		.querySelectorAll('[data-val="true"]')
		.forEach((input) => input.classList.remove("input-error"));

	form.querySelectorAll("[data-valmsg-for]").forEach((span) => {
		span.innerText = "";
		span.classList.remove("field-error");
	});
}

function addFormErrorMessages(errors, form) {
	Object.entries(errors).forEach(([key, messages]) => {
		const input = form.querySelector(`[name="${key}"]`);
		if (input) {
			input.classList.add("input-error");
		}

		const span = form.querySelector(`[data-valmsg-for="${key}"]`);
		if (span) {
			span.innerText = Array.isArray(messages) ? messages.join(" ") : messages;
			span.classList.add("field-error");
		}
	});
}
