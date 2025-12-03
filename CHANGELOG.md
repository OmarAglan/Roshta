# Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/).

## [0.9.9.11] - 2025-12-03

### 🏗️ Architectural Overhaul (Clean Architecture)

This release represents a complete structural rewrite of the solution to strictly adhere to **Clean Architecture** principles.

### Changed

* **Project Structure:** Split the monolithic `Rosheta.csproj` into three physical projects:
  * **`Rosheta.Core`**: Domain entities, Interfaces, and Business Logic (Zero dependencies).
  * **`Rosheta.Infrastructure`**: EF Core Data Access, File Storage, and External Adapters.
  * **`Rosheta.Web`**: The ASP.NET Core Razor Pages presentation layer.
* **Dependency Injection:** Replaced manual registration in `Program.cs` with specific Extension Methods (`AddApplicationServices`, `AddInfrastructureServices`) to enforce the Open/Closed principle.
* **Namespace Standardization:** Unified all namespaces to `Rosheta.*` to match the directory structure.
* **Documentation:** Restructured documentation into a centralized `docs/` folder with a comprehensive `DEVELOPER_GUIDE.md`.

### Added

* **Unit Testing Project:** Added `Rosheta.UnitTests` using xUnit, Moq, and FluentAssertions.
* **Service Tests:** Implemented initial unit test coverage for `DoctorService`, `PatientService`, and `PrescriptionService` covering critical business logic and validation.
* **Domain Exceptions:** Introduced strongly-typed exceptions (`ValidationException`, `NotFoundException`, etc.) to replace generic exceptions.
* **Global Exception Middleware:** Centralized error handling pipeline to catch domain exceptions and render appropriate HTTP responses.

### Removed

* **Root Clutter:** Moved `ARCH_*.md` files to `docs/architecture/`.
* **Circular Dependencies:** Eliminated direct references between Data Repositories and Web PageModels.

## [0.9.9.10] - 2025-06-18

### Added

* **PHASE 3.5: HEADER & FOOTER ENHANCEMENT - COMPLETE** - Final CSS styling implementation for header and footer components.
* **Comprehensive Header Styling** in [`wwwroot/css/site.css`](wwwroot/css/site.css:1):
  * Modern header container with sticky positioning and professional gradients
  * Animated primary gradient background with gradient shifting animation
  * Brand logo and text styling with backdrop blur effects and hover animations
  * Professional navigation link styling with hover effects and transitions
  * Global search dropdown styling with enhanced input group design
  * Status indicator styling with hover effects and scaling animations
  * Notification bell styling with animated badges and pulse effects
  * User menu dropdown styling with shadows and responsive design
* **Complete Footer Styling** implementation:
  * Modern footer container with dark gradient themes
  * Professional footer main section with animated dark gradients
  * Footer section organization with proper typography and spacing
  * Contact information styling with hover animations
  * Footer links with smooth transitions and transform effects
  * System information panel with enhanced styling and status indicators
  * Social media links with circular buttons and hover effects
  * Footer bottom section with proper contrast and typography
* **CSS Architecture Enhancements**:
  * CSS custom properties (variables) for maintainable color schemes
  * Comprehensive responsive design with mobile-first approach
  * Professional animations and transitions throughout
  * Accessibility features with focus-visible support and ARIA compliance
  * Cross-browser compatibility with graceful degradation
  * Print-friendly styles for professional documentation
  * High contrast mode support for accessibility
  * Dark theme preparation with prefers-color-scheme detection
* **Performance Optimizations**:
  * Efficient CSS selectors and animation performance optimization
  * Reduced motion support for accessibility preferences
  * Loading animation utilities with shimmer effects
  * Optimized gradient animations with GPU acceleration

### Enhanced

* **User Experience Improvements**:
  * Smooth hover animations and micro-interactions
  * Professional color scheme with consistent branding
  * Enhanced visual hierarchy with proper spacing and typography
  * Mobile-responsive design with breakpoint-specific optimizations
  * Professional gradient effects with subtle animations
