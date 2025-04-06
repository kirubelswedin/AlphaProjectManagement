/**
 * Handles the selection and editing of clients in a list.
 */

import { SELECTORS, CLASSES } from './constants.js';

class ClientListManager {
    constructor() {
        this.selectedClients = [];
        this.init();
    }

    init() {
        this.initSelectors();
        this.bindEvents();
    }

    initSelectors() {
        this.selectAllCheckbox = document.querySelector(SELECTORS.SELECT_ALL_CLIENTS);
        this.selectedInfo = document.querySelector(SELECTORS.SELECTED_CLIENTS_INFO);
        this.selectedCount = document.querySelector(SELECTORS.SELECTED_CLIENTS_COUNT);

        if (!this.selectAllCheckbox || !this.selectedInfo || !this.selectedCount) return;
    }

    bindEvents() {
        // Bind checkbox events
        this.selectAllCheckbox?.addEventListener('change', () =>
            this.toggleSelectAllClients(this.selectAllCheckbox.checked));

        document.querySelectorAll(SELECTORS.CLIENT_CHECKBOX).forEach(checkbox => {
            checkbox.addEventListener('change', () => this.toggleSelectClient(checkbox));
        });

        // Bind edit client events
        document.addEventListener('editClient', (e) => this.editClient(e.detail));
    }

    toggleSelectAllClients(checked) {
        const clientCheckboxes = document.querySelectorAll(SELECTORS.CLIENT_CHECKBOX);
        this.selectedClients = [];

        clientCheckboxes.forEach(cb => {
            cb.checked = checked;
            if (checked) {
                this.selectedClients.push(cb.getAttribute('data-id'));
            }
        });

        this.updateSelectedClientsInfo();
    }

    toggleSelectClient(checkbox) {
        const clientId = checkbox.getAttribute('data-id');

        if (checkbox.checked) {
            if (!this.selectedClients.includes(clientId)) {
                this.selectedClients.push(clientId);
            }
        } else {
            this.selectedClients = this.selectedClients.filter(id => id !== clientId);
            this.selectAllCheckbox.checked = false;
        }

        this.updateSelectedClientsInfo();
    }

    unselectAllClients() {
        document.querySelectorAll(SELECTORS.CLIENT_CHECKBOX).forEach(cb => {
            cb.checked = false;
        });

        this.selectAllCheckbox.checked = false;
        this.selectedClients = [];
        this.updateSelectedClientsInfo();
    }

    updateSelectedClientsInfo() {
        if (this.selectedClients.length > 0) {
            this.selectedInfo.style.display = 'flex';
            this.selectedCount.textContent = `(${this.selectedClients.length} selected)`;
        } else {
            this.selectedInfo.style.display = 'none';
        }
    }

    async editClient(id) {
        try {
            const response = await fetch(`/clients/${id}`);
            if (!response.ok) throw new Error('Client not found');

            const client = await response.json();
            this.populateEditForm(client);
        } catch (error) {
            console.error('Error fetching client:', error);
            alert('Could not load client data');
        }
    }

    populateEditForm(client) {
        const fields = ['id', 'clientName', 'contactPerson', 'email', 'phone'];

        fields.forEach(field => {
            const element = document.getElementById(field);
            if (element) element.value = client[field] || '';
        });

        // Update modal title
        const modalLabel = document.getElementById('addClientModalLabel');
        if (modalLabel) modalLabel.textContent = 'Edit Client';

        // show modal
        const event = new CustomEvent('openModal', { detail: 'addClientModal' });
        document.dispatchEvent(event);
    }

    async refreshClientList() {
        try {
            const response = await fetch('/clients/list');
            if (!response.ok) throw new Error('Failed to fetch clients');

            // for now, just reload the page
            window.location.reload();
        } catch (error) {
            console.error('Error fetching clients:', error);
        }
    }

    resetClientForm() {
        const form = document.getElementById('addClientForm');
        if (!form) return;

        form.reset();
        document.getElementById('clientId').value = '';
        document.getElementById('addClientModalLabel').textContent = 'Add Client';
    }
}

export default new ClientListManager();