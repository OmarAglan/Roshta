/**
 * Advanced Table Features for Rosheta Application
 * Includes bulk actions, advanced filtering, sorting, and export functionality
 */

class AdvancedTable {
    constructor(tableSelector, options = {}) {
        this.table = document.querySelector(tableSelector);
        this.options = {
            enableBulkActions: true,
            enableAdvancedFilter: true,
            enableExport: true,
            enableColumnSorting: true,
            pageSize: 10,
            ...options
        };
        
        this.selectedRows = new Set();
        this.currentFilters = {};
        this.currentSort = { column: null, direction: 'asc' };
        
        this.init();
    }
    
    init() {
        if (!this.table) return;
        
        this.setupBulkActions();
        this.setupAdvancedFilters();
        this.setupColumnSorting();
        this.setupExportFeatures();
        this.setupKeyboardNavigation();
        this.setupAccessibility();
    }
    
    setupBulkActions() {
        if (!this.options.enableBulkActions) return;
        
        // Create bulk actions toolbar
        const toolbar = this.createBulkActionsToolbar();
        this.table.parentNode.insertBefore(toolbar, this.table);
        
        // Add checkboxes to table rows
        this.addRowCheckboxes();
        
        // Add master checkbox to header
        this.addMasterCheckbox();
        
        // Setup event listeners
        this.setupBulkActionEvents();
    }
    
    createBulkActionsToolbar() {
        const toolbar = document.createElement('div');
        toolbar.className = 'bulk-actions-toolbar';
        toolbar.innerHTML = `
            <div class="d-flex align-items-center">
                <span class="selected-count me-3">
                    <strong>0</strong> items selected
                </span>
                <div class="btn-group" role="group">
                    <button type="button" class="btn btn-sm btn-outline-primary" data-action="export-selected">
                        <i class="fas fa-download me-1"></i>Export Selected
                    </button>
                    <button type="button" class="btn btn-sm btn-outline-warning" data-action="bulk-edit">
                        <i class="fas fa-edit me-1"></i>Bulk Edit
                    </button>
                    <button type="button" class="btn btn-sm btn-outline-danger" data-action="bulk-delete">
                        <i class="fas fa-trash me-1"></i>Delete Selected
                    </button>
                </div>
            </div>
            <button type="button" class="btn btn-sm btn-outline-secondary" data-action="clear-selection">
                <i class="fas fa-times me-1"></i>Clear Selection
            </button>
        `;
        return toolbar;
    }
    
    addRowCheckboxes() {
        const rows = this.table.querySelectorAll('tbody tr');
        rows.forEach((row, index) => {
            const checkbox = document.createElement('input');
            checkbox.type = 'checkbox';
            checkbox.className = 'form-check-input row-checkbox';
            checkbox.setAttribute('data-row-id', index);
            checkbox.setAttribute('aria-label', `Select row ${index + 1}`);
            
            const cell = document.createElement('td');
            cell.className = 'text-center';
            cell.appendChild(checkbox);
            
            row.insertBefore(cell, row.firstChild);
        });
    }
    
    addMasterCheckbox() {
        const headerRow = this.table.querySelector('thead tr');
        if (!headerRow) return;
        
        const masterCheckbox = document.createElement('input');
        masterCheckbox.type = 'checkbox';
        masterCheckbox.className = 'form-check-input master-checkbox';
        masterCheckbox.setAttribute('aria-label', 'Select all rows');
        
        const headerCell = document.createElement('th');
        headerCell.className = 'text-center';
        headerCell.style.width = '50px';
        headerCell.appendChild(masterCheckbox);
        
        headerRow.insertBefore(headerCell, headerRow.firstChild);
    }
    
    setupBulkActionEvents() {
        // Row checkbox events
        this.table.addEventListener('change', (e) => {
            if (e.target.classList.contains('row-checkbox')) {
                this.handleRowSelection(e.target);
            } else if (e.target.classList.contains('master-checkbox')) {
                this.handleMasterSelection(e.target);
            }
        });
        
        // Bulk action button events
        const toolbar = this.table.parentNode.querySelector('.bulk-actions-toolbar');
        toolbar.addEventListener('click', (e) => {
            const action = e.target.closest('[data-action]')?.getAttribute('data-action');
            if (action) {
                this.handleBulkAction(action);
            }
        });
    }
    
    handleRowSelection(checkbox) {
        const rowId = checkbox.getAttribute('data-row-id');
        
        if (checkbox.checked) {
            this.selectedRows.add(rowId);
        } else {
            this.selectedRows.delete(rowId);
        }
        
        this.updateBulkActionsUI();
        this.updateMasterCheckbox();
    }
    