* **Accessibility Features**:
  * Skip navigation links for keyboard users
  * Proper focus management with visible focus indicators
  * High contrast mode support for vision accessibility
  * Reduced motion preferences for users with vestibular disorders
  * WCAG 2.1 AA compliant color contrast ratios
* **Technical Excellence**:
  * Modular CSS architecture with organized sections
  * Comprehensive documentation with clear code comments
  * Future-ready design system with consistent patterns
  * Bootstrap 5 compatibility with custom enhancements

### Completed

* **UI Improvement Initiative - 100% COMPLETE** - All header and footer styling implemented
* **Phase 3.5: Header & Footer Enhancement** - Final phase completed successfully
* Modern, professional design language established throughout the application
* Comprehensive CSS foundation ready for future enhancements
* Enterprise-level user interface with polished visual design

### Technical Notes

* **722 lines of comprehensive CSS** covering all header and footer components
* **Custom CSS properties** enable easy theme customization and maintenance
* **Mobile-first responsive design** ensures optimal experience across all devices
* **Performance-optimized animations** with hardware acceleration and reduced motion support
* **Future-ready architecture** supports upcoming dark theme and additional customization
* **Zero breaking changes** - all existing functionality preserved and enhanced

## [0.9.9.9] - 2025-06-18

### Added

* **PHASE 3: ADVANCED FEATURES & FINAL POLISH** - Complete implementation of advanced UI features, accessibility enhancements, and performance optimizations.
* **Enhanced Details Pages** with professional layouts and comprehensive information display:
  * Redesigned [`Pages/Patients/Details.cshtml`](Pages/Patients/Details.cshtml:1) with modern card-based layout, status indicators, and action panels
  * Enhanced [`Pages/Medications/Details.cshtml`](Pages/Medications/Details.cshtml:1) with drug information, clinical data, and safety alerts
  * Upgraded [`Pages/Prescriptions/Details.cshtml`](Pages/Prescriptions/Details.cshtml:1) with professional prescription layout and management tools
* **Advanced Table Features** - Created [`wwwroot/js/advanced-tables.js`](wwwroot/js/advanced-tables.js:1):
  * Bulk action capabilities with checkbox selection and master checkbox
  * Advanced filtering panel with search, date range, and status filters
  * Enhanced column sorting with visual indicators
  * Export functionality (CSV, Excel, PDF, Print) with data processing
  * Keyboard navigation and accessibility features
* **Loading States & Animations** - Created [`wwwroot/js/loading-animations.js`](wwwroot/js/loading-animations.js:1):
  * Skeleton loaders for better perceived performance
  * Button and form loading states with spinner indicators
  * Smooth page transitions and content animations
  * Progressive content loading with Intersection Observer
  * AJAX call wrapping with loading states
* **Enhanced Error Handling** - Redesigned [`Pages/Error.cshtml`](Pages/Error.cshtml:1):
  * Professional error page with helpful information and actions
  * Development mode detection with detailed debugging information
  * Common issues and solutions section
  * Enhanced navigation and user guidance
* **Advanced Validation** - Enhanced [`Pages/Shared/_ValidationScriptsPartial.cshtml`](Pages/Shared/_ValidationScriptsPartial.cshtml:1):
  * Custom validation methods (strong password, phone number, date validation)
  * Real-time validation with improved user feedback
  * Form auto-save functionality with draft restoration
  * Enhanced accessibility with ARIA attributes and focus management
  * Character counters for textareas with maxlength
* **Comprehensive CSS Enhancements** - Extended [`wwwroot/css/site.css`](wwwroot/css/site.css:400):
  * Loading states and skeleton animations
  * Enhanced card hover effects and transitions
  * Advanced table styling with bulk actions toolbar
  * Error and success state designs
  * Print media queries for professional printing
  * High contrast and reduced motion support
  * Prescription-specific styling for professional layouts

### Enhanced

