using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using Rosheta.Core.Application.Common.Exceptions;
using Rosheta.Core.Application.Contracts.Persistence;
using Rosheta.Core.Application.DTOs;
using Rosheta.Core.Application.Services;
using Rosheta.Core.Domain.Entities;
using Xunit;

namespace Rosheta.UnitTests.Core.Application.Services;

public class PrescriptionServiceTests
{
    private readonly Mock<IPrescriptionRepository> _prescriptionRepoMock;
    private readonly Mock<IPatientRepository> _patientRepoMock;
    private readonly Mock<ILogger<PrescriptionService>> _loggerMock;
    private readonly PrescriptionService _service;

    public PrescriptionServiceTests()
    {
        _prescriptionRepoMock = new Mock<IPrescriptionRepository>();
        _patientRepoMock = new Mock<IPatientRepository>();
        _loggerMock = new Mock<ILogger<PrescriptionService>>();

        _service = new PrescriptionService(
            _prescriptionRepoMock.Object,
            _patientRepoMock.Object,
            _loggerMock.Object
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
            .WithMessage("Prescription must have at least one medication item.");
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
    }
}