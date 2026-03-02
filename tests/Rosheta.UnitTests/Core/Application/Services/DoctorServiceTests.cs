using FluentAssertions;
using Moq;
using Rosheta.Core.Application.Common.Exceptions;
using Rosheta.Core.Application.Contracts.Persistence;
using Rosheta.Core.Application.DTOs.Doctor;
using Rosheta.Core.Application.Services;
using Rosheta.Core.Domain.Entities;
using Xunit;

namespace Rosheta.UnitTests.Core.Application.Services;

public class DoctorServiceTests
{
    private readonly Mock<IDoctorRepository> _repoMock;
    private readonly DoctorService _service;

    public DoctorServiceTests()
    {
        _repoMock = new Mock<IDoctorRepository>();
        _service = new DoctorService(_repoMock.Object);
    }

    [Fact]
    public async Task SaveDoctorProfileAsync_ShouldThrowValidationException_WhenNameIsEmpty()
    {
        var invalidDoctor = new Doctor { Name = "", Specialization = "Cardiology" };
        Func<Task> act = async () => await _service.SaveDoctorProfileAsync(invalidDoctor);
        await act.Should().ThrowAsync<ValidationException>();
    }

    [Fact]
    public async Task SaveDoctorProfileAsync_ShouldAdd_WhenNoProfileExists()
    {
        // Arrange
        var newDoctor = new Doctor { Name = "Dr. House", Specialization = "Diagnostic" };

        // Mock: No existing profile
        _repoMock.Setup(r => r.GetDoctorProfileAsync()).ReturnsAsync((Doctor?)null);
        // Mock: Add returns the entity
        _repoMock.Setup(r => r.AddAsync(newDoctor)).ReturnsAsync(newDoctor);

        // Act
        var result = await _service.SaveDoctorProfileAsync(newDoctor);

        // Assert
        result.Should().Be(newDoctor);
        _repoMock.Verify(r => r.AddAsync(newDoctor), Times.Once);
        _repoMock.Verify(r => r.UpdateAsync(It.IsAny<Doctor>()), Times.Never);
    }

    [Fact]
    public async Task SaveDoctorProfileAsync_ShouldUpdate_WhenProfileExists()
    {
        // Arrange
        var inputDoctor = new Doctor { Name = "Dr. House Updated", Specialization = "Diagnostic" };
        var existingDoctor = new Doctor { Id = 1, Name = "Dr. House", Specialization = "Old Spec" };

        // Mock: Profile exists
        _repoMock.Setup(r => r.GetDoctorProfileAsync()).ReturnsAsync(existingDoctor);
        // Mock: Update returns Task
        _repoMock.Setup(r => r.UpdateAsync(existingDoctor)).Returns(Task.CompletedTask);

        // Act
        var result = await _service.SaveDoctorProfileAsync(inputDoctor);

        // Assert
        result.Id.Should().Be(1); // Should keep ID
        result.Name.Should().Be("Dr. House Updated"); // Should update name
        _repoMock.Verify(r => r.UpdateAsync(existingDoctor), Times.Once);
        _repoMock.Verify(r => r.AddAsync(It.IsAny<Doctor>()), Times.Never);
    }

    [Fact]
    public async Task GetDoctorProfileAsync_ShouldReturnDoctor_WhenRepositoryReturnsDoctor()
    {
        var doctor = new Doctor { Id = 2, Name = "Dr. Who", Specialization = "General" };
        _repoMock.Setup(r => r.GetDoctorProfileAsync()).ReturnsAsync(doctor);

        var result = await _service.GetDoctorProfileAsync();

        result.Should().NotBeNull();
        result!.Id.Should().Be(2);
    }

    [Fact]
    public async Task GetDoctorProfileAsync_ShouldThrowInfrastructureException_WhenRepositoryThrows()
    {
        _repoMock.Setup(r => r.GetDoctorProfileAsync()).ThrowsAsync(new Exception("db down"));

        Func<Task> act = async () => await _service.GetDoctorProfileAsync();

        await act.Should().ThrowAsync<InfrastructureException>()
            .WithMessage("Failed to retrieve doctor profile.");
    }

    [Fact]
    public async Task GetDoctorProfileAsync_ById_ShouldThrowNotFoundException_WhenMissing()
    {
        _repoMock.Setup(r => r.GetByIdAsync(99)).ReturnsAsync((Doctor?)null);

        Func<Task> act = async () => await _service.GetDoctorProfileAsync(99);

        await act.Should().ThrowAsync<NotFoundException>();
    }

    [Fact]
    public async Task UpdateDoctorProfileAsync_ShouldReturnTrue_WhenValid()
    {
        var existing = new Doctor { Id = 1, Name = "Old", Specialization = "OldSpec" };
        var dto = new UpdateDoctorProfileDto
        {
            Name = "New",
            Specialization = "Cardiology",
            LicenseNumber = "1234",
            Phone = "0100",
            Email = "dr@host.com"
        };

        _repoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(existing);
        _repoMock.Setup(r => r.UpdateAsync(existing)).Returns(Task.CompletedTask);

        var result = await _service.UpdateDoctorProfileAsync(1, dto);

        result.Should().BeTrue();
        existing.Name.Should().Be("New");
        existing.Specialization.Should().Be("Cardiology");
        _repoMock.Verify(r => r.UpdateAsync(existing), Times.Once);
    }

    [Fact]
    public async Task UpdateDoctorProfileAsync_ShouldThrowValidationException_WhenSpecializationMissing()
    {
        var dto = new UpdateDoctorProfileDto { Name = "Dr.", Specialization = "" };

        Func<Task> act = async () => await _service.UpdateDoctorProfileAsync(1, dto);

        await act.Should().ThrowAsync<ValidationException>()
            .WithMessage("Doctor specialization is required.");
    }
}