* **Accessibility Improvements** throughout the application:
  * WCAG 2.1 AA compliance with proper ARIA attributes
  * Enhanced focus management and keyboard navigation
  * Screen reader optimizations with semantic HTML
  * Skip links and proper heading hierarchy
* **Performance Optimizations**:
  * CSS critical path optimization considerations
  * Efficient animation handling with will-change properties
  * Intersection Observer for content loading
  * Debounced validation and search functionality
* **User Experience Enhancements**:
  * Toast notification system integration
  * Contextual help and tooltips
  * Progressive enhancement patterns
  * Mobile-first responsive design improvements

### Technical Features

* **Advanced JavaScript Architecture**:
  * Class-based component design with `AdvancedTable` and `LoadingManager`
  * Singleton pattern implementation for global loading management
  * Event delegation and proper cleanup
  * Modular design with auto-initialization capabilities
* **Cross-browser Compatibility**:
  * Modern browser feature detection
  * Graceful degradation for older browsers
  * Consistent styling across different platforms
* **Developer Experience**:
  * Comprehensive documentation in code comments
  * Extensible architecture for future enhancements
  * Consistent code patterns and conventions

### Notes

* **Phase 3 UI improvements are now COMPLETE** - All advanced features implemented
* Application now provides enterprise-level user experience with professional polish
* Accessibility features ensure compliance with modern web standards
* Performance optimizations provide smooth user interactions
* Comprehensive error handling improves user confidence and debugging

## [0.9.9.8] - 2025-06-18

### Added

* **COMPREHENSIVE SETTINGS FUNCTIONALITY & ENHANCED NAVIGATION** - Complete implementation of user settings management and improved navigation system.
* Created [`ISettingsService`](Services/Interfaces/ISettingsService.cs:1) interface and [`SettingsService`](Services/SettingsService.cs:1) implementation:
  * File-based settings persistence using JSON in LocalApplicationData
  * GetUserSettingsAsync, SaveUserSettingsAsync, and GetDefaultSettings methods
  * Comprehensive error handling and logging
* Created [`UserSettingsModel`](ViewModels/UserSettingsModel.cs:1) with extensive configuration options:
  * Display settings: date/time formats, search results per page, table density, theme preferences
  * Prescription defaults: duration, dosage frequency
  * Notification preferences: success/warning/error notifications, auto-hide, duration
  * Application preferences: auto-save drafts, delete confirmation, advanced options, keyboard shortcuts
* **Enhanced Navigation System** in [`_Layout.cshtml`](Pages/Shared/_Layout.cshtml:1):
  * User dropdown menu with doctor name and profile icon
  * "Profile & Settings" link with gear icon
  * About section with version information
  * Responsive breadcrumb navigation component
* **Tabbed Settings Interface** - Complete redesign of [`DoctorProfile/Edit.cshtml`](Pages/DoctorProfile/Edit.cshtml:1):
  * Five-tab interface: Profile Information, Display Settings, Prescription Defaults, Notifications, Application Preferences
  * Bootstrap 5 tab components with proper ARIA attributes
  * Individual forms for each settings category with proper validation
  * Tab persistence using localStorage
* Enhanced CSS styling in [`site.css`](wwwroot/css/site.css:164):
  * Navigation dropdown styling with shadows and hover effects
  * Breadcrumb navigation with proper separators and links
  * Tab interface styling with active states and transitions
  * Enhanced form styling with focus states and validation
  * Responsive design improvements for mobile devices
* Created [`settings.js`](wwwroot/js/settings.js:1) with advanced functionality:
  * Tab persistence across page reloads
  * Live preview for date/time format settings
  * Notification settings preview
  * Form validation and loading states
  * Theme preview preparation for future implementation
* Added settings service registration to [`Program.cs`](Program.cs:39)

### Changed

* Extended [`DoctorProfile/Edit.cshtml.cs`](Pages/DoctorProfile/Edit.cshtml.cs:1) with settings functionality:
  * Added ISettingsService dependency injection
  * Added UserSettingsModel property binding
  * Implemented OnPostSaveSettingsAsync handler for settings persistence
  * Enhanced error handling and user feedback
