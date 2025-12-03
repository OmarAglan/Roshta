using FluentAssertions;
using Moq;
using Rosheta.Core.Application.Common.Exceptions;
using Rosheta.Core.Application.Contracts.Persistence;
using Rosheta.Core.Application.Services;
using Rosheta.Core.Domain.Entities;
using Xunit;

namespace Rosheta.UnitTests.Core.Application.Services;

public class DoctorServiceTests
{
    private readonly Mock<IDoctorRepository> _doctorRepositoryMock;
    private readonly DoctorService _service;

    public DoctorServiceTests()
    {
        // 1. Arrange: Setup the mock and the system under test (SUT)
        _doctorRepositoryMock = new Mock<IDoctorRepository>();
        _service = new DoctorService(_doctorRepositoryMock.Object);
    }

    [Fact]
    public async Task SaveDoctorProfileAsync_ShouldThrowValidationException_WhenNameIsEmpty()
    {
        // Arrange
        var invalidDoctor = new Doctor
        {
            Name = "", // Invalid
            Specialization = "Cardiology"
        };

        // Act
        // We use a Func here to capture the exception
        Func<Task> act = async () => await _service.SaveDoctorProfileAsync(invalidDoctor);

        // Assert
        await act.Should().ThrowAsync<ValidationException>()
            .WithMessage("Doctor name is required.");

        // Verify the repository was NEVER called (Pure unit test)
        _doctorRepositoryMock.Verify(r => r.SaveDoctorProfileAsync(It.IsAny<Doctor>()), Times.Never);
    }

    [Fact]
    public async Task SaveDoctorProfileAsync_ShouldCallRepository_WhenDataIsValid()
    {
        // Arrange
        var validDoctor = new Doctor
        {
            Name = "Dr. Gregory House",
            Specialization = "Diagnostic Medicine"
        };

        // Setup the mock to return the doctor when saved
        _doctorRepositoryMock
            .Setup(r => r.SaveDoctorProfileAsync(It.IsAny<Doctor>()))
            .ReturnsAsync(validDoctor);

        // Act
        var result = await _service.SaveDoctorProfileAsync(validDoctor);

        // Assert
        result.Should().NotBeNull();
        result.Name.Should().Be("Dr. Gregory House");

        // Verify the repository WAS called exactly once
        _doctorRepositoryMock.Verify(r => r.SaveDoctorProfileAsync(validDoctor), Times.Once);
    }
}