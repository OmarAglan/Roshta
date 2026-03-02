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

    [Fact]
    public async Task DeletePatientAsync_ShouldCallDelete_WhenPatientExists()
    {
        // Arrange
        var patient = new Patient { Id = 10, Name = "To Delete" };

        // Mock GetById because Service now calls it first
        _patientRepositoryMock.Setup(r => r.GetByIdAsync(10)).ReturnsAsync(patient);

        // Mock Delete (Void)
        _patientRepositoryMock.Setup(r => r.DeleteAsync(patient)).Returns(Task.CompletedTask);

        // Act
        var result = await _service.DeletePatientAsync(10);

        // Assert
        result.Should().BeTrue();
        _patientRepositoryMock.Verify(r => r.DeleteAsync(patient), Times.Once);
    }

    [Fact]
    public async Task GetAllPatientsAsync_ShouldReturnPatients_WhenRepositorySucceeds()
    {
        var data = new List<Patient> { new() { Id = 1, Name = "A", ContactInfo = "x" } };
        _patientRepositoryMock.Setup(r => r.GetAllAsync()).ReturnsAsync(data);

        var result = await _service.GetAllPatientsAsync();

        result.Should().HaveCount(1);
    }

    [Fact]
    public async Task SearchPatientsAsync_ShouldReturnFilteredPatients_WhenRepositorySucceeds()
    {
        var data = new List<Patient> { new() { Id = 1, Name = "Ahmed", ContactInfo = "x" } };
        _patientRepositoryMock.Setup(r => r.SearchAsync("ah")).ReturnsAsync(data);

        var result = await _service.SearchPatientsAsync("ah");

        result.Should().ContainSingle(p => p.Name == "Ahmed");
    }

    [Fact]
    public async Task UpdatePatientAsync_ShouldThrowBusinessRuleException_WhenContactInfoDuplicate()
    {
        var patient = new Patient { Id = 1, Name = "Unique", ContactInfo = "dup" };
        _patientRepositoryMock.Setup(r => r.ExistsAsync(1)).ReturnsAsync(true);
        _patientRepositoryMock.Setup(r => r.IsNameUniqueAsync(patient.Name, 1)).ReturnsAsync(true);
        _patientRepositoryMock.Setup(r => r.IsContactInfoUniqueAsync(patient.ContactInfo, 1)).ReturnsAsync(false);

        Func<Task> act = async () => await _service.UpdatePatientAsync(patient);

        await act.Should().ThrowAsync<BusinessRuleException>()
            .WithMessage("A patient with the contact info 'dup' already exists.");
    }

    [Fact]
    public async Task UpdatePatientAsync_ShouldReturnPatient_WhenValid()
    {
        var patient = new Patient { Id = 4, Name = "Updated", ContactInfo = "0101" };
        _patientRepositoryMock.Setup(r => r.ExistsAsync(4)).ReturnsAsync(true);
        _patientRepositoryMock.Setup(r => r.IsNameUniqueAsync(patient.Name, 4)).ReturnsAsync(true);
        _patientRepositoryMock.Setup(r => r.IsContactInfoUniqueAsync(patient.ContactInfo, 4)).ReturnsAsync(true);
        _patientRepositoryMock.Setup(r => r.UpdateAsync(patient)).Returns(Task.CompletedTask);

        var result = await _service.UpdatePatientAsync(patient);

        result.Should().Be(patient);
        _patientRepositoryMock.Verify(r => r.UpdateAsync(patient), Times.Once);
    }

    [Fact]
    public async Task DeletePatientAsync_ShouldThrowNotFoundException_WhenPatientMissing()
    {
        _patientRepositoryMock.Setup(r => r.GetByIdAsync(50)).ReturnsAsync((Patient?)null);

        Func<Task> act = async () => await _service.DeletePatientAsync(50);

        await act.Should().ThrowAsync<NotFoundException>();
    }

    [Fact]
    public async Task PatientExistsAsync_ShouldWrapException_AsInfrastructureException()
    {
        _patientRepositoryMock.Setup(r => r.ExistsAsync(1)).ThrowsAsync(new Exception("db"));

        Func<Task> act = async () => await _service.PatientExistsAsync(1);

        await act.Should().ThrowAsync<InfrastructureException>()
            .WithMessage("Failed to check patient existence.");
    }

    [Fact]
    public async Task UtilityMethods_ShouldDelegateToRepository()
    {
        _patientRepositoryMock.Setup(r => r.IsContactInfoUniqueAsync("x", 1)).ReturnsAsync(true);
        _patientRepositoryMock.Setup(r => r.IsNameUniqueAsync("n", 1)).ReturnsAsync(true);
        _patientRepositoryMock.Setup(r => r.GetPagedAsync(1, 10, "s", "name_desc"))
            .ReturnsAsync(new List<Patient> { new() { Id = 7, Name = "P", ContactInfo = "x" } });
        _patientRepositoryMock.Setup(r => r.GetCountAsync("s")).ReturnsAsync(1);

        var uniqueContact = await _service.IsContactInfoUniqueAsync("x", 1);
        var uniqueName = await _service.IsNameUniqueAsync("n", 1);
        var paged = await _service.GetPatientsPagedAsync(1, 10, "s", "name_desc");
        var count = await _service.GetPatientsCountAsync("s");

        uniqueContact.Should().BeTrue();
        uniqueName.Should().BeTrue();
        paged.Should().ContainSingle();
        count.Should().Be(1);
    }
}
