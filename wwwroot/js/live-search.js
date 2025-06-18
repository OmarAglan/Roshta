/**
 * Live Search with Autocomplete and Debouncing
 * Provides a reusable search component for Rosheta prescription management system
 */

class LiveSearch {
    constructor(options) {
        this.searchInput = options.searchInput;
        this.dropdownContainer = options.dropdownContainer;
        this.endpoint = options.endpoint;
        this.displayProperty = options.displayProperty || 'name';
        this.secondaryProperty = options.secondaryProperty || null;
        this.onSelect = options.onSelect || this.defaultOnSelect.bind(this);
        this.debounceMs = options.debounceMs || 300;
        this.minChars = options.minChars || 2;
        
        this.debounceTimer = null;
        this.currentResults = [];
        this.selectedIndex = -1;
        this.isDropdownVisible = false;
        
        this.init();
    }
    
    init() {
        if (!this.searchInput || !this.dropdownContainer) {
            console.warn('LiveSearch: Required elements not found');
            return;
        }
        
        this.setupEventListeners();
        this.setupAriaAttributes();
    }
    
    setupEventListeners() {
        // Input events
        this.searchInput.addEventListener('input', this.handleInput.bind(this));
        this.searchInput.addEventListener('keydown', this.handleKeydown.bind(this));
        this.searchInput.addEventListener('focus', this.handleFocus.bind(this));
        this.searchInput.addEventListener('blur', this.handleBlur.bind(this));
        
        // Click outside to close
        document.addEventListener('click', this.handleDocumentClick.bind(this));
    }
    
    setupAriaAttributes() {
        this.searchInput.setAttribute('role', 'combobox');
        this.searchInput.setAttribute('aria-expanded', 'false');
        this.searchInput.setAttribute('aria-autocomplete', 'list');
        this.searchInput.setAttribute('aria-haspopup', 'listbox');
        
        this.dropdownContainer.setAttribute('role', 'listbox');
        this.dropdownContainer.setAttribute('aria-hidden', 'true');
    }
    
    handleInput(event) {
        const query = event.target.value.trim();
        
        if (query.length < this.minChars) {
            this.hideDropdown();
            return;
        }
        
        // Clear previous timer
        if (this.debounceTimer) {
            clearTimeout(this.debounceTimer);
        }
        
        // Show loading state immediately
        this.showLoading();
        
        // Debounce the search
        this.debounceTimer = setTimeout(() => {
            this.performSearch(query);
        }, this.debounceMs);
    }
    
    handleKeydown(event) {
        if (!this.isDropdownVisible || this.currentResults.length === 0) {
            return;
        }
        
        switch (event.key) {
            case 'ArrowDown':
                event.preventDefault();
                this.navigateDown();
                break;
            case 'ArrowUp':
                event.preventDefault();
                this.navigateUp();
                break;
            case 'Enter':
                event.preventDefault();
                if (this.selectedIndex >= 0) {
                    this.selectResult(this.currentResults[this.selectedIndex]);
                }
                break;
            case 'Escape':
                event.preventDefault();
                this.hideDropdown();
                break;
        }
    }
    
    handleFocus() {
        const query = this.searchInput.value.trim();
        if (query.length >= this.minChars && this.currentResults.length > 0) {
            this.showDropdown();
        }
    }
    
    handleBlur(event) {
        // Delay hiding to allow for clicks on dropdown items
        setTimeout(() => {
            if (!this.dropdownContainer.contains(document.activeElement)) {
                this.hideDropdown();
            }
        }, 150);
    }
    
    handleDocumentClick(event) {
        if (!this.searchInput.contains(event.target) && 
            !this.dropdownContainer.contains(event.target)) {
            this.hideDropdown();
        }
    }
    
    async performSearch(query) {
        try {
            const url = `${this.endpoint}&searchTerm=${encodeURIComponent(query)}`;
            const response = await fetch(url, {
                method: 'GET',
                headers: {
                    'Accept': 'application/json',
                    'X-Requested-With': 'XMLHttpRequest'
                }
            });
            
            if (!response.ok) {
                throw new Error(`HTTP error! status: ${response.status}`);
            }
            
            const results = await response.json();
            this.displayResults(results);
            
        } catch (error) {
            console.error('Search error:', error);
            this.showError('Failed to perform search. Please try again.');
        }
    }
    
    displayResults(results) {
        this.currentResults = results || [];
        this.selectedIndex = -1;
        
        if (this.currentResults.length === 0) {
            this.showNoResults();
            return;
        }
        
        const html = this.currentResults.map((result, index) => {
            const primaryText = this.getPropertyValue(result, this.displayProperty);
            const secondaryText = this.secondaryProperty ? 
                this.getPropertyValue(result, this.secondaryProperty) : '';
            
            return `
                <div class="live-search-item" 
                     data-index="${index}"
                     role="option"
                     aria-selected="false">
                    <div class="live-search-item-primary">${this.escapeHtml(primaryText)}</div>
                    ${secondaryText ? `<div class="live-search-item-secondary">${this.escapeHtml(secondaryText)}</div>` : ''}
                </div>
            `;
        }).join('');
        
        this.dropdownContainer.innerHTML = html;
        this.setupResultClickHandlers();
        this.showDropdown();
    }
    