* Updated page title to "Profile & Settings" with proper breadcrumb integration
* Improved form layout and user experience with card-based design
* Enhanced mobile responsiveness across all settings components

### Technical Notes

* **Settings are persisted locally** using JSON files in user's LocalApplicationData folder
* **Backward compatibility maintained** - existing profile functionality unchanged
* **Comprehensive validation** implemented for all settings with proper error messaging
* **Accessibility enhanced** with proper ARIA attributes and keyboard navigation
* **Performance optimized** with debounced validation and efficient tab switching
* **Future-ready** theme system prepared for light/dark mode implementation

## [0.9.9.7] - 2025-06-18

### Added

* **COMPLETED LIVE SEARCH FUNCTIONALITY** - Full integration of frontend and backend components.
* Created [`live-search.js`](wwwroot/js/live-search.js:1) with comprehensive autocomplete functionality:
  * Debounced search with 300ms delay to optimize performance
  * Keyboard navigation support (arrow keys, enter, escape)
  * Accessibility features with ARIA attributes
  * Loading states, error handling, and "no results" messaging
  * Configurable display and secondary properties
  * Auto-initialization via data attributes
* Enhanced all Index pages with live search integration:
  * [`Patients/Index.cshtml`](Pages/Patients/Index.cshtml:1) with name and contact info autocomplete
  * [`Medications/Index.cshtml`](Pages/Medications/Index.cshtml:1) with medication name autocomplete
  * [`Prescriptions/Index.cshtml`](Pages/Prescriptions/Index.cshtml:1) with patient and medication search
* Added comprehensive CSS styling for autocomplete dropdowns in [`site.css`](wwwroot/css/site.css:1):
  * Responsive dropdown positioning and sizing
  * Hover and keyboard selection highlighting
  * Loading, error, and no-results state styling
  * Bootstrap-consistent design language
* Updated [`_Layout.cshtml`](Pages/Shared/_Layout.cshtml:1) to include live-search.js

### Changed

* Enhanced search experience from form-based to real-time autocomplete
* Improved user workflow efficiency with instant search suggestions
* Maintained backward compatibility with existing form-based search as fallback

### Technical Notes

* **Live search feature is now COMPLETE and fully functional**
* Debouncing prevents excessive API calls and improves performance
* Keyboard navigation enhances accessibility and user experience
* Error handling ensures graceful degradation when API calls fail
* All three main entities (Patients, Medications, Prescriptions) support live search

## [0.9.9.6] - 2025-06-18

### Added

* Implemented backend API endpoints for live search functionality across all main entities.
* Added [`PatientSearchDto`](ViewModels/PatientSearchDto.cs:1) class for optimized patient search responses.
* Added [`MedicationSearchDto`](ViewModels/MedicationSearchDto.cs:1) class for optimized medication search responses.
* Added [`PrescriptionSearchDto`](ViewModels/PrescriptionSearchDto.cs:1) class for optimized prescription search responses.
* Added [`OnGetSearchAsync`](Pages/Patients/Index.cshtml.cs:1) method to Patients Index page for JSON API endpoint.
* Added [`OnGetSearchAsync`](Pages/Medications/Index.cshtml.cs:1) method to Medications Index page for JSON API endpoint.
* Added [`OnGetSearchAsync`](Pages/Prescriptions/Index.cshtml.cs:1) method to Prescriptions Index page for JSON API endpoint.

### Changed

* Prepared backend infrastructure for live search/autocomplete implementation.
* Updated project documentation to reflect current development progress.

### Technical Notes

* Backend API endpoints ready for frontend JavaScript integration.
* Search DTOs provide lightweight, optimized data transfer for AJAX requests.
* JSON endpoints support query parameter-based search functionality.

## [0.9.9.5] - 2025-05-01

### Added

