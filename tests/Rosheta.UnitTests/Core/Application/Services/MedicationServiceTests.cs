using FluentAssertions;
using Moq;
using Rosheta.Core.Application.Common.Exceptions;
using Rosheta.Core.Application.Common.Validation;
using Rosheta.Core.Application.Contracts.Persistence;
using Rosheta.Core.Application.Services;
using Rosheta.Core.Domain.Entities;
using Xunit;

namespace Rosheta.UnitTests.Core.Application.Services;

public class MedicationServiceTests
{
    private readonly Mock<IMedicationRepository> _repoMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly IValidationService _validationService;
    private readonly MedicationService _service;

    public MedicationServiceTests()
    {
        _repoMock = new Mock<IMedicationRepository>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _unitOfWorkMock
            .Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);
        _validationService = TestValidationServiceFactory.Create();

        _service = new MedicationService(_repoMock.Object, _unitOfWorkMock.Object, _validationService);
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
            .WithMessage("*Medication name is required.*");
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
    public async Task UpdateMedicationAsync_ShouldCallRepository_WhenValid()
    {
        // Arrange
        var medication = new Medication { Id = 1, Name = "Updated Name" };

        _repoMock.Setup(r => r.ExistsAsync(1)).ReturnsAsync(true);
        _repoMock.Setup(r => r.IsNameUniqueAsync(medication.Name, 1)).ReturnsAsync(true);
        // NEW: UpdateAsync returns Task, so we use Returns(Task.CompletedTask)
        _repoMock.Setup(r => r.UpdateAsync(medication)).Returns(Task.CompletedTask);

        // Act
        var result = await _service.UpdateMedicationAsync(medication);

        // Assert
        result.Should().BeEquivalentTo(medication);
        _repoMock.Verify(r => r.UpdateAsync(medication), Times.Once);
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
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

    [Fact]
    public async Task DeleteMedicationAsync_ShouldCallDelete_WhenExists()
    {
        // Arrange
        var medication = new Medication { Id = 1, Name = "To Delete" };
        // We must mock GetByIdAsync because the service now fetches before deleting
        _repoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(medication);
        _repoMock.Setup(r => r.DeleteAsync(medication)).Returns(Task.CompletedTask);

        // Act
        var result = await _service.DeleteMedicationAsync(1);

        // Assert
        result.Should().BeTrue();
        _repoMock.Verify(r => r.DeleteAsync(medication), Times.Once);
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task GetMedicationByIdAsync_ShouldThrowNotFoundException_WhenMissing()
    {
        _repoMock.Setup(r => r.GetByIdAsync(123)).ReturnsAsync((Medication?)null);

        Func<Task> act = async () => await _service.GetMedicationByIdAsync(123);

        await act.Should().ThrowAsync<NotFoundException>();
    }

    [Fact]
    public async Task GetAllMedicationsAsync_ShouldReturnData_WhenRepositorySucceeds()
    {
        _repoMock.Setup(r => r.GetAllAsync())
            .ReturnsAsync(new List<Medication> { new() { Id = 1, Name = "Aspirin" } });

        var result = await _service.GetAllMedicationsAsync();

        result.Should().ContainSingle();
    }

    [Fact]
    public async Task SearchMedicationsAsync_ShouldReturnData_WhenRepositorySucceeds()
    {
        _repoMock.Setup(r => r.SearchAsync("asp"))
            .ReturnsAsync(new List<Medication> { new() { Id = 1, Name = "Aspirin" } });

        var result = await _service.SearchMedicationsAsync("asp");

        result.Should().ContainSingle(x => x.Name == "Aspirin");
    }

    [Fact]
    public async Task DeleteMedicationAsync_ShouldThrowNotFoundException_WhenMissing()
    {
        _repoMock.Setup(r => r.GetByIdAsync(404)).ReturnsAsync((Medication?)null);

        Func<Task> act = async () => await _service.DeleteMedicationAsync(404);

        await act.Should().ThrowAsync<NotFoundException>();
    }

    [Fact]
    public async Task UtilityMethods_ShouldDelegateToRepository()
    {
        _repoMock.Setup(r => r.ExistsAsync(1)).ReturnsAsync(true);
        _repoMock.Setup(r => r.IsNameUniqueAsync("Aspirin", 1)).ReturnsAsync(true);
        _repoMock.Setup(r => r.GetPagedAsync(1, 10, "asp", "name_desc"))
            .ReturnsAsync(new List<Medication> { new() { Id = 1, Name = "Aspirin" } });
        _repoMock.Setup(r => r.GetCountAsync("asp")).ReturnsAsync(1);

        var exists = await _service.MedicationExistsAsync(1);
        var unique = await _service.IsNameUniqueAsync("Aspirin", 1);
        var paged = await _service.GetMedicationsPagedAsync(1, 10, "asp", "name_desc");
        var count = await _service.GetMedicationsCountAsync("asp");

        exists.Should().BeTrue();
        unique.Should().BeTrue();
        paged.Should().ContainSingle();
        count.Should().Be(1);
    }

    [Fact]
    public async Task GetAllMedicationsAsync_ShouldWrapException_AsInfrastructureException()
    {
        _repoMock.Setup(r => r.GetAllAsync()).ThrowsAsync(new Exception("db"));

        Func<Task> act = async () => await _service.GetAllMedicationsAsync();

        await act.Should().ThrowAsync<InfrastructureException>()
            .WithMessage("Failed to retrieve medications.");
    }
}
