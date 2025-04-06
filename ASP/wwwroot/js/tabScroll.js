import {CLASSES} from './constants.js';

/**
 * Handle tab filtering and horizontal scroll
 */

class TabScrollManager {
    constructor() {
        this.init();
    }

    init() {
        this.tabsContainer = document.querySelector('.projects-tabs');
        if (!this.tabsContainer) return;

        this.bindEvents();
        this.checkForOverflow();
        this.initializeActiveTab();
    }

    bindEvents() {
        // Scroll events
        window.addEventListener('resize', () => this.checkForOverflow());
        this.tabsContainer.addEventListener('scroll', () => this.updateShadowEffect());

        // Tab click events
        this.tabsContainer.querySelectorAll('.tab').forEach(tab => {
            tab.addEventListener('click', (e) => this.handleTabClick(e));
        });
    }

    handleTabClick(e) {
        e.preventDefault();
        const tab = e.currentTarget;
        const status = tab.textContent.trim().split('[')[0].trim();
        
        // Update URL
        const url = new URL(window.location);
        url.searchParams.set('tab', status);
        window.history.pushState({}, '', url);

        // Update active state
        this.tabsContainer.querySelectorAll('.tab').forEach(t => {
            t.classList.remove('active');
        });
        tab.classList.add('active');

        // Fetch filtered projects
        this.fetchFilteredProjects(status);
    }

    async fetchFilteredProjects(status) {
        try {
            const response = await fetch(`/projects/list?tab=${status}`, {
                headers: {
                    'X-Requested-With': 'XMLHttpRequest'
                }
            });
            
            if (!response.ok) throw new Error('Failed to fetch projects');

            const projectsGrid = document.querySelector('.projects-grid');
            if (projectsGrid) {
                const html = await response.text();
                projectsGrid.innerHTML = ''; 
                
                
                const temp = document.createElement('div');
                temp.innerHTML = html;
                
                // For each project in the response, create a new project card
                const projects = temp.children;
                Array.from(projects).forEach(project => {
                    projectsGrid.appendChild(project);
                });
            }
        } catch (error) {
            console.error('Error fetching filtered projects:', error);
        }
    }

    //  
    checkForOverflow() {
        const hasOverflow = this.tabsContainer.scrollWidth > this.tabsContainer.clientWidth;
        this.tabsContainer.classList.toggle(CLASSES.HAS_OVERFLOW, hasOverflow);
    }

    updateShadowEffect() {
        const isAtEnd = this.tabsContainer.scrollLeft + this.tabsContainer.clientWidth >=
            this.tabsContainer.scrollWidth - 10;

        this.tabsContainer.classList.toggle(CLASSES.HAS_OVERFLOW, !isAtEnd);
    }

    initializeActiveTab() {
        const url = new URL(window.location);
        const activeStatus = url.searchParams.get('tab') || 'ALL';
        
        const activeTab = Array.from(this.tabsContainer.querySelectorAll('.tab')).find(
            tab => tab.textContent.trim().split('[')[0].trim() === activeStatus
        );
        
        if (activeTab) {
            activeTab.classList.add('active');
        }
    }
}

export default new TabScrollManager();