using FluentAssertions;
using Moq;
using Rosheta.Core.Application.Common.Exceptions;
using Rosheta.Core.Application.Contracts.Persistence;
using Rosheta.Core.Application.Services;
using Rosheta.Core.Domain.Entities;
using Xunit;

namespace Rosheta.UnitTests.Core.Application.Services;

public class MedicationServiceTests
{
    private readonly Mock<IMedicationRepository> _repoMock;
    private readonly MedicationService _service;

    public MedicationServiceTests()
    {
        _repoMock = new Mock<IMedicationRepository>();
        _service = new MedicationService(_repoMock.Object);
    }

    [Fact]
    public async Task AddMedicationAsync_ShouldThrowValidationException_WhenNameIsEmpty()
    {
        // Arrange
        var medication = new Medication { Name = "", Dosage = "500mg" };

        // Act
        Func<Task> act = async () => await _service.AddMedicationAsync(medication);

        // Assert
        await act.Should().ThrowAsync<ValidationException>()
            .WithMessage("Medication name is required.");
    }

    [Fact]
    public async Task AddMedicationAsync_ShouldThrowBusinessRuleException_WhenNameDuplicate()
    {
        // Arrange
        var medication = new Medication { Name = "Panadol", Dosage = "500mg" };

        // Mock repo to say "Not Unique" (false)
        _repoMock.Setup(r => r.IsNameUniqueAsync(medication.Name, null))
            .ReturnsAsync(false);

        // Act
        Func<Task> act = async () => await _service.AddMedicationAsync(medication);

        // Assert
        await act.Should().ThrowAsync<BusinessRuleException>()
            .WithMessage($"A medication with the name '{medication.Name}' already exists.");
    }

    [Fact]
    public async Task UpdateMedicationAsync_ShouldThrowNotFoundException_WhenIdDoesNotExist()
    {
        // Arrange
        var medication = new Medication { Id = 999, Name = "Valid Name" };

        // Mock Exists check
        _repoMock.Setup(r => r.ExistsAsync(medication.Id)).ReturnsAsync(false);

        // Act
        Func<Task> act = async () => await _service.UpdateMedicationAsync(medication);

        // Assert
        await act.Should().ThrowAsync<NotFoundException>()
            .WithMessage("*Entity \"Medication\" with key (999) was not found.*");
    }

    [Fact]
    public async Task GetMedicationByIdAsync_ShouldReturnMedication_WhenExists()
    {
        // Arrange
        var medication = new Medication { Id = 1, Name = "Aspirin" };
        _repoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(medication);

        // Act
        var result = await _service.GetMedicationByIdAsync(1);

        // Assert
        result.Should().NotBeNull();
        result!.Name.Should().Be("Aspirin");
    }
}