    handleMasterSelection(masterCheckbox) {
        const rowCheckboxes = this.table.querySelectorAll('.row-checkbox');
        
        if (masterCheckbox.checked) {
            rowCheckboxes.forEach(checkbox => {
                checkbox.checked = true;
                this.selectedRows.add(checkbox.getAttribute('data-row-id'));
            });
        } else {
            rowCheckboxes.forEach(checkbox => {
                checkbox.checked = false;
            });
            this.selectedRows.clear();
        }
        
        this.updateBulkActionsUI();
    }
    
    updateBulkActionsUI() {
        const toolbar = this.table.parentNode.querySelector('.bulk-actions-toolbar');
        const countElement = toolbar.querySelector('.selected-count strong');
        const count = this.selectedRows.size;
        
        countElement.textContent = count;
        
        if (count > 0) {
            toolbar.classList.add('show');
        } else {
            toolbar.classList.remove('show');
        }
    }
    
    updateMasterCheckbox() {
        const masterCheckbox = this.table.querySelector('.master-checkbox');
        const rowCheckboxes = this.table.querySelectorAll('.row-checkbox');
        const checkedBoxes = this.table.querySelectorAll('.row-checkbox:checked');
        
        if (checkedBoxes.length === 0) {
            masterCheckbox.checked = false;
            masterCheckbox.indeterminate = false;
        } else if (checkedBoxes.length === rowCheckboxes.length) {
            masterCheckbox.checked = true;
            masterCheckbox.indeterminate = false;
        } else {
            masterCheckbox.checked = false;
            masterCheckbox.indeterminate = true;
        }
    }
    
    handleBulkAction(action) {
        const selectedCount = this.selectedRows.size;
        
        if (selectedCount === 0) {
            this.showToast('Please select items first', 'warning');
            return;
        }
        
        switch (action) {
            case 'export-selected':
                this.exportSelected();
                break;
            case 'bulk-edit':
                this.bulkEdit();
                break;
            case 'bulk-delete':
                this.bulkDelete();
                break;
            case 'clear-selection':
                this.clearSelection();
                break;
        }
    }
    
    setupAdvancedFilters() {
        if (!this.options.enableAdvancedFilter) return;
        
        const filterPanel = this.createFilterPanel();
        this.table.parentNode.insertBefore(filterPanel, this.table);
        
        this.setupFilterEvents();
    }
    
    createFilterPanel() {
        const panel = document.createElement('div');
        panel.className = 'filter-panel collapsed';
        panel.innerHTML = `
            <div class="d-flex justify-content-between align-items-center">
                <h6 class="mb-0">
                    <button class="filter-toggle" type="button" aria-expanded="false" aria-controls="filterContent">
                        <i class="fas fa-filter me-2"></i>Advanced Filters
                        <i class="fas fa-chevron-down ms-2"></i>
                    </button>
                </h6>
                <div>
                    <button type="button" class="btn btn-sm btn-outline-secondary" data-filter-action="clear">
                        <i class="fas fa-times me-1"></i>Clear All
                    </button>
                    <button type="button" class="btn btn-sm btn-primary" data-filter-action="apply">
                        <i class="fas fa-search me-1"></i>Apply Filters
                    </button>
                </div>
            </div>
            <div class="filter-content" id="filterContent" style="display: none;">
                <div class="row mt-3">
                    <div class="col-md-4 mb-3">
                        <label class="form-label">Search</label>
                        <input type="text" class="form-control" id="filterSearch" placeholder="Search all columns...">
                    </label>
                    <div class="col-md-4 mb-3">
                        <label class="form-label">Date Range</label>
                        <div class="input-group">
                            <input type="date" class="form-control" id="filterDateFrom">
                            <span class="input-group-text">to</span>
                            <input type="date" class="form-control" id="filterDateTo">
                        </div>
                    </div>
                    <div class="col-md-4 mb-3">
                        <label class="form-label">Status</label>
                        <select class="form-select" id="filterStatus">
                            <option value="">All Status</option>
                            <option value="active">Active</option>
                            <option value="inactive">Inactive</option>
                            <option value="pending">Pending</option>
                        </select>
                    </div>
                </div>
            </div>
        `;
        return panel;
    }
    
    setupFilterEvents() {
        const filterPanel = this.table.parentNode.querySelector('.filter-panel');
        
        // Toggle filter panel
        filterPanel.querySelector('.filter-toggle').addEventListener('click', (e) => {
            const isExpanded = e.currentTarget.getAttribute('aria-expanded') === 'true';
            e.currentTarget.setAttribute('aria-expanded', !isExpanded);
            
            const content = filterPanel.querySelector('.filter-content');
            if (isExpanded) {
                content.style.display = 'none';
                filterPanel.classList.add('collapsed');
            } else {
                content.style.display = 'block';
                filterPanel.classList.remove('collapsed');
            }
        });
        
        // Filter actions
        filterPanel.addEventListener('click', (e) => {
            const action = e.target.closest('[data-filter-action]')?.getAttribute('data-filter-action');
            if (action === 'apply') {
                this.applyFilters();
            } else if (action === 'clear') {
                this.clearFilters();
            }
        });
        
        // Real-time search
        const searchInput = filterPanel.querySelector('#filterSearch');
        let searchTimeout;
        searchInput.addEventListener('input', (e) => {
            clearTimeout(searchTimeout);
            searchTimeout = setTimeout(() => {
                this.performSearch(e.target.value);
            }, 300);
        });
    }
    