* Added toast notification container to `_Layout.cshtml`.
* Added `showToast` JavaScript helper function to `wwwroot/js/site.js`.

### Changed

* Standardized form layout (spacing, labels) on Patient, Medication, Doctor Profile, and Prescription Create/Edit pages using Bootstrap 5 conventions (`mb-3`, `form-label`).
* Standardized button styles (`btn-primary`, `btn-outline-secondary`, `btn-danger`) on Index, Create, Edit, Delete pages and Dashboard.
* Replaced `TempData` alerts with Bootstrap Toasts for feedback messages on Prescription Index, Doctor Profile Edit, Patient Index (for CRUD), and Medication Index (for CRUD) pages.
* Hid automatically set `CreatedAt`/`UpdatedAt` fields on Create forms.
* Made `CreatedAt`/`UpdatedAt` fields read-only on Edit forms.

## [0.9.9.0] - 2025-05-01

### Added

* Implemented server-side pagination for Patients, Medications, and Prescriptions index pages.
* Implemented server-side sorting (by Name/Date) for Patients, Medications, and Prescriptions index pages.
* Added Bootstrap Icons CDN link to `_Layout.cshtml`.
* Added Select2 library (CDN) for enhanced dropdowns.
* Initialized Select2 on Patient and Medication dropdowns in `Pages/Prescriptions/Create.cshtml`.
* Added seed data for Doctor, Patients, and Medications via `ApplicationDbContext.OnModelCreating`.
* Created `AddSeedData` database migration.
* Transformed `Pages/Index.cshtml` into a dashboard displaying entity counts and quick action links.

### Changed

* Updated Patients, Medications, and Prescriptions repositories and services to support pagination and sorting.
* Updated Patients, Medications, and Prescriptions index page models (`.cshtml.cs`) to handle pagination and sorting logic.
* Updated Patients, Medications, and Prescriptions index views (`.cshtml`) to include pagination controls and sortable column headers.
* Replaced text action links with icon buttons on Patients, Medications, and Prescriptions index pages.
* Used Bootstrap Badges for status display on Patients and Prescriptions index pages.
* Updated `validation-helpers.js` (`validateRequiredField`) to correctly handle missing validation spans and return boolean validity.
* Refined client-side validation logic in `Pages/Prescriptions/Create.cshtml` for adding items.
* Simplified header navigation in `_Layout.cshtml` (removed Privacy, New Prescription, Edit Profile links).
* Moved "New Prescription" and conditional "Edit Profile" buttons to the Dashboard page (`Pages/Index.cshtml`).
* Updated `README.md` and `ROADMAP.md` to reflect completed UI/UX enhancements.

### Fixed

* Fixed Bootstrap Icons not appearing due to missing CSS link.
* Fixed client-side validation issue preventing adding items in `Pages/Prescriptions/Create.cshtml`.
* Corrected `Doctor.Specialization` property name in seed data.
* Corrected `<script>` tag for Select2 JS in `_Layout.cshtml`.

## [0.9.8.9] - 2024-05-05

### Added

* Implemented real-time AJAX uniqueness checks for Patient Name field on `Pages/Patients/Create.cshtml` and `Pages/Patients/Edit.cshtml`.
* Added `IsNameUniqueAsync` method to `IPatientRepository`/`PatientRepository` and `IPatientService`/`PatientService`.
* Added `setupPatientNameUniquenessCheck` function to `wwwroot/js/validation-helpers.js`.
* Implemented real-time AJAX uniqueness check for Medication Name field on `Pages/Medications/Create.cshtml` and `Pages/Medications/Edit.cshtml`.
* Added `IsNameUniqueAsync` method to `IMedicationRepository`/`MedicationRepository` and `IMedicationService`/`MedicationService`.
* Added `setupMedicationNameUniquenessCheck` function to `wwwroot/js/validation-helpers.js`.
* Implemented real-time AJAX uniqueness check for Patient ContactInfo field on `Pages/Patients/Create.cshtml` and `Pages/Patients/Edit.cshtml`.
* Added `IsContactInfoUniqueAsync` method to `IPatientRepository`/`PatientRepository` and `IPatientService`/`PatientService`.
* Added `setupPatientContactInfoUniquenessCheck` function to `wwwroot/js/validation-helpers.js`.

