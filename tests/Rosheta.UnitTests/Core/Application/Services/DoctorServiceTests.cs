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
}