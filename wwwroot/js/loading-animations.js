/**
 * Loading States and Animations for Rosheta Application
 * Provides skeleton loaders, smooth transitions, and loading indicators
 */

class LoadingManager {
    constructor() {
        this.activeLoaders = new Map();
        this.animationDuration = 300;
        this.init();
    }
    
    init() {
        this.setupGlobalLoadingOverlay();
        this.setupFormLoadingStates();
        this.setupPageTransitions();
        this.observeContentLoading();
    }
    
    // Global Loading Overlay
    setupGlobalLoadingOverlay() {
        if (!document.getElementById('globalLoadingOverlay')) {
            const overlay = document.createElement('div');
            overlay.id = 'globalLoadingOverlay';
            overlay.className = 'loading-overlay';
            overlay.style.display = 'none';
            overlay.innerHTML = `
                <div class="loading-spinner">
                    <div class="spinner-border text-primary" role="status">
                        <span class="visually-hidden">Loading...</span>
                    </div>
                    <p class="mt-2 text-white" id="loadingMessage">Loading...</p>
                </div>
            `;
            document.body.appendChild(overlay);
        }
    }
    
    showGlobalLoading(message = 'Loading...') {
        const overlay = document.getElementById('globalLoadingOverlay');
        const messageElement = document.getElementById('loadingMessage');
        
        if (overlay && messageElement) {
            messageElement.textContent = message;
            overlay.style.display = 'flex';
            overlay.style.opacity = '0';
            
            // Fade in
            requestAnimationFrame(() => {
                overlay.style.transition = `opacity ${this.animationDuration}ms ease-in-out`;
                overlay.style.opacity = '1';
            });
        }
    }
    
    hideGlobalLoading() {
        const overlay = document.getElementById('globalLoadingOverlay');
        
        if (overlay) {
            overlay.style.opacity = '0';
            setTimeout(() => {
                overlay.style.display = 'none';
            }, this.animationDuration);
        }
    }
    
    // Skeleton Loaders
    createSkeletonLoader(container, config = {}) {
        const {
            lines = 3,
            height = '1rem',
            spacing = '0.5rem',
            animate = true,
            className = 'loading-skeleton'
        } = config;
        
        const skeleton = document.createElement('div');
        skeleton.className = `skeleton-container ${className}`;
        
        for (let i = 0; i < lines; i++) {
            const line = document.createElement('div');
            line.className = `skeleton-text ${animate ? 'loading-skeleton' : ''}`;
            line.style.height = height;
            line.style.marginBottom = spacing;
            
            // Vary line widths for more realistic appearance
            if (i === lines - 1) {
                line.classList.add('narrow');
            } else if (i % 2 === 0) {
                line.classList.add('wide');
            } else {
                line.classList.add('medium');
            }
            
            skeleton.appendChild(line);
        }
        
        if (typeof container === 'string') {
            container = document.querySelector(container);
        }
        
        if (container) {
            const loaderId = this.generateLoaderId();
            skeleton.setAttribute('data-loader-id', loaderId);
            container.appendChild(skeleton);
            this.activeLoaders.set(loaderId, skeleton);
            return loaderId;
        }
        
        return null;
    }
    
    createCardSkeleton(container) {
        return this.createSkeletonLoader(container, {
            lines: 4,
            height: '1.25rem',
            spacing: '0.75rem',
            className: 'card-skeleton'
        });
    }
    
