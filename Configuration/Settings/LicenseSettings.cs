namespace Rosheta.Configuration.Settings;

public class LicenseSettings
{
    public const string SectionName = "LicenseSettings";

    public string ExpectedRegistrationNumber { get; set; } = string.Empty;
    public string ExpectedSerialNumber { get; set; } = string.Empty;
}