    setupColumnSorting() {
        if (!this.options.enableColumnSorting) return;
        
        const headers = this.table.querySelectorAll('thead th');
        headers.forEach((header, index) => {
            if (header.classList.contains('sortable') || !header.classList.contains('no-sort')) {
                header.classList.add('sortable');
                header.style.cursor = 'pointer';
                header.setAttribute('tabindex', '0');
                header.setAttribute('role', 'button');
                header.setAttribute('aria-label', `Sort by ${header.textContent.trim()}`);
                
                // Add sort indicator
                const indicator = document.createElement('span');
                indicator.className = 'table-sort-indicator ms-1';
                indicator.innerHTML = '<i class="fas fa-sort"></i>';
                header.appendChild(indicator);
                
                header.addEventListener('click', () => this.sortColumn(index));
                header.addEventListener('keydown', (e) => {
                    if (e.key === 'Enter' || e.key === ' ') {
                        e.preventDefault();
                        this.sortColumn(index);
                    }
                });
            }
        });
    }
    
    sortColumn(columnIndex) {
        const header = this.table.querySelectorAll('thead th')[columnIndex];
        const currentDirection = this.currentSort.column === columnIndex ? this.currentSort.direction : 'asc';
        const newDirection = currentDirection === 'asc' ? 'desc' : 'asc';
        
        this.currentSort = { column: columnIndex, direction: newDirection };
        
        // Update visual indicators
        this.updateSortIndicators(columnIndex, newDirection);
        
        // Perform sort
        this.performSort(columnIndex, newDirection);
    }
    
    updateSortIndicators(activeColumn, direction) {
        const headers = this.table.querySelectorAll('thead th .table-sort-indicator');
        headers.forEach((indicator, index) => {
            if (index === activeColumn) {
                indicator.classList.add('active');
                indicator.innerHTML = `<i class="fas fa-sort-${direction === 'asc' ? 'up' : 'down'}"></i>`;
            } else {
                indicator.classList.remove('active');
                indicator.innerHTML = '<i class="fas fa-sort"></i>';
            }
        });
    }
    
    setupExportFeatures() {
        if (!this.options.enableExport) return;
        
        // Add export button to table header or create export dropdown
        const exportButton = this.createExportButton();
        
        // Find appropriate location for export button
        const tableContainer = this.table.parentNode;
        let headerElement = tableContainer.querySelector('.table-header');
        
        if (!headerElement) {
            headerElement = document.createElement('div');
            headerElement.className = 'table-header d-flex justify-content-between align-items-center mb-3';
            headerElement.innerHTML = '<h5 class="mb-0">Table Data</h5><div class="table-actions"></div>';
            tableContainer.insertBefore(headerElement, tableContainer.firstChild);
        }
        
        headerElement.querySelector('.table-actions').appendChild(exportButton);
    }
    
    createExportButton() {
        const dropdown = document.createElement('div');
        dropdown.className = 'dropdown';
        dropdown.innerHTML = `
            <button class="btn btn-outline-success dropdown-toggle" type="button" data-bs-toggle="dropdown" aria-expanded="false">
                <i class="fas fa-download me-1"></i>Export
            </button>
            <ul class="dropdown-menu">
                <li><a class="dropdown-item" href="#" data-export="csv">
                    <i class="fas fa-file-csv me-2"></i>Export as CSV
                </a></li>
                <li><a class="dropdown-item" href="#" data-export="excel">
                    <i class="fas fa-file-excel me-2"></i>Export as Excel
                </a></li>
                <li><a class="dropdown-item" href="#" data-export="pdf">
                    <i class="fas fa-file-pdf me-2"></i>Export as PDF
                </a></li>
                <li><hr class="dropdown-divider"></li>
                <li><a class="dropdown-item" href="#" data-export="print">
                    <i class="fas fa-print me-2"></i>Print Table
                </a></li>
            </ul>
        `;
        
        dropdown.addEventListener('click', (e) => {
            const exportType = e.target.closest('[data-export]')?.getAttribute('data-export');
            if (exportType) {
                this.exportData(exportType);
            }
        });
        
        return dropdown;
    }
    
