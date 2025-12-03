using FluentAssertions;
using Moq;
using Rosheta.Core.Application.Common.Exceptions;
using Rosheta.Core.Application.Contracts.Persistence;
using Rosheta.Core.Application.Services;
using Rosheta.Core.Domain.Entities;
using Xunit;

namespace Rosheta.UnitTests.Core.Application.Services;

public class PatientServiceTests
{
    private readonly Mock<IPatientRepository> _patientRepositoryMock;
    private readonly PatientService _service;

    public PatientServiceTests()
    {
        _patientRepositoryMock = new Mock<IPatientRepository>();
        _service = new PatientService(_patientRepositoryMock.Object);
    }

    [Fact]
    public async Task AddPatientAsync_ShouldThrowValidationException_WhenNameIsEmpty()
    {
        // Arrange
        var patient = new Patient { Name = "", ContactInfo = "01000000000" };

        // Act
        Func<Task> act = async () => await _service.AddPatientAsync(patient);

        // Assert
        await act.Should().ThrowAsync<ValidationException>()
            .WithMessage("Patient name is required.");
    }

    [Fact]
    public async Task AddPatientAsync_ShouldThrowBusinessRuleException_WhenNameIsNotUnique()
    {
        // Arrange
        var patient = new Patient { Name = "Duplicate Name", ContactInfo = "01000000000" };

        // Mock the repo to say "No, this name is NOT unique" (i.e., it exists)
        _patientRepositoryMock
            .Setup(r => r.IsNameUniqueAsync(patient.Name, null))
            .ReturnsAsync(false);

        // Act
        Func<Task> act = async () => await _service.AddPatientAsync(patient);

        // Assert
        await act.Should().ThrowAsync<BusinessRuleException>()
            .WithMessage($"A patient with the name '{patient.Name}' already exists.");

        // Verify we never attempted to add
        _patientRepositoryMock.Verify(r => r.AddAsync(It.IsAny<Patient>()), Times.Never);
    }

    [Fact]
    public async Task AddPatientAsync_ShouldThrowBusinessRuleException_WhenContactInfoIsNotUnique()
    {
        // Arrange
        var patient = new Patient { Name = "New Patient", ContactInfo = "DuplicateContact" };

        // Mock name is unique, BUT contact is NOT unique
        _patientRepositoryMock.Setup(r => r.IsNameUniqueAsync(patient.Name, null)).ReturnsAsync(true);
        _patientRepositoryMock.Setup(r => r.IsContactInfoUniqueAsync(patient.ContactInfo, null)).ReturnsAsync(false);

        // Act
        Func<Task> act = async () => await _service.AddPatientAsync(patient);

        // Assert
        await act.Should().ThrowAsync<BusinessRuleException>()
            .WithMessage($"A patient with the contact info '{patient.ContactInfo}' already exists.");
    }

    [Fact]
    public async Task AddPatientAsync_ShouldCallRepository_WhenRulesAreMet()
    {
        // Arrange
        var patient = new Patient { Name = "Valid Patient", ContactInfo = "01000000000" };

        // Mock all checks to pass
        _patientRepositoryMock.Setup(r => r.IsNameUniqueAsync(patient.Name, null)).ReturnsAsync(true);
        _patientRepositoryMock.Setup(r => r.IsContactInfoUniqueAsync(patient.ContactInfo, null)).ReturnsAsync(true);
        _patientRepositoryMock.Setup(r => r.AddAsync(patient)).ReturnsAsync(patient);

        // Act
        var result = await _service.AddPatientAsync(patient);

        // Assert
        result.Should().NotBeNull();
        _patientRepositoryMock.Verify(r => r.AddAsync(patient), Times.Once);
    }

    [Fact]
    public async Task GetPatientByIdAsync_ShouldThrowNotFoundException_WhenPatientDoesNotExist()
    {
        // Arrange
        int patientId = 999;
        _patientRepositoryMock.Setup(r => r.GetByIdAsync(patientId)).ReturnsAsync((Patient?)null);

        // Act
        Func<Task> act = async () => await _service.GetPatientByIdAsync(patientId);

        // Assert
        await act.Should().ThrowAsync<NotFoundException>();
    }
}