### Changed

* Updated `Pages/Patients/Create.cshtml` and `Pages/Patients/Edit.cshtml` to call `setupPatientNameUniquenessCheck` for real-time validation.
* Updated `Pages/Patients/Create.cshtml` and `Pages/Patients/Edit.cshtml` to call `setupPatientContactInfoUniquenessCheck` for real-time validation.
* Updated `Pages/Medications/Create.cshtml` and `Pages/Medications/Edit.cshtml` to call `setupMedicationNameUniquenessCheck` for real-time validation.
* Updated `ROADMAP.md` to mark Phase B.5 (real-time uniqueness checks) as completed for Medication Name, Patient ContactInfo, and Patient Name.
* Updated `README.md` to reflect the new version number and added features.

## [0.9.8.2] - YYYY-MM-DD

### Added

* Added client-side "required" validation for the Name field and dynamic submit button logic to `Pages/Medications/Create.cshtml` and `Pages/Medications/Edit.cshtml` for consistency with other forms.

### Changed

* Reviewed Patient, Doctor Profile, and Prescription Create forms to ensure consistent application of client-side validation enhancements (required, format, cross-field checks, debouncing, dynamic submit button).

## [0.9.8.0] - YYYY-MM-DD

### Added

* Added client-side validation helper functions (`validateBasicContactFormat`, `validateNumericRange`, `validatePositiveNumber`, `validateNonNegativeInteger`) to `wwwroot/js/validation-helpers.js`.
* Added client-side "required" validation for Dosage, Frequency, and Duration inputs in the "Add Item" section of `Pages/Prescriptions/Create.cshtml`.
* Added client-side "required" validation for the Name field on `Pages/Medications/Create.cshtml` and `Pages/Medications/Edit.cshtml`.

### Changed

* Updated `Pages/Patients/Create.cshtml` and `Pages/Patients/Edit.cshtml` to add basic format validation (phone/email check) to the `ContactInfo` field using `validateBasicContactFormat`.
* Updated `Pages/DoctorProfile/Setup.cshtml` and `Pages/DoctorProfile/Edit.cshtml` to add numeric range validation (digits, length 4-19) to the `LicenseNumber` field using `validateNumericRange`.
* Updated `Pages/Prescriptions/Create.cshtml` to use `validatePositiveNumber` for the Quantity input and `validateNonNegativeInteger` for the Refills input in the "Add Item" section.
* Updated all forms with client-side validation to include dynamic submit button disabling/enabling based on validation state.
* Updated `ROADMAP.md` to reflect completion of Validation Phases B.1-B.4.

## [0.9.7.5] - YYYY-MM-DD

### Added

* Added client-side, debounced input validation for the `Name` and `ContactInfo` fields on the Patient Create (`Pages/Patients/Create.cshtml`) and Patient Edit (`Pages/Patients/Edit.cshtml`) pages to ensure they are not empty.

### Changed

* Updated `ActivationCheckPageFilter` to redirect activated users to `/DoctorProfile/Setup` if their profile is not yet complete.
* Updated `Pages/Patients/Create.cshtml` and `Pages/Patients/Edit.cshtml` to include `id` attributes on relevant inputs and integrate JavaScript validation using `wwwroot/js/validation-helpers.js`.

## [0.9.7.1] - YYYY-MM-DD

### Added

* Implemented "Cancel Prescription" feature (updates status via button on Index page).
* Implemented "Copy Prescription" feature (button on Details page links to pre-populated Create page).

### Changed