    createTableSkeleton(container, rows = 5, columns = 4) {
        const table = document.createElement('div');
        table.className = 'skeleton-table loading-skeleton';
        
        // Header row
        const headerRow = document.createElement('div');
        headerRow.className = 'skeleton-table-row skeleton-table-header';
        for (let i = 0; i < columns; i++) {
            const cell = document.createElement('div');
            cell.className = 'skeleton-text skeleton-table-cell loading-skeleton';
            headerRow.appendChild(cell);
        }
        table.appendChild(headerRow);
        
        // Data rows
        for (let r = 0; r < rows; r++) {
            const row = document.createElement('div');
            row.className = 'skeleton-table-row';
            
            for (let c = 0; c < columns; c++) {
                const cell = document.createElement('div');
                cell.className = 'skeleton-text skeleton-table-cell loading-skeleton';
                
                // Vary cell content appearance
                if (c === 0) {
                    cell.classList.add('wide');
                } else if (c === columns - 1) {
                    cell.classList.add('narrow');
                } else {
                    cell.classList.add('medium');
                }
                
                row.appendChild(cell);
            }
            table.appendChild(row);
        }
        
        if (typeof container === 'string') {
            container = document.querySelector(container);
        }
        
        if (container) {
            const loaderId = this.generateLoaderId();
            table.setAttribute('data-loader-id', loaderId);
            container.appendChild(table);
            this.activeLoaders.set(loaderId, table);
            return loaderId;
        }
        
        return null;
    }
    
    removeSkeleton(loaderId) {
        const skeleton = this.activeLoaders.get(loaderId);
        if (skeleton) {
            skeleton.style.opacity = '0';
            skeleton.style.transition = `opacity ${this.animationDuration}ms ease-out`;
            
            setTimeout(() => {
                if (skeleton.parentNode) {
                    skeleton.parentNode.removeChild(skeleton);
                }
                this.activeLoaders.delete(loaderId);
            }, this.animationDuration);
        }
    }
    
    // Button Loading States
    setButtonLoading(button, loadingText = 'Loading...') {
        if (typeof button === 'string') {
            button = document.querySelector(button);
        }
        
        if (!button) return;
        
        const originalText = button.innerHTML;
        const originalDisabled = button.disabled;
        
        button.setAttribute('data-original-text', originalText);
        button.setAttribute('data-original-disabled', originalDisabled);
        button.disabled = true;
        button.classList.add('btn-loading');
        
        if (button.tagName === 'BUTTON') {
            button.innerHTML = `
                <span class="spinner-border spinner-border-sm me-2" role="status" aria-hidden="true"></span>
                ${loadingText}
            `;
        }
    }
    
    removeButtonLoading(button) {
        if (typeof button === 'string') {
            button = document.querySelector(button);
        }
        
        if (!button) return;
        
        const originalText = button.getAttribute('data-original-text');
        const originalDisabled = button.getAttribute('data-original-disabled') === 'true';
        
        button.innerHTML = originalText || button.innerHTML;
        button.disabled = originalDisabled;
        button.classList.remove('btn-loading');
        
        button.removeAttribute('data-original-text');
        button.removeAttribute('data-original-disabled');
    }
    
    // Form Loading States
    setupFormLoadingStates() {
        document.addEventListener('submit', (e) => {
            const form = e.target;
            if (form.tagName === 'FORM' && !form.hasAttribute('data-no-loading')) {
                this.setFormLoading(form);
            }
        });
    }
    
    setFormLoading(form) {
        if (typeof form === 'string') {
            form = document.querySelector(form);
        }
        
        if (!form) return;
        
        const submitButtons = form.querySelectorAll('button[type="submit"], input[type="submit"]');
        submitButtons.forEach(button => {
            this.setButtonLoading(button, 'Processing...');
        });
        
        // Disable all form inputs
        const inputs = form.querySelectorAll('input, select, textarea, button');
        inputs.forEach(input => {
            if (!input.hasAttribute('data-original-disabled')) {
                input.setAttribute('data-original-disabled', input.disabled);
            }
            if (input.type !== 'submit') {
                input.disabled = true;
            }
        });
        
        form.classList.add('form-loading');
    }
    
    removeFormLoading(form) {
        if (typeof form === 'string') {
            form = document.querySelector(form);
        }
        
        if (!form) return;
        
        const submitButtons = form.querySelectorAll('button[type="submit"], input[type="submit"]');
        submitButtons.forEach(button => {
            this.removeButtonLoading(button);
        });
        
        // Re-enable form inputs
        const inputs = form.querySelectorAll('input, select, textarea, button');
        inputs.forEach(input => {
            const originalDisabled = input.getAttribute('data-original-disabled') === 'true';
            input.disabled = originalDisabled;
            input.removeAttribute('data-original-disabled');
        });
        
        form.classList.remove('form-loading');
    }
    
