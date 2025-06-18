using System.ComponentModel.DataAnnotations;

namespace Roshta.ViewModels;

public class UserSettingsModel
{
    // Display Settings
    [Display(Name = "Date Format")]
    public string DateFormat { get; set; } = "dd/MM/yyyy";

    [Display(Name = "Time Format")]
    public string TimeFormat { get; set; } = "HH:mm";

    [Display(Name = "Search Results Per Page")]
    [Range(5, 100, ErrorMessage = "Search results per page must be between 5 and 100")]
    public int SearchResultsPerPage { get; set; } = 10;

    [Display(Name = "Table Density")]
    public string TableDensity { get; set; } = "normal"; // compact, normal, comfortable

    [Display(Name = "Theme Preference")]
    public string ThemePreference { get; set; } = "light"; // light, dark, auto

    // Prescription Defaults
    [Display(Name = "Default Prescription Duration (Days)")]
    [Range(1, 365, ErrorMessage = "Duration must be between 1 and 365 days")]
    public int DefaultPrescriptionDuration { get; set; } = 7;

    [Display(Name = "Default Dosage Frequency")]
    public string DefaultDosageFrequency { get; set; } = "Twice daily";

    // Notification Preferences
    [Display(Name = "Enable Success Notifications")]
    public bool EnableSuccessNotifications { get; set; } = true;

    [Display(Name = "Enable Warning Notifications")]
    public bool EnableWarningNotifications { get; set; } = true;

    [Display(Name = "Enable Error Notifications")]
    public bool EnableErrorNotifications { get; set; } = true;

    [Display(Name = "Auto-hide Success Messages")]
    public bool AutoHideSuccessMessages { get; set; } = true;

    [Display(Name = "Notification Duration (Seconds)")]
    [Range(1, 30, ErrorMessage = "Duration must be between 1 and 30 seconds")]
    public int NotificationDuration { get; set; } = 5;

    // Application Preferences
    [Display(Name = "Auto-save Drafts")]
    public bool AutoSaveDrafts { get; set; } = true;

    [Display(Name = "Confirm Before Delete")]
    public bool ConfirmBeforeDelete { get; set; } = true;

    [Display(Name = "Show Advanced Options")]
    public bool ShowAdvancedOptions { get; set; } = false;

    [Display(Name = "Enable Keyboard Shortcuts")]
    public bool EnableKeyboardShortcuts { get; set; } = true;
}