    exportData(format) {
        this.showLoading();
        
        setTimeout(() => {
            const data = this.getTableData();
            
            switch (format) {
                case 'csv':
                    this.exportToCSV(data);
                    break;
                case 'excel':
                    this.exportToExcel(data);
                    break;
                case 'pdf':
                    this.exportToPDF(data);
                    break;
                case 'print':
                    this.printTable();
                    break;
            }
            
            this.hideLoading();
            this.showToast(`Table exported as ${format.toUpperCase()}`, 'success');
        }, 1000);
    }
    
    exportToCSV(data) {
        const csv = this.convertToCSV(data);
        const blob = new Blob([csv], { type: 'text/csv' });
        const url = URL.createObjectURL(blob);
        
        const link = document.createElement('a');
        link.href = url;
        link.download = `table-export-${new Date().toISOString().split('T')[0]}.csv`;
        link.click();
        
        URL.revokeObjectURL(url);
    }
    
    convertToCSV(data) {
        const headers = data.headers.map(h => `"${h}"`).join(',');
        const rows = data.rows.map(row => 
            row.map(cell => `"${String(cell).replace(/"/g, '""')}"`).join(',')
        ).join('\n');
        
        return headers + '\n' + rows;
    }
    
    getTableData() {
        const headers = Array.from(this.table.querySelectorAll('thead th'))
            .filter(th => !th.querySelector('.row-checkbox'))
            .map(th => th.textContent.trim().replace(/\s+/g, ' '));
        
        const rows = Array.from(this.table.querySelectorAll('tbody tr')).map(row => 
            Array.from(row.querySelectorAll('td'))
                .filter(td => !td.querySelector('.row-checkbox'))
                .map(td => td.textContent.trim().replace(/\s+/g, ' '))
        );
        
        return { headers, rows };
    }
    
    setupKeyboardNavigation() {
        this.table.addEventListener('keydown', (e) => {
            if (e.ctrlKey || e.metaKey) {
                switch (e.key) {
                    case 'a':
                        e.preventDefault();
                        this.selectAll();
                        break;
                    case 'e':
                        e.preventDefault();
                        this.exportData('csv');
                        break;
                }
            }
        });
    }
    
    setupAccessibility() {
        // Add table caption if not present
        if (!this.table.querySelector('caption')) {
            const caption = document.createElement('caption');
            caption.textContent = 'Data table with sorting, filtering and bulk actions';
            caption.className = 'visually-hidden';
            this.table.insertBefore(caption, this.table.firstChild);
        }
        
        // Add aria-labels to interactive elements
        this.table.setAttribute('role', 'table');
        this.table.setAttribute('aria-label', 'Sortable data table');
    }
    
    // Utility methods
    showLoading() {
        const overlay = document.createElement('div');
        overlay.className = 'table-loading position-absolute d-flex align-items-center justify-content-center';
        overlay.innerHTML = '<div class="spinner-border text-primary" role="status"><span class="visually-hidden">Loading...</span></div>';
        
        this.table.style.position = 'relative';
        this.table.appendChild(overlay);
    }
    
    hideLoading() {
        const overlay = this.table.querySelector('.table-loading');
        if (overlay) {
            overlay.remove();
        }
    }
    
    showToast(message, type = 'info') {
        // Reuse toast system from validation scripts
        if (typeof showToast === 'function') {
            showToast(message, type);
        } else {
            console.log(`${type.toUpperCase()}: ${message}`);
        }
    }
    
    // Placeholder methods for future implementation
    performSearch(query) {
        console.log('Search:', query);
    }
    
    applyFilters() {
        console.log('Apply filters');
    }
    
    clearFilters() {
        console.log('Clear filters');
    }
    
    performSort(column, direction) {
        console.log('Sort:', column, direction);
    }
    
    exportSelected() {
        console.log('Export selected:', Array.from(this.selectedRows));
    }
    
    bulkEdit() {
        console.log('Bulk edit:', Array.from(this.selectedRows));
    }
    
    bulkDelete() {
        if (confirm(`Are you sure you want to delete ${this.selectedRows.size} items?`)) {
            console.log('Bulk delete:', Array.from(this.selectedRows));
        }
    }
    
    clearSelection() {
        this.selectedRows.clear();
        this.table.querySelectorAll('.row-checkbox').forEach(cb => cb.checked = false);
        this.updateBulkActionsUI();
        this.updateMasterCheckbox();
    }
    
    selectAll() {
        this.table.querySelector('.master-checkbox').click();
    }
    
    exportToExcel(data) {
        this.showToast('Excel export requires additional library', 'info');
    }
    
    exportToPDF(data) {
        this.showToast('PDF export requires additional library', 'info');
    }
    
    printTable() {
        window.print();
    }
}

// Auto-initialize tables with data-advanced-table attribute
document.addEventListener('DOMContentLoaded', function() {
    document.querySelectorAll('[data-advanced-table]').forEach(table => {
        new AdvancedTable(`#${table.id}` || table);
    });
});

// Export for manual initialization
window.AdvancedTable = AdvancedTable;