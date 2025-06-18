// Settings-specific JavaScript functionality
document.addEventListener('DOMContentLoaded', function () {
    
    // Tab persistence functionality
    function initializeTabPersistence() {
        const tabTriggerList = [].slice.call(document.querySelectorAll('#profileTabs button[data-bs-toggle="tab"]'));
        
        tabTriggerList.forEach(function (tabTriggerEl) {
            tabTriggerEl.addEventListener('click', function (event) {
                // Store active tab in localStorage
                const tabId = event.target.getAttribute('aria-controls');
                if (tabId) {
                    localStorage.setItem('activeProfileTab', tabId);
                }
            });
        });

        // Restore active tab from localStorage
        const activeTab = localStorage.getItem('activeProfileTab');
        if (activeTab) {
            const tabButton = document.querySelector(`#profileTabs button[aria-controls="${activeTab}"]`);
            if (tabButton) {
                const tab = new bootstrap.Tab(tabButton);
                tab.show();
            }
        }
    }

    // Form validation for settings forms
    function initializeSettingsValidation() {
        const settingsForms = document.querySelectorAll('form[asp-page-handler="SaveSettings"]');
        
        settingsForms.forEach(function(form) {
            form.addEventListener('submit', function(e) {
                const submitButton = form.querySelector('button[type="submit"]');
                if (submitButton) {
                    // Add loading state
                    submitButton.classList.add('loading');
                    submitButton.disabled = true;
                    
                    // Remove loading state after form submission
                    setTimeout(() => {
                        submitButton.classList.remove('loading');
                        submitButton.disabled = false;
                    }, 2000);
                }
            });
        });
    }

    // Live preview for format settings
    function initializeLivePreview() {
        const dateFormatSelect = document.getElementById('UserSettings_DateFormat');
        const timeFormatSelect = document.getElementById('UserSettings_TimeFormat');
        
        if (dateFormatSelect) {
            updateDatePreview(dateFormatSelect.value);
            dateFormatSelect.addEventListener('change', function() {
                updateDatePreview(this.value);
            });
        }
        
        if (timeFormatSelect) {
            updateTimePreview(timeFormatSelect.value);
            timeFormatSelect.addEventListener('change', function() {
                updateTimePreview(this.value);
            });
        }
    }

    function updateDatePreview(format) {
        const now = new Date();
        let preview = '';
        
        switch(format) {
            case 'dd/MM/yyyy':
                preview = `${now.getDate().toString().padStart(2, '0')}/${(now.getMonth() + 1).toString().padStart(2, '0')}/${now.getFullYear()}`;
                break;
            case 'MM/dd/yyyy':
                preview = `${(now.getMonth() + 1).toString().padStart(2, '0')}/${now.getDate().toString().padStart(2, '0')}/${now.getFullYear()}`;
                break;
            case 'yyyy-MM-dd':
                preview = `${now.getFullYear()}-${(now.getMonth() + 1).toString().padStart(2, '0')}-${now.getDate().toString().padStart(2, '0')}`;
                break;
            case 'dd-MM-yyyy':
                preview = `${now.getDate().toString().padStart(2, '0')}-${(now.getMonth() + 1).toString().padStart(2, '0')}-${now.getFullYear()}`;
                break;
        }
        
        updatePreviewText('datePreview', preview);
    }

    function updateTimePreview(format) {
        const now = new Date();
        let preview = '';
        
        if (format === 'HH:mm') {
            preview = `${now.getHours().toString().padStart(2, '0')}:${now.getMinutes().toString().padStart(2, '0')}`;
        } else if (format === 'hh:mm tt') {
            const hours = now.getHours();
            const minutes = now.getMinutes();
            const ampm = hours >= 12 ? 'PM' : 'AM';
            const displayHours = hours % 12 || 12;
            preview = `${displayHours.toString().padStart(2, '0')}:${minutes.toString().padStart(2, '0')} ${ampm}`;
        }
        
        updatePreviewText('timePreview', preview);
    }

    function updatePreviewText(elementId, text) {
        let previewElement = document.getElementById(elementId);
        if (!previewElement) {
            // Create preview element if it doesn't exist
            const selectElement = document.getElementById(elementId.replace('Preview', ''));
            if (selectElement) {
                previewElement = document.createElement('small');
                previewElement.id = elementId;
                previewElement.className = 'form-text text-primary';
                selectElement.parentNode.appendChild(previewElement);
            }
        }
        
        if (previewElement) {
            previewElement.textContent = `Preview: ${text}`;
        }
    }

    // Notification settings preview
    function initializeNotificationPreview() {
        const enableSuccess = document.getElementById('UserSettings_EnableSuccessNotifications');
        const enableWarning = document.getElementById('UserSettings_EnableWarningNotifications');
        const enableError = document.getElementById('UserSettings_EnableErrorNotifications');
        const autoHide = document.getElementById('UserSettings_AutoHideSuccessMessages');
        const duration = document.getElementById('UserSettings_NotificationDuration');

        if (enableSuccess) {
            enableSuccess.addEventListener('change', updateNotificationPreview);
        }
        if (enableWarning) {
            enableWarning.addEventListener('change', updateNotificationPreview);
        }
        if (enableError) {
            enableError.addEventListener('change', updateNotificationPreview);
        }
        if (autoHide) {
            autoHide.addEventListener('change', updateNotificationPreview);
        }
        if (duration) {
            duration.addEventListener('input', updateNotificationPreview);
        }

        // Initial preview
        updateNotificationPreview();
    }

    function updateNotificationPreview() {
        const enableSuccess = document.getElementById('UserSettings_EnableSuccessNotifications')?.checked || false;
        const enableWarning = document.getElementById('UserSettings_EnableWarningNotifications')?.checked || false;
        const enableError = document.getElementById('UserSettings_EnableErrorNotifications')?.checked || false;
        const autoHide = document.getElementById('UserSettings_AutoHideSuccessMessages')?.checked || false;
        const duration = document.getElementById('UserSettings_NotificationDuration')?.value || 5;

        let previewText = 'Enabled: ';
        const enabled = [];
        
        if (enableSuccess) enabled.push('Success');
        if (enableWarning) enabled.push('Warning');
        if (enableError) enabled.push('Error');
        
        if (enabled.length === 0) {
            previewText = 'No notifications will be shown';
        } else {
            previewText += enabled.join(', ');
            if (autoHide && enableSuccess) {
                previewText += ` | Auto-hide after ${duration}s`;
            }
        }

        updatePreviewText('notificationPreview', previewText);
    }

    // Theme preview (placeholder for future implementation)
    function initializeThemePreview() {
        const themeSelect = document.getElementById('UserSettings_ThemePreference');
        if (themeSelect) {
            themeSelect.addEventListener('change', function() {
                const theme = this.value;
                let previewText = '';
                
                switch(theme) {
                    case 'light':
                        previewText = 'Light theme - bright background with dark text';
                        break;
                    case 'dark':
                        previewText = 'Dark theme - dark background with light text (coming soon)';
                        break;
                    case 'auto':
                        previewText = 'Auto theme - follows your system preference (coming soon)';
                        break;
                }
                
                updatePreviewText('themePreview', previewText);
            });
            
            // Initial preview
            const event = new Event('change');
            themeSelect.dispatchEvent(event);
        }
    }

    // Initialize all settings functionality
    if (document.getElementById('profileTabs')) {
        initializeTabPersistence();
        initializeSettingsValidation();
        initializeLivePreview();
        initializeNotificationPreview();
        initializeThemePreview();
    }
});

// Export functions for potential external use
window.SettingsJS = {
    updateDatePreview: function(format) {
        // Function available globally if needed
    },
    updateTimePreview: function(format) {
        // Function available globally if needed
    }
};