* Added `CancelAsync`/`CancelPrescriptionAsync` methods to `IPrescriptionRepository`/`PrescriptionRepository` and `IPrescriptionService`/`PrescriptionService`.
* Updated `Pages/Prescriptions/Create.cshtml.cs` (`OnGetAsync`) to handle `copyFromId` parameter.
* Updated `Pages/Prescriptions/Create.cshtml` view and JavaScript to render and manage pre-populated items.
* Updated `Pages/Prescriptions/Index.cshtml` to include Cancel button/form for active prescriptions and improve status display.
* Updated `Pages/Prescriptions/Details.cshtml` to include Copy button.
* Updated `README.md`.

### Fixed

* Fixed Razor syntax errors (missing braces, unclosed div) in `Pages/Prescriptions/Details.cshtml`.

## [0.9.7] - YYYY-MM-DD

### Added

* Implemented custom server-side validation using `IValidatableObject`:
  * `PrescriptionCreateModel`: Validate `ExpiryDate` and `NextAppointmentDate` are in the future; ensure at least one item exists.
  * `Patient`: Validate `DateOfBirth` is in the past; `LastVisitDate` is not in the future.
  * `SetupModel` & `EditModel` (Doctor Profile): Ensure at least one contact method (Phone or Email) is provided.
* Implemented client-side AJAX check on `Pages/Prescriptions/Create.cshtml` to verify `MedicationId` existence before adding item to the list.
* Added server-side AJAX handler `OnGetCheckMedicationExistsAsync` to `Pages/Prescriptions/Create.cshtml.cs`.

### Changed

* Improved server-side `MedicationId` validation error message in `Pages/Prescriptions/Create.cshtml.cs` to show medication name if possible.
* Updated `README.md` and `ROADMAP.md`.

## [0.9.6] - YYYY-MM-DD

### Added

* Implemented basic search functionality (form submission) for Patients, Medications, and Prescriptions lists.
* Added `Dosage`, `Frequency`, and `Duration` fields to `PrescriptionItem` model and `PrescriptionItemCreateModel` view model.
* Added corresponding input fields to `Pages/Prescriptions/Create.cshtml` and updated JavaScript logic.
* Added EF Core migration `AddDosageFrequencyDurationToItem`.

### Changed

* Updated `PatientService`, `MedicationService`, `PrescriptionService` and their interfaces to include `Search...Async` methods.
* Updated `PatientRepository`, `MedicationRepository`, `PrescriptionRepository` and their interfaces to include `SearchAsync` methods.
* Refactored method return types in `PatientRepository`/`IPatientRepository` and `MedicationRepository`/`IMedicationRepository` (Add/Update/Delete) for consistency.
* Renamed methods in `PrescriptionRepository`/`IPrescriptionRepository` for consistency.
* Updated `README.md` and `ROADMAP.md`.

### Fixed

* Resolved build errors caused by mismatched method signatures between services, repositories, and interfaces after adding search functionality.
* Resolved build errors caused by incorrect field names in search query for Patients (`ContactInfo` instead of `ContactEmail`/`ContactPhone`).
* Resolved build errors caused by missing fields (`Dosage`, `Frequency`, `Duration`) in `PrescriptionItem` mapping within `PrescriptionService`.

## [0.9.5] - YYYY-MM-DD

### Added

* Created `Pages/DoctorProfile/Setup.cshtml` and `.cs` for initial doctor profile setup after activation.
* Created `Pages/DoctorProfile/Edit.cshtml` and `.cs` for editing the doctor profile after setup.
* Added `GetDoctorProfileAsync(int)` and `UpdateDoctorProfileAsync(int, DoctorProfileInputModel)` methods to `IDoctorService`/`DoctorService` and `IDoctorRepository`/`DoctorRepository`.
* Added "Edit Profile" link to the main navigation in `_Layout.cshtml`, visible only after activation and profile setup.

### Changed

* Updated `ActivationCheckPageFilter` to redirect to `/DoctorProfile/Setup` if activated but profile is not set up.
* Updated `ActivationCheckPageFilter` to redirect from `/Activate` page to `/Index` (or `/DoctorProfile/Setup`) if already activated.
* Updated `README.md` and `ROADMAP.md` to reflect current features and progress.

