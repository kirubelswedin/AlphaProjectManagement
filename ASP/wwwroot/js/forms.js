import { CLASSES } from './constants.js';

/**
 * Handles form submissions and error handling
 */

class FormManager {
  constructor() {
    this.init();
  }

  init() {
    this.initFormSubmissions();
    this.initImageUploads();
  }

  initImageUploads() {
    const imagePlaceholders = document.querySelectorAll('.image-placeholder');
    imagePlaceholders.forEach(placeholder => {
      const hiddenInput = placeholder.parentElement.querySelector('.hidden-input');
      const previewImage = placeholder.querySelector('.preview-image');
      
      if (!previewImage) {
        console.warn('Preview image element not found in placeholder');
        return;
      }
      
      placeholder.addEventListener('click', () => {
        hiddenInput.click();
      });
      
      hiddenInput.addEventListener('change', (e) => {
        const file = e.target.files[0];
        if (file) {
          const reader = new FileReader();
          reader.onload = (e) => {
            if (previewImage) {
              previewImage.src = e.target.result;
              previewImage.style.display = 'block';
            }
          };
          reader.readAsDataURL(file);
        }
      });
    });
  }

  initFormSubmissions() {
    document.querySelectorAll('form:not(.auth-form)').forEach(form => {
      form.addEventListener('submit', async (e) => {
        e.preventDefault();
        await this.handleFormSubmit(form);
      });
    });
  }

  async handleFormSubmit(form) {
    this.clearFormErrorMessages(form);
    const formData = new FormData(form);

    try {
      const response = await this.submitForm(form, formData);
      await this.handleFormResponse(response, form);
    } catch (error) {
      console.error("Form submission error:", error);
      this.handleFormError();
    }
  }

  async submitForm(form, formData) {
    return await fetch(form.action, {
      method: form.method || 'POST',
      body: formData,
      headers: {
        'Accept': 'application/json'
      }
    });
  }

  async handleFormResponse(response, form) {
    const data = await response.json();

    if (data.success) {
      // Close modal if exists
      const modal = form.closest('.modal');
      if (modal) {
        const modalId = modal.id;
        window.closeModal(modalId);
      }

      // Refresh the page or update UI as needed
      if (data.redirect) {
        window.location.href = data.redirect;
      } else {
        window.location.reload();
      }
    } else {
      this.handleFormErrors(form, data.errors || { '': [data.message] });
    }
  }

  handleFormErrors(form, errors) {
    Object.entries(errors).forEach(([field, messages]) => {
      const errorContainer = form.querySelector(`[data-valmsg-for="${field}"]`);
      if (errorContainer) {
        errorContainer.textContent = messages[0];
        const input = form.querySelector(`[name="${field}"]`);
        if (input) {
          input.classList.add('input-error');
        }
      }
    });
  }

  clearFormErrorMessages(form) {
    form.querySelectorAll('.field-error').forEach(error => {
      error.textContent = '';
    });
    form.querySelectorAll('.input-error').forEach(input => {
      input.classList.remove('input-error');
    });
  }

  handleFormError() {
    alert('An error occurred. Please try again.');
  }
}

export default new FormManager();