    // Progressive Content Loading
    observeContentLoading() {
        // Intersection Observer for lazy loading animations
        const observer = new IntersectionObserver((entries) => {
            entries.forEach(entry => {
                if (entry.isIntersecting) {
                    this.animateContentIn(entry.target);
                    observer.unobserve(entry.target);
                }
            });
        }, {
            threshold: 0.1,
            rootMargin: '0px 0px -50px 0px'
        });
        
        // Observe elements with animation classes
        document.querySelectorAll('.animate-on-scroll, .card-fade-in').forEach(el => {
            observer.observe(el);
        });
        
        // Re-observe when new content is added
        this.observeNewContent = (container) => {
            if (typeof container === 'string') {
                container = document.querySelector(container);
            }
            
            if (container) {
                container.querySelectorAll('.animate-on-scroll, .card-fade-in').forEach(el => {
                    observer.observe(el);
                });
            }
        };
    }
    
    animateContentIn(element) {
        element.style.opacity = '0';
        element.style.transform = 'translateY(30px)';
        element.style.transition = `opacity ${this.animationDuration}ms ease-out, transform ${this.animationDuration}ms ease-out`;
        
        requestAnimationFrame(() => {
            element.style.opacity = '1';
            element.style.transform = 'translateY(0)';
        });
    }
    
    // Page Transitions
    setupPageTransitions() {
        // Smooth page exit animations
        document.addEventListener('beforeunload', () => {
            document.body.style.opacity = '0.5';
            document.body.style.transition = 'opacity 200ms ease-out';
        });
        
        // Page enter animations
        document.addEventListener('DOMContentLoaded', () => {
            this.animatePageIn();
        });
    }
    
    animatePageIn() {
        const main = document.querySelector('main') || document.body;
        main.style.opacity = '0';
        main.style.transform = 'translateY(20px)';
        
        requestAnimationFrame(() => {
            main.style.transition = `opacity ${this.animationDuration}ms ease-out, transform ${this.animationDuration}ms ease-out`;
            main.style.opacity = '1';
            main.style.transform = 'translateY(0)';
        });
        
        // Animate cards with staggered delay
        const cards = document.querySelectorAll('.card');
        cards.forEach((card, index) => {
            card.style.opacity = '0';
            card.style.transform = 'translateY(20px)';
            
            setTimeout(() => {
                card.style.transition = `opacity ${this.animationDuration}ms ease-out, transform ${this.animationDuration}ms ease-out`;
                card.style.opacity = '1';
                card.style.transform = 'translateY(0)';
            }, index * 50);
        });
    }
    
    // AJAX Loading States
    wrapAjaxCall(ajaxFunction, options = {}) {
        const {
            showGlobalLoading = false,
            loadingMessage = 'Loading...',
            successMessage = null,
            errorMessage = 'An error occurred',
            onStart = null,
            onSuccess = null,
            onError = null,
            onComplete = null
        } = options;
        
        return async (...args) => {
            try {
                if (showGlobalLoading) {
                    this.showGlobalLoading(loadingMessage);
                }
                
                if (onStart) onStart();
                
                const result = await ajaxFunction(...args);
                
                if (successMessage) {
                    this.showToast(successMessage, 'success');
                }
                
                if (onSuccess) onSuccess(result);
                
                return result;
                
            } catch (error) {
                console.error('AJAX Error:', error);
                
                this.showToast(errorMessage, 'error');
                
                if (onError) onError(error);
                
                throw error;
                
            } finally {
                if (showGlobalLoading) {
                    this.hideGlobalLoading();
                }
                
                if (onComplete) onComplete();
            }
        };
    }
    