### Fixed

* Corrected `RedirectToPageResult` calls in `ActivationCheckPageFilter` to use correct page routes (e.g., `/DoctorProfile/Setup`) instead of file paths.

## [0.9.0] - YYYY-MM-DD

### Added

* Added temporary license key storage in `appsettings.json` and `LicenseSettings` model.
* Created `ILicenseService` and `LicenseService` for license validation and activation status (using simple file flag).
* Created `Pages/Activate.cshtml` and `.cs` for user license key input.
* Implemented `ActivationCheckPageFilter` (global Razor Page filter) to enforce activation.
* Registered `LicenseService` and `ActivationCheckPageFilter`.

### Fixed

* Resolved ambiguous route issue for the `/Activate` page.

## [0.8.0] - YYYY-MM-DD

### Added

* Created `Pages/Prescriptions/Index.cshtml` and `.cs` for listing prescriptions.
* Created `Pages/Prescriptions/Details.cshtml` and `.cs` for viewing prescription details (including items).
* Updated `IPrescriptionService` and `PrescriptionService` with methods for retrieving prescriptions.
* Added navigation link for Prescriptions list to `_Layout.cshtml`.

## [0.7.0] - YYYY-MM-DD

### Added

* Defined `IMedicationService` and `IPatientService` interfaces.
* Implemented `MedicationService` and `PatientService` using respective repositories.
* Registered services for dependency injection.

### Changed

* Refactored Medication CRUD pages (`Pages/Medications/*`) to use `IMedicationService`.
* Refactored Patient CRUD pages (`Pages/Patients/*`) to use `IPatientService`.

## [0.6.0] - YYYY-MM-DD

### Added

* Defined `IPrescriptionRepository` interface and `PrescriptionRepository` implementation.
* Defined `IPrescriptionService` interface and `PrescriptionService` implementation.
* Defined `PrescriptionCreateModel` ViewModel for prescription creation form.
* Created `Pages/Prescriptions/Create.cshtml` and `.cs` for adding new prescriptions, including basic JavaScript for dynamic medication item handling.
* Registered `IPrescriptionRepository` and `IPrescriptionService` for DI.

### Changed

* Added navigation link for 'New Prescription' to `_Layout.cshtml`.

## [0.5.0] - 2025-04-14

### Added

* Scaffolded basic CRUD Razor Pages for Patient Management (`Pages/Patients`).
* Defined `IPatientRepository` interface.
* Implemented `PatientRepository` using `ApplicationDbContext`.
* Registered `IPatientRepository` for dependency injection.

### Changed

* Refactored Patient CRUD pages (`Pages/Patients/*`) to use `IPatientRepository` instead of direct `DbContext` access.
* Added navigation link for Patients to `_Layout.cshtml`.

## [0.4.0] - 2025-04-14

### Added

* Defined `IMedicationRepository` interface.
* Implemented `MedicationRepository` using `ApplicationDbContext`.
* Registered `IMedicationRepository` for dependency injection.

### Changed

* Refactored Medication CRUD pages (`Pages/Medications/*`) to use `IMedicationRepository` instead of direct `DbContext` access.

## [0.3.0] - 2025-04-13

### Added

* Scaffolded basic CRUD Razor Pages for Medication Management (`Pages/Medications`).

### Changed

* Updated `_Layout.cshtml` with 'Rosheta' branding and navigation link for Medications.

## [0.2.0] - 2025-04-13

### Added

* Added specific fields (e.g., `IsSubscribed`, `VisitCount`, `HasOutstandingBalance`, `Notes`, `ContactPhone`, `ContactEmail`) to core models based on requirements.
* Added audit fields (`CreatedAt`, `UpdatedAt`) to all core models.
* Added `PrescriptionStatus` enum.
* Added `ApplicationDbContext.SaveChanges` override to auto-update audit fields.

### Changed

* Refined core data models with suggestions (e.g., `
