// Header and Footer Enhancement JavaScript
// Provides functionality for the enhanced header and footer components

(function() {
    'use strict';

    // Initialize all header/footer functionality when DOM is ready
    document.addEventListener('DOMContentLoaded', function() {
        initializeTooltips();
        initializeGlobalSearch();
        initializeNotificationBell();
        initializeFooterFunctionality();
        initializeMobileNavigation();
        initializeStatusIndicators();
    });

    // Initialize Bootstrap tooltips for status indicators and buttons
    function initializeTooltips() {
        var tooltipTriggerList = [].slice.call(document.querySelectorAll('[data-bs-toggle="tooltip"]'));
        var tooltipList = tooltipTriggerList.map(function(tooltipTriggerEl) {
            return new bootstrap.Tooltip(tooltipTriggerEl, {
                trigger: 'hover focus'
            });
        });
    }

    // Global search functionality
    function initializeGlobalSearch() {
        const searchInput = document.querySelector('.header-search input');
        if (!searchInput) return;

        let searchTimeout;
        
        searchInput.addEventListener('input', function(e) {
            clearTimeout(searchTimeout);
            const query = e.target.value.trim();
            
            if (query.length < 2) {
                hideSearchResults();
                return;
            }
            
            searchTimeout = setTimeout(() => {
                performGlobalSearch(query);
            }, 300);
        });

        // Hide results when clicking outside
        document.addEventListener('click', function(e) {
            if (!e.target.closest('.header-search')) {
                hideSearchResults();
            }
        });

        // Handle keyboard navigation
        searchInput.addEventListener('keydown', function(e) {
            if (e.key === 'Escape') {
                hideSearchResults();
                searchInput.blur();
            }
        });
    }

    // Perform global search across patients, medications, etc.
    function performGlobalSearch(query) {
        // Show loading indicator
        showSearchLoading();

        // Simulate search API call
        setTimeout(() => {
            const mockResults = generateMockSearchResults(query);
            displaySearchResults(mockResults);
        }, 500);
    }

    // Generate mock search results for demonstration
    function generateMockSearchResults(query) {
        const results = [];
        
        // Mock patient results
        if (query.toLowerCase().includes('patient') || query.toLowerCase().includes('john')) {
            results.push({
                type: 'patient',
                title: 'John Doe',
                subtitle: 'Patient ID: P001 • Age: 45',
                url: '/Patients/Details/1',
                icon: 'bi-person'
            });
        }
        
        // Mock medication results
        if (query.toLowerCase().includes('med') || query.toLowerCase().includes('aspirin')) {
            results.push({
                type: 'medication',
                title: 'Aspirin 100mg',
                subtitle: 'Pain reliever • Stock: 150 units',
                url: '/Medications/Details/1',
                icon: 'bi-capsule'
            });
        }
        
        // Mock prescription results
        if (query.toLowerCase().includes('presc') || query.toLowerCase().includes('rx')) {
            results.push({
                type: 'prescription',
                title: 'Prescription RX001',
                subtitle: 'John Doe • Created: 2 days ago',
                url: '/Prescriptions/Details/1',
                icon: 'bi-file-medical'
            });
        }
        
        return results;
    }

    // Display search results dropdown
    function displaySearchResults(results) {
        hideSearchLoading();
        
        const searchContainer = document.querySelector('.header-search');
        if (!searchContainer) return;

        // Remove existing results
        const existingDropdown = searchContainer.querySelector('.search-results-dropdown');
        if (existingDropdown) {
            existingDropdown.remove();
        }

        // Create results dropdown
        const dropdown = document.createElement('div');
        dropdown.className = 'search-results-dropdown position-absolute bg-white border rounded shadow-lg';
        dropdown.style.cssText = 'top: 100%; left: 0; right: 0; z-index: 1050; max-height: 300px; overflow-y: auto;';

        if (results.length === 0) {
            dropdown.innerHTML = `
                <div class="p-3 text-center text-muted">
                    <i class="bi bi-search me-2"></i>No results found
                </div>
            `;
        } else {
            dropdown.innerHTML = results.map(result => `
                <a href="${result.url}" class="d-block p-3 text-decoration-none border-bottom search-result-item">
                    <div class="d-flex align-items-center">
                        <div class="me-3">
                            <i class="bi ${result.icon} text-primary fs-5"></i>
                        </div>
                        <div class="flex-grow-1">
                            <div class="fw-semibold text-dark">${result.title}</div>
                            <small class="text-muted">${result.subtitle}</small>
                        </div>
                        <div class="ms-2">
                            <i class="bi bi-arrow-right text-muted"></i>
                        </div>
                    </div>
                </a>
            `).join('');
        }

        searchContainer.appendChild(dropdown);

        // Add hover effects
        dropdown.querySelectorAll('.search-result-item').forEach(item => {
            item.addEventListener('mouseenter', function() {
                this.style.backgroundColor = '#f8f9fa';
            });
            item.addEventListener('mouseleave', function() {
                this.style.backgroundColor = '';
            });
        });
    }

    // Show search loading indicator
    function showSearchLoading() {
        const searchContainer = document.querySelector('.header-search');
        if (!searchContainer) return;

        hideSearchResults();
        
        const dropdown = document.createElement('div');
        dropdown.className = 'search-results-dropdown position-absolute bg-white border rounded shadow-lg';
        dropdown.style.cssText = 'top: 100%; left: 0; right: 0; z-index: 1050;';
        dropdown.innerHTML = `
            <div class="p-3 text-center">
                <div class="spinner-border spinner-border-sm text-primary me-2" role="status">
                    <span class="visually-hidden">Loading...</span>
                </div>
                Searching...
            </div>
        `;
        
        searchContainer.appendChild(dropdown);
    }

    // Hide search loading indicator
    function hideSearchLoading() {
        hideSearchResults();
    }

    // Hide search results dropdown
    function hideSearchResults() {
        const dropdown = document.querySelector('.search-results-dropdown');
        if (dropdown) {
            dropdown.remove();
        }
    }

    // Notification bell functionality
    function initializeNotificationBell() {
        const notificationBell = document.querySelector('[data-bs-toggle="tooltip"][title="Notifications"]');
        if (!notificationBell) return;

        notificationBell.addEventListener('click', function(e) {
            e.preventDefault();
            showNotifications();
        });
    }

    // Show notifications modal or dropdown
    function showNotifications() {
        // Create notification modal
        const modal = document.createElement('div');
        modal.className = 'modal fade';
        modal.innerHTML = `
            <div class="modal-dialog">
                <div class="modal-content">
                    <div class="modal-header">
                        <h5 class="modal-title">
                            <i class="bi bi-bell me-2"></i>Notifications
                        </h5>
                        <button type="button" class="btn-close" data-bs-dismiss="modal"></button>
                    </div>
                    <div class="modal-body">
                        <div class="text-center py-4">
                            <i class="bi bi-bell-slash text-muted fs-1 mb-3"></i>
                            <p class="text-muted">No new notifications</p>
                            <small class="text-muted">You're all caught up!</small>
                        </div>
                    </div>
                </div>
            </div>
        `;
        
        document.body.appendChild(modal);
        const bsModal = new bootstrap.Modal(modal);
        bsModal.show();
        
        // Clean up when modal is hidden
        modal.addEventListener('hidden.bs.modal', function() {
            modal.remove();
        });
    }

    // Mobile navigation enhancements
    function initializeMobileNavigation() {
        const navbarToggler = document.querySelector('.navbar-toggler');
        const navbarCollapse = document.querySelector('.navbar-collapse');
        
        if (!navbarToggler || !navbarCollapse) return;

        navbarToggler.addEventListener('click', function() {
            // Add smooth animation class
            navbarCollapse.style.transition = 'all 0.3s ease';
        });
    }

    // Status indicators functionality
    function initializeStatusIndicators() {
        const statusIndicators = document.querySelectorAll('.status-indicator');
        
        statusIndicators.forEach(indicator => {
            indicator.addEventListener('click', function() {
                const title = this.getAttribute('data-bs-original-title') || this.getAttribute('title');
                showStatusDetails(title);
            });
        });
    }

    // Show status details
    function showStatusDetails(statusType) {
        const modal = document.createElement('div');
        modal.className = 'modal fade';
        modal.innerHTML = `
            <div class="modal-dialog">
                <div class="modal-content">
                    <div class="modal-header">
                        <h5 class="modal-title">
                            <i class="bi bi-info-circle me-2"></i>${statusType}
                        </h5>
                        <button type="button" class="btn-close" data-bs-dismiss="modal"></button>
                    </div>
                    <div class="modal-body">
                        <div class="d-flex align-items-center mb-3">
                            <i class="bi bi-check-circle text-success fs-3 me-3"></i>
                            <div>
                                <h6 class="mb-1">System Status: Operational</h6>
                                <small class="text-muted">All systems are running normally</small>
                            </div>
                        </div>
                        <hr>
                        <div class="row g-3">
                            <div class="col-6">
                                <div class="text-center">
                                    <i class="bi bi-server text-primary fs-4"></i>
                                    <div class="mt-2">
                                        <small class="text-muted d-block">Server</small>
                                        <span class="badge bg-success">Online</span>
                                    </div>
                                </div>
                            </div>
                            <div class="col-6">
                                <div class="text-center">
                                    <i class="bi bi-database text-primary fs-4"></i>
                                    <div class="mt-2">
                                        <small class="text-muted d-block">Database</small>
                                        <span class="badge bg-success">Connected</span>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        `;
        
        document.body.appendChild(modal);
        const bsModal = new bootstrap.Modal(modal);
        bsModal.show();
        
        modal.addEventListener('hidden.bs.modal', function() {
            modal.remove();
        });
    }

    // Footer functionality
    function initializeFooterFunctionality() {
        // Add smooth scrolling for footer links
        document.querySelectorAll('.footer-links a[href^="#"]').forEach(link => {
            link.addEventListener('click', function(e) {
                e.preventDefault();
                const target = document.querySelector(this.getAttribute('href'));
                if (target) {
                    target.scrollIntoView({ behavior: 'smooth' });
                }
            });
        });
    }

    // Global functions for footer links (these would be implemented based on specific needs)
    window.showUserGuide = function() {
        showInfoModal('User Guide', 'Comprehensive guide for using Rosheta prescription management system.');
    };

    window.showDocumentation = function() {
        showInfoModal('Documentation', 'Technical documentation and API references.');
    };

    window.contactSupport = function() {
        showInfoModal('Contact Support', 'Reach out to our support team at support@rosheta.com or call +1 (555) 123-4567.');
    };

    window.showKeyboardShortcuts = function() {
        const shortcuts = `
            <div class="row g-3">
                <div class="col-md-6">
                    <h6>Navigation</h6>
                    <ul class="list-unstyled small">
                        <li><kbd>Ctrl</kbd> + <kbd>D</kbd> - Dashboard</li>
                        <li><kbd>Ctrl</kbd> + <kbd>P</kbd> - New Patient</li>
                        <li><kbd>Ctrl</kbd> + <kbd>R</kbd> - New Prescription</li>
                        <li><kbd>Ctrl</kbd> + <kbd>M</kbd> - Medications</li>
                    </ul>
                </div>
                <div class="col-md-6">
                    <h6>Actions</h6>
                    <ul class="list-unstyled small">
                        <li><kbd>Ctrl</kbd> + <kbd>S</kbd> - Save</li>
                        <li><kbd>Ctrl</kbd> + <kbd>F</kbd> - Search</li>
                        <li><kbd>Esc</kbd> - Close Modal</li>
                        <li><kbd>?</kbd> - Show Shortcuts</li>
                    </ul>
                </div>
            </div>
        `;
        showInfoModal('Keyboard Shortcuts', shortcuts);
    };

    window.showSystemStatus = function() {
        showStatusDetails('System Status');
    };

    window.showQuickActions = function() {
        showInfoModal('Quick Actions', 'Quick action menu for common tasks will be implemented here.');
    };

    window.showSystemInfo = function() {
        const info = `
            <div class="system-info-grid">
                <div class="row g-3">
                    <div class="col-md-6">
                        <div class="card border-0 bg-light">
                            <div class="card-body text-center">
                                <i class="bi bi-code-square text-primary fs-3"></i>
                                <h6 class="mt-2">Version</h6>
                                <p class="mb-0">1.0.0</p>
                            </div>
                        </div>
                    </div>
                    <div class="col-md-6">
                        <div class="card border-0 bg-light">
                            <div class="card-body text-center">
                                <i class="bi bi-calendar3 text-primary fs-3"></i>
                                <h6 class="mt-2">Build Date</h6>
                                <p class="mb-0">${new Date().toLocaleDateString()}</p>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        `;
        showInfoModal('System Information', info);
    };

    window.showPrivacyPolicy = function() {
        showInfoModal('Privacy Policy', 'Privacy policy and data protection information.');
    };

    window.showTermsOfService = function() {
        showInfoModal('Terms of Service', 'Terms of service and usage agreement.');
    };

    window.showLicenseAgreement = function() {
        showInfoModal('License Agreement', 'Software license agreement and usage terms.');
    };

    // Helper function to show information modals
    function showInfoModal(title, content) {
        const modal = document.createElement('div');
        modal.className = 'modal fade';
        modal.innerHTML = `
            <div class="modal-dialog modal-lg">
                <div class="modal-content">
                    <div class="modal-header">
                        <h5 class="modal-title">${title}</h5>
                        <button type="button" class="btn-close" data-bs-dismiss="modal"></button>
                    </div>
                    <div class="modal-body">
                        ${content}
                    </div>
                    <div class="modal-footer">
                        <button type="button" class="btn btn-secondary" data-bs-dismiss="modal">Close</button>
                    </div>
                </div>
            </div>
        `;
        
        document.body.appendChild(modal);
        const bsModal = new bootstrap.Modal(modal);
        bsModal.show();
        
        modal.addEventListener('hidden.bs.modal', function() {
            modal.remove();
        });
    }

    // Keyboard shortcuts handler
    document.addEventListener('keydown', function(e) {
        if (e.ctrlKey || e.metaKey) {
            switch(e.key.toLowerCase()) {
                case 'd':
                    e.preventDefault();
                    window.location.href = '/';
                    break;
                case 'p':
                    e.preventDefault();
                    window.location.href = '/Patients/Create';
                    break;
                case 'r':
                    e.preventDefault();
                    window.location.href = '/Prescriptions/Create';
                    break;
                case 'm':
                    e.preventDefault();
                    window.location.href = '/Medications/Index';
                    break;
                case 'f':
                    e.preventDefault();
                    const searchInput = document.querySelector('.header-search input');
                    if (searchInput) {
                        searchInput.focus();
                    }
                    break;
            }
        } else if (e.key === '?' && !e.target.matches('input, textarea')) {
            e.preventDefault();
            showKeyboardShortcuts();
        }
    });

})();