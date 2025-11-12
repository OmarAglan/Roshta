# Rosheta - Prescription Management Application

This project is an ASP.NET Core 9.0 Razor Pages application designed for managing medical prescriptions.

## Project Goal

To provide a system for doctors to issue and manage prescriptions, patients to view their prescriptions, and potentially for pharmacists to track dispensing.

## Technology Stack

* **Framework:** ASP.NET Core 9.0
* **UI:** Razor Pages
* **Data Access:** Entity Framework Core
* **Database:** SQLite (currently using `Rosheta.db`)
* **Authentication:** ASP.NET Core Identity (Planned)
* **Core Models:** Includes Patients, Doctors, Prescriptions, Medications with relationships, audit fields (`CreatedAt`, `UpdatedAt`), status tracking (e.g., `PrescriptionStatus` enum), and specific fields for visits, payments, etc.

## Features (v0.9.8.9)

* **Activation & Licensing:** Application requires activation via a license key (simple string check). Access is controlled by an `ActivationCheckPageFilter`.
* **Doctor Profile Management:**
  * Initial profile setup (`/DoctorProfile/Setup`) enforced after activation.
  * Profile editing (`/DoctorProfile/Edit`) available after setup.
* **Prescription Management:**
  * Creation with dynamic medication items.
  * List and Detail views.
  * Cancellation (status change) of active prescriptions.
  * Copying existing prescriptions to a new draft.
* **CRUD Operations:**
  * Medications
  * Patients
  * Prescriptions (linking Patients and Medications, associated with the current Doctor)
* **Data Validation:** Uses Data Annotations and Model Validation.
  * Added custom validation for foreign keys (e.g., Patient/Medication selection in Prescription Create).
  * Added custom validation for business rules (e.g., Date checks in Patient, required contact in Doctor Profile).
  * Added client-side validation helpers (`validation-helpers.js`) with debouncing for improved UX (e.g., required fields on Patient forms).
  * Added real-time (AJAX on blur) uniqueness checks for Medication Name, Patient Name, and Patient ContactInfo on Create/Edit pages.
* **Service & Repository Pattern:** Business logic is separated using services and data access using repositories.
* **Basic Search:** Implemented simple text search on list pages for Patients (Name, ContactInfo), Medications (Name), and Prescriptions (Patient Name).
* **UI/UX Enhancements:**
  * **Index Pages:** Replaced text action links with icons, added status badges, and implemented server-side pagination and sorting for Patients, Medications, and Prescriptions lists.
  * **Forms:** Standardized layout (spacing, labels) and button styles across main CRUD pages using Bootstrap 5 conventions. Enhanced Prescription Create item list UI (icon button, separator) and integrated Select2 for dropdowns.
  * **Notifications:** Replaced TempData alerts with Bootstrap Toasts for user feedback on key actions.
  * **Dashboard:** Converted home page into a dashboard with key stats and actions; simplified main navigation.

## Getting Started

1. Ensure you have the .NET 9 SDK installed.
2. Install EF Core tools globally: `dotnet tool install --global dotnet-ef` (if not already installed).
3. Clone the repository.
4. The application uses a SQLite database (`Rosheta.db`) located in the project root. The connection string is configured in `appsettings.json`.
5. Apply database migrations: `dotnet ef database update` (this will create the `Rosheta.db` file if it doesn't exist).
6. Run the application: `dotnet run`
7. Upon first run, you will be prompted to activate the application (enter any non-empty string for the key) and then set up the doctor profile.

## Development Roadmap

See [ROADMAP.md](ROADMAP.md) for the detailed development plan.