    // Micro-animations
    pulseElement(element, duration = 600) {
        if (typeof element === 'string') {
            element = document.querySelector(element);
        }
        
        if (!element) return;
        
        element.style.animation = `pulse ${duration}ms ease-in-out`;
        
        setTimeout(() => {
            element.style.animation = '';
        }, duration);
    }
    
    shakeElement(element, duration = 600) {
        if (typeof element === 'string') {
            element = document.querySelector(element);
        }
        
        if (!element) return;
        
        element.style.animation = `shake ${duration}ms ease-in-out`;
        
        setTimeout(() => {
            element.style.animation = '';
        }, duration);
    }
    
    bounceElement(element, duration = 600) {
        if (typeof element === 'string') {
            element = document.querySelector(element);
        }
        
        if (!element) return;
        
        element.style.animation = `bounce ${duration}ms ease-in-out`;
        
        setTimeout(() => {
            element.style.animation = '';
        }, duration);
    }
    
    // Utility methods
    generateLoaderId() {
        return 'loader_' + Math.random().toString(36).substr(2, 9);
    }
    
    showToast(message, type = 'info') {
        // Integrate with existing toast system
        if (typeof showToast === 'function') {
            showToast(message, type);
        } else {
            console.log(`${type.toUpperCase()}: ${message}`);
        }
    }
    
    // Public API
    static getInstance() {
        if (!LoadingManager.instance) {
            LoadingManager.instance = new LoadingManager();
        }
        return LoadingManager.instance;
    }
}

// CSS animations (injected via JavaScript to avoid external CSS dependencies)
const animationStyles = `
@keyframes pulse {
    0% { transform: scale(1); }
    50% { transform: scale(1.05); }
    100% { transform: scale(1); }
}

@keyframes shake {
    0%, 100% { transform: translateX(0); }
    10%, 30%, 50%, 70%, 90% { transform: translateX(-2px); }
    20%, 40%, 60%, 80% { transform: translateX(2px); }
}

@keyframes bounce {
    0%, 20%, 53%, 80%, 100% { transform: translateY(0); }
    40%, 43% { transform: translateY(-8px); }
    70% { transform: translateY(-4px); }
    90% { transform: translateY(-2px); }
}

.skeleton-table {
    display: table;
    width: 100%;
    border-spacing: 0;
}

.skeleton-table-row {
    display: table-row;
}

.skeleton-table-cell {
    display: table-cell;
    padding: 0.75rem;
    border-bottom: 1px solid #e9ecef;
}

.skeleton-table-header .skeleton-table-cell {
    background-color: #f8f9fa;
    border-bottom: 2px solid #dee2e6;
}

.form-loading {
    position: relative;
    opacity: 0.7;
    pointer-events: none;
}

.animate-on-scroll {
    opacity: 0;
    transform: translateY(30px);
    transition: opacity 0.6s ease-out, transform 0.6s ease-out;
}

.animate-on-scroll.animated {
    opacity: 1;
    transform: translateY(0);
}
`;

// Inject styles
const styleSheet = document.createElement('style');
styleSheet.textContent = animationStyles;
document.head.appendChild(styleSheet);

// Initialize global instance
document.addEventListener('DOMContentLoaded', () => {
    window.loadingManager = LoadingManager.getInstance();
});

// Export for manual usage
window.LoadingManager = LoadingManager;

// Convenience functions
window.showGlobalLoading = (message) => LoadingManager.getInstance().showGlobalLoading(message);
window.hideGlobalLoading = () => LoadingManager.getInstance().hideGlobalLoading();
window.setButtonLoading = (button, text) => LoadingManager.getInstance().setButtonLoading(button, text);
window.removeButtonLoading = (button) => LoadingManager.getInstance().removeButtonLoading(button);
window.createSkeleton = (container, config) => LoadingManager.getInstance().createSkeletonLoader(container, config);
window.removeSkeleton = (loaderId) => LoadingManager.getInstance().removeSkeleton(loaderId);