    setupResultClickHandlers() {
        const items = this.dropdownContainer.querySelectorAll('.live-search-item');
        items.forEach(item => {
            item.addEventListener('click', (event) => {
                event.preventDefault();
                const index = parseInt(item.dataset.index);
                this.selectResult(this.currentResults[index]);
            });
        });
    }
    
    showLoading() {
        this.dropdownContainer.innerHTML = `
            <div class="live-search-loading">
                <div class="spinner-border spinner-border-sm me-2" role="status">
                    <span class="visually-hidden">Loading...</span>
                </div>
                Searching...
            </div>
        `;
        this.showDropdown();
    }
    
    showNoResults() {
        this.dropdownContainer.innerHTML = `
            <div class="live-search-no-results">
                <i class="bi bi-search me-2"></i>
                No results found
            </div>
        `;
        this.showDropdown();
    }
    
    showError(message) {
        this.dropdownContainer.innerHTML = `
            <div class="live-search-error">
                <i class="bi bi-exclamation-triangle me-2"></i>
                ${this.escapeHtml(message)}
            </div>
        `;
        this.showDropdown();
    }
    
    showDropdown() {
        this.dropdownContainer.classList.add('show');
        this.dropdownContainer.setAttribute('aria-hidden', 'false');
        this.searchInput.setAttribute('aria-expanded', 'true');
        this.isDropdownVisible = true;
    }
    
    hideDropdown() {
        this.dropdownContainer.classList.remove('show');
        this.dropdownContainer.setAttribute('aria-hidden', 'true');
        this.searchInput.setAttribute('aria-expanded', 'false');
        this.isDropdownVisible = false;
        this.selectedIndex = -1;
        this.updateSelection();
    }
    
    navigateDown() {
        if (this.selectedIndex < this.currentResults.length - 1) {
            this.selectedIndex++;
            this.updateSelection();
        }
    }
    
    navigateUp() {
        if (this.selectedIndex > 0) {
            this.selectedIndex--;
            this.updateSelection();
        } else if (this.selectedIndex === 0) {
            this.selectedIndex = -1;
            this.updateSelection();
        }
    }
    
    updateSelection() {
        const items = this.dropdownContainer.querySelectorAll('.live-search-item');
        items.forEach((item, index) => {
            const isSelected = index === this.selectedIndex;
            item.classList.toggle('selected', isSelected);
            item.setAttribute('aria-selected', isSelected.toString());
        });
    }
    
    selectResult(result) {
        this.onSelect(result);
        this.hideDropdown();
    }
    
    defaultOnSelect(result) {
        const displayValue = this.getPropertyValue(result, this.displayProperty);
        this.searchInput.value = displayValue;
    }
    
    getPropertyValue(obj, property) {
        return property.split('.').reduce((o, p) => o && o[p], obj) || '';
    }
    
    escapeHtml(text) {
        const div = document.createElement('div');
        div.textContent = text;
        return div.innerHTML;
    }
    
    // Public methods
    clear() {
        this.searchInput.value = '';
        this.hideDropdown();
        this.currentResults = [];
    }
    
    destroy() {
        if (this.debounceTimer) {
            clearTimeout(this.debounceTimer);
        }
        
        // Remove event listeners
        this.searchInput.removeEventListener('input', this.handleInput);
        this.searchInput.removeEventListener('keydown', this.handleKeydown);
        this.searchInput.removeEventListener('focus', this.handleFocus);
        this.searchInput.removeEventListener('blur', this.handleBlur);
        document.removeEventListener('click', this.handleDocumentClick);
    }
}

// Utility function to create debounced functions
function debounce(func, wait) {
    let timeout;
    return function executedFunction(...args) {
        const later = () => {
            clearTimeout(timeout);
            func(...args);
        };
        clearTimeout(timeout);
        timeout = setTimeout(later, wait);
    };
}

// Auto-initialization for search inputs with data attributes
document.addEventListener('DOMContentLoaded', function() {
    const searchInputs = document.querySelectorAll('[data-live-search]');
    
    searchInputs.forEach(input => {
        const endpoint = input.dataset.liveSearch;
        const dropdownId = input.dataset.liveSearchDropdown;
        const displayProperty = input.dataset.liveSearchDisplay || 'name';
        const secondaryProperty = input.dataset.liveSearchSecondary || null;
        
        const dropdownContainer = document.getElementById(dropdownId);
        
        if (dropdownContainer) {
            new LiveSearch({
                searchInput: input,
                dropdownContainer: dropdownContainer,
                endpoint: endpoint,
                displayProperty: displayProperty,
                secondaryProperty: secondaryProperty
            });
        }
    });
});

// Export for module usage
if (typeof module !== 'undefined' && module.exports) {
    module.exports = { LiveSearch, debounce };
}