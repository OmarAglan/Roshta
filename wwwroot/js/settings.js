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

    // Enhanced theme functionality with actual theme switching
    function initializeThemePreview() {
        const themeSelect = document.getElementById('UserSettings_ThemePreference');
        if (themeSelect) {
            // Load saved theme or detect system preference
            const savedTheme = localStorage.getItem('theme') || 'auto';
            themeSelect.value = savedTheme;
            applyTheme(savedTheme);
            
            themeSelect.addEventListener('change', function() {
                const theme = this.value;
                applyTheme(theme);
                localStorage.setItem('theme', theme);
                updateThemePreview(theme);
            });
            
            // Initial preview
            updateThemePreview(savedTheme);
            
            // Listen for system theme changes when in auto mode
            if (window.matchMedia) {
                const mediaQuery = window.matchMedia('(prefers-color-scheme: dark)');
                mediaQuery.addEventListener('change', function() {
                    if (themeSelect.value === 'auto') {
                        applyTheme('auto');
                    }
                });
            }
        }
    }

    function updateThemePreview(theme) {
        let previewText = '';
        
        switch(theme) {
            case 'light':
                previewText = 'Light theme - bright background with dark text for optimal readability';
                break;
            case 'dark':
                previewText = 'Dark theme - dark background with light text, easier on the eyes';
                break;
            case 'auto':
                const isDarkMode = window.matchMedia && window.matchMedia('(prefers-color-scheme: dark)').matches;
                previewText = `Auto theme - currently using ${isDarkMode ? 'dark' : 'light'} based on system preference`;
                break;
        }
        
        updatePreviewText('themePreview', previewText);
    }

    function applyTheme(theme) {
        // Add transitioning class to prevent flicker
        document.body.classList.add('theme-transitioning');
        
        // Remove existing theme classes
        document.documentElement.removeAttribute('data-theme');
        
        // Apply new theme
        if (theme === 'auto') {
            // Let CSS handle auto theme via media queries
            document.documentElement.setAttribute('data-theme', 'auto');
        } else {
            document.documentElement.setAttribute('data-theme', theme);
        }
        
        // Remove transitioning class after a brief delay
        setTimeout(() => {
            document.body.classList.remove('theme-transitioning');
        }, 50);
        
        // Update any theme-specific UI elements
        updateThemeSpecificElements(theme);
    }

    function updateThemeSpecificElements(theme) {
        // Update any elements that need special handling for theme changes
        const themeMetaTag = document.querySelector('meta[name="theme-color"]');
        if (themeMetaTag) {
            const isDark = theme === 'dark' || (theme === 'auto' && window.matchMedia('(prefers-color-scheme: dark)').matches);
            themeMetaTag.setAttribute('content', isDark ? '#121212' : '#ffffff');
        }
        
        // Dispatch custom event for other components that might need to react to theme changes
        const themeChangeEvent = new CustomEvent('themeChanged', {
            detail: { theme: theme }
        });
        document.dispatchEvent(themeChangeEvent);
    }

    // Utility function to get current effective theme
    function getCurrentTheme() {
        const savedTheme = localStorage.getItem('theme') || 'auto';
        if (savedTheme === 'auto') {
            return window.matchMedia && window.matchMedia('(prefers-color-scheme: dark)').matches ? 'dark' : 'light';
        }
        return savedTheme;
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
    },
    applyTheme: function(theme) {
        applyTheme(theme);
    },
    getCurrentTheme: function() {
        return getCurrentTheme();
    }
};

// Initialize theme on page load (for pages without settings form)
document.addEventListener('DOMContentLoaded', function() {
    // Only initialize theme if not already handled by settings page
    if (!document.getElementById('profileTabs')) {
        const savedTheme = localStorage.getItem('theme') || 'auto';
        applyTheme(savedTheme);
    }
});