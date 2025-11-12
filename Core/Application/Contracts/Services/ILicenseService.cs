namespace Rosheta.Core.Application.Contracts.Services;

public interface ILicenseService
{
    /// <summary>
    /// Validates the entered registration and serial numbers against the expected values.
    /// </summary>
    /// <param name="enteredRegistrationNumber">The registration number entered by the user.</param>
    /// <param name="enteredSerialNumber">The serial number entered by the user.</param>
    /// <returns>True if the keys are valid, false otherwise.</returns>
    bool ValidateLicense(string enteredRegistrationNumber, string enteredSerialNumber);

    /// <summary>
    /// Checks if the application has been successfully activated.
    /// </summary>
    /// <returns>True if activated, false otherwise.</returns>
    bool IsActivated();

    /// <summary>
    /// Marks the application as activated (e.g., after successful validation).
    /// </summary>
    Task MarkAsActivatedAsync();

    /// <summary>
    /// Checks if the doctor profile has been set up after activation.
    /// </summary>
    /// <returns>True if profile is set up, false otherwise.</returns>
    Task<bool> IsProfileSetupAsync();

    /// <summary>
    /// Marks the profile as set up and stores the associated Doctor ID.
    /// </summary>
    /// <param name="doctorId">The ID of the saved Doctor profile.</param>
    Task MarkProfileAsSetupAsync(int doctorId);

    /// <summary>
    /// Gets the ID of the currently configured/licensed doctor.
    /// </summary>
    /// <returns>The Doctor ID if set up, otherwise null.</returns>
    Task<int?> GetCurrentDoctorIdAsync();
} 
