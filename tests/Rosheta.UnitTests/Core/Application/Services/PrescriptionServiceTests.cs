using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using Rosheta.Core.Application.Common.Exceptions;
using Rosheta.Core.Application.Common.Validation;
using Rosheta.Core.Application.Contracts.Persistence;
using Rosheta.Core.Application.DTOs;
using Rosheta.Core.Application.Services;
using Rosheta.Core.Domain.Entities;
using Rosheta.Core.Domain.Enums;
using Xunit;

namespace Rosheta.UnitTests.Core.Application.Services;

public class PrescriptionServiceTests
{
    private readonly Mock<IPrescriptionRepository> _prescriptionRepoMock;
    private readonly Mock<IPatientRepository> _patientRepoMock;
    private readonly Mock<ILogger<PrescriptionService>> _loggerMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly IValidationService _validationService;
    private readonly PrescriptionService _service;

    public PrescriptionServiceTests()
    {
        _prescriptionRepoMock = new Mock<IPrescriptionRepository>();
        _patientRepoMock = new Mock<IPatientRepository>();
        _loggerMock = new Mock<ILogger<PrescriptionService>>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _unitOfWorkMock
            .Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);
        _validationService = TestValidationServiceFactory.Create();

        _service = new PrescriptionService(
            _prescriptionRepoMock.Object,
            _patientRepoMock.Object,
            _loggerMock.Object,
            _unitOfWorkMock.Object,
            _validationService
        );
    }

    [Fact]
    public async Task CreatePrescriptionAsync_ShouldThrowValidationException_WhenNoItemsProvided()
    {
        // Arrange
        var model = new PrescriptionCreateModel
        {
            PatientId = 1,
            Items = new List<PrescriptionCreateModel.PrescriptionItemCreateModel>() // Empty list
        };

        // Act
        Func<Task> act = async () => await _service.CreatePrescriptionAsync(model, doctorId: 1);

        // Assert
        await act.Should().ThrowAsync<ValidationException>()
            .WithMessage("Items: The prescription must contain at least one medication item.");
    }

    [Fact]
    public async Task CreatePrescriptionAsync_ShouldThrowNotFoundException_WhenPatientDoesNotExist()
    {
        // Arrange
        var model = new PrescriptionCreateModel
        {
            PatientId = 999, // Non-existent
            Items = new List<PrescriptionCreateModel.PrescriptionItemCreateModel>
            {
                new() { MedicationId = 1, Quantity = "1", Instructions = "Take it" }
            }
        };

        // Mock patient repo to return false for Exists
        _patientRepoMock.Setup(r => r.ExistsAsync(model.PatientId)).ReturnsAsync(false);

        // Act
        Func<Task> act = async () => await _service.CreatePrescriptionAsync(model, doctorId: 1);

        // Assert
        await act.Should().ThrowAsync<NotFoundException>();
    }

    [Fact]
    public async Task CreatePrescriptionAsync_ShouldCreate_WhenDataIsValid()
    {
        // Arrange
        var model = new PrescriptionCreateModel
        {
            PatientId = 1,
            Items = new List<PrescriptionCreateModel.PrescriptionItemCreateModel>
            {
                new() { MedicationId = 10, Quantity = "1 strip", Instructions = "Daily" }
            }
        };

        _patientRepoMock.Setup(r => r.ExistsAsync(model.PatientId)).ReturnsAsync(true);
        _prescriptionRepoMock
            .Setup(r => r.AddAsync(It.IsAny<Prescription>()))
            .ReturnsAsync((Prescription p) => { p.Id = 100; return p; }); // Simulate DB ID generation

        // Act
        var result = await _service.CreatePrescriptionAsync(model, doctorId: 5);

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be(100);
        result.DoctorId.Should().Be(5);
        result.PrescriptionItems.Should().HaveCount(1);
        result.PrescriptionItems.First().MedicationId.Should().Be(10);
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task GetAllPrescriptionsAsync_ShouldReturnData_WhenRepositorySucceeds()
    {
        _prescriptionRepoMock.Setup(r => r.GetAllAsync())
            .ReturnsAsync(new List<Prescription> { new() { Id = 1 } });

        var result = await _service.GetAllPrescriptionsAsync();

        result.Should().ContainSingle();
    }

    [Fact]
    public async Task SearchPrescriptionsAsync_ShouldReturnData_WhenRepositorySucceeds()
    {
        _prescriptionRepoMock.Setup(r => r.SearchAsync("ah"))
            .ReturnsAsync(new List<Prescription> { new() { Id = 2 } });

        var result = await _service.SearchPrescriptionsAsync("ah");

        result.Should().ContainSingle(x => x.Id == 2);
    }

    [Fact]
    public async Task GetPrescriptionByIdAsync_ShouldThrowNotFoundException_WhenMissing()
    {
        _prescriptionRepoMock.Setup(r => r.GetByIdAsync(99)).ReturnsAsync((Prescription?)null);

        Func<Task> act = async () => await _service.GetPrescriptionByIdAsync(99);

        await act.Should().ThrowAsync<NotFoundException>();
    }

    [Fact]
    public async Task CancelPrescriptionAsync_ShouldThrowBusinessRuleException_WhenAlreadyCancelled()
    {
        _prescriptionRepoMock.Setup(r => r.GetByIdAsync(1))
            .ReturnsAsync(new Prescription { Id = 1, Status = PrescriptionStatus.Cancelled });

        Func<Task> act = async () => await _service.CancelPrescriptionAsync(1);

        await act.Should().ThrowAsync<BusinessRuleException>()
            .WithMessage("Cannot cancel a prescription that is already cancelled.");
    }

    [Fact]
    public async Task CancelPrescriptionAsync_ShouldThrowBusinessRuleException_WhenFilled()
    {
        _prescriptionRepoMock.Setup(r => r.GetByIdAsync(1))
            .ReturnsAsync(new Prescription { Id = 1, Status = PrescriptionStatus.Filled });

        Func<Task> act = async () => await _service.CancelPrescriptionAsync(1);

        await act.Should().ThrowAsync<BusinessRuleException>()
            .WithMessage("Cannot cancel a prescription that has been filled.");
    }

    [Fact]
    public async Task CancelPrescriptionAsync_ShouldReturnTrue_WhenActive()
    {
        _prescriptionRepoMock.Setup(r => r.GetByIdAsync(1))
            .ReturnsAsync(new Prescription { Id = 1, Status = PrescriptionStatus.Active });
        _prescriptionRepoMock.Setup(r => r.CancelAsync(1)).ReturnsAsync(true);

        var result = await _service.CancelPrescriptionAsync(1);

        result.Should().BeTrue();
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task PagingMethods_ShouldDelegateToRepository()
    {
        _prescriptionRepoMock.Setup(r => r.GetPagedAsync(1, 10, "ah", "date_desc"))
            .ReturnsAsync(new List<Prescription> { new() { Id = 5 } });
        _prescriptionRepoMock.Setup(r => r.GetCountAsync("ah")).ReturnsAsync(1);

        var paged = await _service.GetPrescriptionsPagedAsync(1, 10, "ah", "date_desc");
        var count = await _service.GetPrescriptionsCountAsync("ah");

        paged.Should().ContainSingle(x => x.Id == 5);
        count.Should().Be(1);
    }

    [Fact]
    public async Task GetAllPrescriptionsAsync_ShouldWrapException_AsInfrastructureException()
    {
        _prescriptionRepoMock.Setup(r => r.GetAllAsync()).ThrowsAsync(new Exception("db"));

        Func<Task> act = async () => await _service.GetAllPrescriptionsAsync();

        await act.Should().ThrowAsync<InfrastructureException>()
            .WithMessage("Failed to retrieve prescriptions.");
    }
}
