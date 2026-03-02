using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using Rosheta.Core.Application.Common.Exceptions;
using Rosheta.Core.Application.Contracts.Infrastructure;
using Rosheta.Core.Application.Models;
using Rosheta.Core.Application.Services;
using System.Text.Json;
using Xunit;

namespace Rosheta.UnitTests.Core.Application.Services;

public class SettingsServiceTests
{
    private readonly Mock<IFileStorageProvider> _fileStorageMock;
    private readonly Mock<ILogger<SettingsService>> _loggerMock;
    private readonly SettingsService _service;

    public SettingsServiceTests()
    {
        _fileStorageMock = new Mock<IFileStorageProvider>();
        _loggerMock = new Mock<ILogger<SettingsService>>();

        _fileStorageMock.Setup(x => x.GetApplicationDataPath()).Returns("C:/FakePath");
        _fileStorageMock.Setup(x => x.CombinePath(It.IsAny<string[]>()))
            .Returns((string[] paths) => string.Join("/", paths));

        _service = new SettingsService(_loggerMock.Object, _fileStorageMock.Object);
    }

    [Fact]
    public void Constructor_ShouldEnsureSettingsDirectoryExists()
    {
        _fileStorageMock.Verify(x => x.EnsureDirectoryExists(
            It.Is<string>(p => p.Contains("Rosheta/Settings/test.tmp"))),
            Times.Once);
    }

    [Fact]
    public void GetDefaultSettings_ShouldReturnExpectedDefaults()
    {
        var settings = _service.GetDefaultSettings();

        settings.Should().NotBeNull();
        settings.DateFormat.Should().Be("dd/MM/yyyy");
        settings.TimeFormat.Should().Be("HH:mm");
        settings.SearchResultsPerPage.Should().Be(10);
        settings.ThemePreference.Should().Be("light");
    }

    [Fact]
    public async Task GetUserSettingsAsync_ShouldReturnDefaults_WhenSettingsFileDoesNotExist()
    {
        const int doctorId = 11;
        _fileStorageMock.Setup(x => x.FileExists(
                It.Is<string>(p => p.EndsWith($"doctor_{doctorId}_settings.json"))))
            .Returns(false);

        var result = await _service.GetUserSettingsAsync(doctorId);

        result.Should().NotBeNull();
        result.DateFormat.Should().Be("dd/MM/yyyy");
        result.ThemePreference.Should().Be("light");
        _fileStorageMock.Verify(x => x.ReadAllTextAsync(It.IsAny<string>()), Times.Never);
    }

    [Fact]
    public async Task GetUserSettingsAsync_ShouldReturnDeserializedSettings_WhenFileExists()
    {
        const int doctorId = 7;
        var expected = new UserSettingsModel
        {
            DateFormat = "MM-dd-yyyy",
            TimeFormat = "hh:mm tt",
            SearchResultsPerPage = 25,
            ThemePreference = "dark",
            AutoSaveDrafts = false
        };
        var json = JsonSerializer.Serialize(expected);

        _fileStorageMock.Setup(x => x.FileExists(It.IsAny<string>())).Returns(true);
        _fileStorageMock.Setup(x => x.ReadAllTextAsync(
                It.Is<string>(p => p.EndsWith($"doctor_{doctorId}_settings.json"))))
            .ReturnsAsync(json);

        var result = await _service.GetUserSettingsAsync(doctorId);

        result.DateFormat.Should().Be(expected.DateFormat);
        result.TimeFormat.Should().Be(expected.TimeFormat);
        result.SearchResultsPerPage.Should().Be(expected.SearchResultsPerPage);
        result.ThemePreference.Should().Be(expected.ThemePreference);
        result.AutoSaveDrafts.Should().BeFalse();
    }

    [Fact]
    public async Task GetUserSettingsAsync_ShouldReturnDefaults_WhenDeserializationReturnsNull()
    {
        const int doctorId = 3;
        _fileStorageMock.Setup(x => x.FileExists(It.IsAny<string>())).Returns(true);
        _fileStorageMock.Setup(x => x.ReadAllTextAsync(It.IsAny<string>())).ReturnsAsync("null");

        var result = await _service.GetUserSettingsAsync(doctorId);

        result.DateFormat.Should().Be("dd/MM/yyyy");
        result.ThemePreference.Should().Be("light");
    }

    [Fact]
    public async Task GetUserSettingsAsync_ShouldThrowInfrastructureException_WhenReadFails()
    {
        const int doctorId = 5;
        _fileStorageMock.Setup(x => x.FileExists(It.IsAny<string>())).Returns(true);
        _fileStorageMock.Setup(x => x.ReadAllTextAsync(It.IsAny<string>()))
            .ThrowsAsync(new Exception("disk read failure"));

        Func<Task> act = async () => await _service.GetUserSettingsAsync(doctorId);

        await act.Should().ThrowAsync<InfrastructureException>()
            .WithMessage($"Failed to load settings for doctor {doctorId}.*");
    }

    [Fact]
    public async Task SaveUserSettingsAsync_ShouldThrowValidationException_WhenSettingsIsNull()
    {
        Func<Task> act = async () => await _service.SaveUserSettingsAsync(1, null!);

        await act.Should().ThrowAsync<ValidationException>()
            .WithMessage("Settings cannot be null.");
    }

    [Fact]
    public async Task SaveUserSettingsAsync_ShouldWriteSettingsJson_WhenSettingsIsValid()
    {
        const int doctorId = 9;
        var settings = new UserSettingsModel
        {
            DateFormat = "yyyy-MM-dd",
            SearchResultsPerPage = 30,
            ThemePreference = "dark"
        };

        _fileStorageMock.Setup(x => x.WriteAllTextAsync(It.IsAny<string>(), It.IsAny<string>()))
            .Returns(Task.CompletedTask);

        var result = await _service.SaveUserSettingsAsync(doctorId, settings);

        result.Should().BeTrue();
        _fileStorageMock.Verify(x => x.EnsureDirectoryExists(
            It.Is<string>(p => p.EndsWith($"doctor_{doctorId}_settings.json"))), Times.Once);
        _fileStorageMock.Verify(x => x.WriteAllTextAsync(
            It.Is<string>(p => p.EndsWith($"doctor_{doctorId}_settings.json")),
            It.Is<string>(json =>
                json.Contains("\"DateFormat\": \"yyyy-MM-dd\"") &&
                json.Contains("\"SearchResultsPerPage\": 30") &&
                json.Contains("\"ThemePreference\": \"dark\""))),
            Times.Once);
    }

    [Fact]
    public async Task SaveUserSettingsAsync_ShouldThrowInfrastructureException_WhenWriteFails()
    {
        const int doctorId = 12;
        var settings = new UserSettingsModel();

        _fileStorageMock.Setup(x => x.WriteAllTextAsync(It.IsAny<string>(), It.IsAny<string>()))
            .ThrowsAsync(new Exception("disk write failure"));

        Func<Task> act = async () => await _service.SaveUserSettingsAsync(doctorId, settings);

        await act.Should().ThrowAsync<InfrastructureException>()
            .WithMessage($"Failed to save settings for doctor {doctorId}.*");
    }
}
