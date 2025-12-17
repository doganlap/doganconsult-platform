/**
 * DC OS Sidebar Controller
 * Manages sidebar auto-collapse and user preferences
 */
(function () {
    'use strict';

    const STORAGE_KEY = 'dc-sidebar-collapsed';
    const SIDEBAR_SELECTORS = [
        '.lpx-sidebar',
        '.lpx-side-menu',
        '[class*="sidebar"]'
    ];

    // Initialize sidebar state on DOM ready
    document.addEventListener('DOMContentLoaded', initSidebar);
    
    // Also listen for Blazor navigation
    if (typeof Blazor !== 'undefined') {
        Blazor.addEventListener('enhancedload', initSidebar);
    }

    function initSidebar() {
        const sidebar = findSidebar();
        if (!sidebar) {
            // Retry after a short delay if sidebar not found
            setTimeout(initSidebar, 500);
            return;
        }

        // Get saved preference or default to collapsed
        const isCollapsed = getSavedState();
        
        if (isCollapsed) {
            collapseSidebar(sidebar);
        }

        // Setup toggle functionality
        setupToggle(sidebar);
    }

    function findSidebar() {
        for (const selector of SIDEBAR_SELECTORS) {
            const element = document.querySelector(selector);
            if (element) return element;
        }
        return null;
    }

    function getSavedState() {
        const saved = localStorage.getItem(STORAGE_KEY);
        // Default to collapsed (true) if not set
        return saved === null ? true : saved === 'true';
    }

    function saveState(isCollapsed) {
        localStorage.setItem(STORAGE_KEY, isCollapsed.toString());
    }

    function collapseSidebar(sidebar) {
        sidebar.classList.add('collapsed', 'lpx-sidebar-mini');
        document.body.classList.add('sidebar-collapsed');
        
        // Also update content wrapper margin
        const contentWrapper = document.querySelector('.lpx-content-wrapper');
        if (contentWrapper) {
            contentWrapper.style.marginLeft = 'var(--lpx-sidebar-mini-width, 70px)';
        }
    }

    function expandSidebar(sidebar) {
        sidebar.classList.remove('collapsed', 'lpx-sidebar-mini');
        document.body.classList.remove('sidebar-collapsed');
        
        // Reset content wrapper margin
        const contentWrapper = document.querySelector('.lpx-content-wrapper');
        if (contentWrapper) {
            contentWrapper.style.marginLeft = '';
        }
    }

    function toggleSidebar(sidebar) {
        const isCurrentlyCollapsed = sidebar.classList.contains('collapsed') || 
                                     sidebar.classList.contains('lpx-sidebar-mini');
        
        if (isCurrentlyCollapsed) {
            expandSidebar(sidebar);
            saveState(false);
        } else {
            collapseSidebar(sidebar);
            saveState(true);
        }
    }

    function setupToggle(sidebar) {
        // Find or create toggle button
        const existingToggle = document.querySelector('.lpx-sidebar-toggle, .sidebar-toggle, [data-sidebar-toggle]');
        
        if (existingToggle) {
            existingToggle.addEventListener('click', function (e) {
                e.preventDefault();
                toggleSidebar(sidebar);
            });
        }

        // Also handle keyboard shortcut (Ctrl + B)
        document.addEventListener('keydown', function (e) {
            if (e.ctrlKey && e.key === 'b') {
                e.preventDefault();
                toggleSidebar(sidebar);
            }
        });

        // Handle hover expand on collapsed sidebar
        sidebar.addEventListener('mouseenter', function () {
            if (this.classList.contains('collapsed') || this.classList.contains('lpx-sidebar-mini')) {
                this.classList.add('hover-expanded');
            }
        });

        sidebar.addEventListener('mouseleave', function () {
            this.classList.remove('hover-expanded');
        });
    }

    // Expose global function for manual control
    window.dcSidebar = {
        toggle: function () {
            const sidebar = findSidebar();
            if (sidebar) toggleSidebar(sidebar);
        },
        collapse: function () {
            const sidebar = findSidebar();
            if (sidebar) {
                collapseSidebar(sidebar);
                saveState(true);
            }
        },
        expand: function () {
            const sidebar = findSidebar();
            if (sidebar) {
                expandSidebar(sidebar);
                saveState(false);
            }
        },
        isCollapsed: function () {
            const sidebar = findSidebar();
            return sidebar && (sidebar.classList.contains('collapsed') || 
                              sidebar.classList.contains('lpx-sidebar-mini'));
        }
    };
})();
