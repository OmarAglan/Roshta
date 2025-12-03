using FluentAssertions;
using Microsoft.Extensions.Options;
using Moq;
using Rosheta.Core.Application.Contracts.Infrastructure;
using Rosheta.Core.Application.Services;
using Rosheta.Core.Application.Settings;
using Xunit;

namespace Rosheta.UnitTests.Core.Application.Services;

public class LicenseServiceTests
{
    private readonly Mock<IFileStorageProvider> _fileStorageMock;
    private readonly Mock<IOptions<LicenseSettings>> _optionsMock;
    private readonly LicenseService _service;
    private readonly LicenseSettings _settings;

    public LicenseServiceTests()
    {
        _fileStorageMock = new Mock<IFileStorageProvider>();
        
        // Setup Configuration Mock
        _settings = new LicenseSettings 
        { 
            ExpectedRegistrationNumber = "REG-TEST", 
            ExpectedSerialNumber = "SN-TEST" 
        };
        _optionsMock = new Mock<IOptions<LicenseSettings>>();
        _optionsMock.Setup(x => x.Value).Returns(_settings);

        // Setup FileStorage to return a dummy path for base dir
        _fileStorageMock.Setup(x => x.GetApplicationDataPath()).Returns("C:/FakePath");
        _fileStorageMock.Setup(x => x.CombinePath(It.IsAny<string[]>()))
            .Returns((string[] paths) => string.Join("/", paths)); // Simple path join simulation

        _service = new LicenseService(_optionsMock.Object, _fileStorageMock.Object);
    }

    [Fact]
    public void ValidateLicense_ShouldReturnTrue_ForValidKeys()
    {
        // Act
        var result = _service.ValidateLicense("REG-TEST", "SN-TEST");

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void ValidateLicense_ShouldReturnFalse_ForInvalidKeys()
    {
        // Act
        var result = _service.ValidateLicense("WRONG", "WRONG");

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void IsActivated_ShouldReturnTrue_WhenActivationFileExists()
    {
        // Arrange
        // We expect the service to check for a specific file path.
        // Based on service logic: AppData + Rosheta + .activated
        _fileStorageMock.Setup(x => x.FileExists(It.Is<string>(s => s.Contains(".activated"))))
            .Returns(true);

        // Act
        var result = _service.IsActivated();

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public async Task MarkAsActivatedAsync_ShouldCreateActivationFile()
    {
        // Act
        await _service.MarkAsActivatedAsync();

        // Assert
        // Verify that WriteAllTextAsync was called once for the activation file
        _fileStorageMock.Verify(x => x.WriteAllTextAsync(
            It.Is<string>(s => s.Contains(".activated")), 
            string.Empty), 
            Times.Once);
    }
}