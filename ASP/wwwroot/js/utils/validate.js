import { CLASSES } from '../constants.js';

/**
 * Handles validation error messages for forms
 */

class ValidationManager {
    clearFormErrorMessages(form) {
        form.querySelectorAll('[data-val="true"]')
            .forEach(input => input.classList.remove(CLASSES.INPUT_ERROR));

        form.querySelectorAll('[data-valmsg-for]')
            .forEach(span => {
                span.innerText = '';
                span.classList.remove(CLASSES.FIELD_ERROR);
            });
    }

    addFormErrorMessages(errors, form) {
        Object.entries(errors).forEach(([key, messages]) => {
            const input = form.querySelector(`[name="${key}"]`);
            if (input) {
                input.classList.add(CLASSES.INPUT_ERROR);
            }

            const span = form.querySelector(`[data-valmsg-for="${key}"]`);
            if (span) {
                span.innerText = Array.isArray(messages) ? messages.join(' ') : messages;
                span.classList.add(CLASSES.FIELD_ERROR);
            }
        });
    }
}

export default new ValidationManager();