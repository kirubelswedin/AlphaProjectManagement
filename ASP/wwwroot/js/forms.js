/**
 * Handles form submissions and error handling
 */

document.addEventListener("DOMContentLoaded", () => {
	initImageUploads();
	initFormSubmissions();
});

function initImageUploads() {
	document.querySelectorAll("[data-file-upload]").forEach((container) => {
		const input = container.querySelector('input[type="file"]');
		const preview = container.querySelector("img");
		const iconContainer = container.querySelector(".circle");
		const icon = iconContainer?.querySelector("i");

		container.addEventListener("click", () => {
			input?.click();
		});

		input?.addEventListener("change", (e) => {
			const file = e.target.files[0];
			if (file && file.type.startsWith("image/")) {
				const reader = new FileReader();
				reader.onload = () => {
					preview.src = reader.result;
					preview.classList.remove("hide");
					iconContainer.classList.add("circle-gray");
					icon?.classList.replace("fa-camera", "fa-pen");
				};
				reader.readAsDataURL(file);
			}
		});
	});
}

function initFormSubmissions() {
	document.querySelectorAll("form").forEach((form) => {
		form.addEventListener("submit", async (e) => {
			e.preventDefault();
			await handleFormSubmit(form);
		});
	});
}

async function handleFormSubmit(form) {
	clearFormErrorMessages(form);
	const formData = new FormData(form);

	// Debug logging
	console.log("Form data being sent:");
	for (let pair of formData.entries()) {
		console.log(pair[0] + ": " + pair[1]);
	}

	try {
		const response = await submitForm(form, formData);
		await handleFormResponse(response, form);
	} catch (error) {
		console.error("Form submission error:", error);
		handleFormError(form);
	}
}

async function submitForm(form, formData) {
	const response = await fetch(form.action, {
		method: form.method,
		body: formData,
	});

	if (!response.ok) {
		throw new Error(`HTTP error! status: ${response.status}`);
	}

	return await response.json();
}

async function handleFormResponse(response, form) {
	if (response.success) {
		// Close modal if exists
		const modal = form.closest(".modal");
		if (modal) {
			modal.classList.remove("show");
		}

		// Reset the form
		form.reset();

		// Refresh the page or update UI as needed
		if (form.dataset.reload !== "false") {
			window.location.reload();
		}
	} else {
		showValidationErrors(form, response.errors);
	}
}

function showValidationErrors(form, errors) {
	if (!errors) return;

	Object.entries(errors).forEach(([field, messages]) => {
		const errorElement = form.querySelector(`[data-valmsg-for="${field}"]`);
		if (errorElement) {
			errorElement.textContent = messages.join(", ");
		}
	});
}

function clearFormErrorMessages(form) {
	// Clear all error messages
	form.querySelectorAll("[data-valmsg-for]").forEach((element) => {
		element.textContent = "";
	});
}

function handleFormError(form) {
	const errorElement = form.querySelector(".field-error");
	if (errorElement) {
		errorElement.textContent =
			"An error occurred while submitting the form. Please try again.";
	}
}
