using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Rosheta.Core;
using Rosheta.Core.Application.Common.Exceptions;
using Rosheta.Core.Application.Contracts.Services;
using Rosheta.Core.Application.DTOs;
using Rosheta.Core.Application.DTOs.Doctor;
using Rosheta.Core.Domain.Entities;
using Rosheta.Core.Domain.Enums;
using System.ComponentModel.DataAnnotations;
using Xunit;

namespace Rosheta.UnitTests.Core;

public class CoreCoverageBoostTests
{
    [Fact]
    public void AddApplicationServices_ShouldRegisterExpectedServices()
    {
        var services = new ServiceCollection();

        services.AddApplicationServices();

        services.Should().Contain(d => d.ServiceType == typeof(IDoctorService));
        services.Should().Contain(d => d.ServiceType == typeof(IPatientService));
        services.Should().Contain(d => d.ServiceType == typeof(IMedicationService));
        services.Should().Contain(d => d.ServiceType == typeof(IPrescriptionService));
        services.Should().Contain(d => d.ServiceType == typeof(ILicenseService));
        services.Should().Contain(d => d.ServiceType == typeof(ISettingsService));
    }

    [Fact]
    public void Patient_Validate_ShouldReturnExpectedErrors_ForInvalidDates()
    {
        var patient = new Patient
        {
            Name = "Ahmed",
            DateOfBirth = DateTime.Today.AddDays(1),
            LastVisitDate = DateTime.Today.AddDays(1)
        };

        var results = patient.Validate(new ValidationContext(patient)).ToList();

        results.Should().HaveCount(2);
        results.Should().Contain(r => r.ErrorMessage == "Date of Birth must be in the past.");
        results.Should().Contain(r => r.ErrorMessage == "Last Visit Date cannot be in the future.");
    }

    [Fact]
    public void Patient_Validate_ShouldReturnNoErrors_ForValidDates()
    {
        var patient = new Patient
        {
            Name = "Naguib",
            DateOfBirth = DateTime.Today.AddYears(-30),
            LastVisitDate = DateTime.Today.AddDays(-1)
        };

        var results = patient.Validate(new ValidationContext(patient)).ToList();

        results.Should().BeEmpty();
    }

    [Fact]
    public void PrescriptionCreateModel_Validate_ShouldReturnExpectedErrors_WhenInvalid()
    {
        var model = new PrescriptionCreateModel
        {
            PatientId = 1,
            ExpiryDate = DateTime.Today,
            NextAppointmentDate = DateTime.Today
        };

        var results = model.Validate(new ValidationContext(model)).ToList();

        results.Should().HaveCount(3);
        results.Should().Contain(r => r.ErrorMessage == "Expiry Date must be in the future.");
        results.Should().Contain(r => r.ErrorMessage == "Next Appointment Date must be in the future.");
        results.Should().Contain(r => r.ErrorMessage == "The prescription must contain at least one medication item.");
    }

    [Fact]
    public void DtosAndExceptionConstructors_ShouldBehaveAsExpected()
    {
        var med = new MedicationSearchDto { Id = 1, Name = "Panadol", Dosage = "500mg" };
        var patient = new PatientSearchDto { Id = 2, Name = "Ahmed", ContactInfo = "0100" };
        var rx = new PrescriptionSearchDto
        {
            Id = 3,
            PatientName = "Ahmed",
            Date = new DateTime(2025, 1, 1),
            Status = PrescriptionStatus.Active
        };
        var doctor = new UpdateDoctorProfileDto
        {
            Name = "Dr. House",
            Specialization = "Diagnostic",
            LicenseNumber = "1234",
            Phone = "0100",
            Email = "dr@example.com"
        };

        var item = new PrescriptionCreateModel.PrescriptionItemCreateModel
        {
            MedicationId = 9,
            Dosage = "250mg",
            Frequency = "BID",
            Duration = "5 days",
            Quantity = "10 tablets",
            Instructions = "After meals",
            Refills = 2,
            Notes = "Do not skip dose"
        };

        med.Name.Should().Be("Panadol");
        patient.ContactInfo.Should().Be("0100");
        rx.Status.Should().Be(PrescriptionStatus.Active);
        doctor.Specialization.Should().Be("Diagnostic");
        item.Refills.Should().Be(2);
        item.Notes.Should().Be("Do not skip dose");

        var validationErrors = new Dictionary<string, string[]> { ["Name"] = new[] { "Required" } };
        var vex = new Rosheta.Core.Application.Common.Exceptions.ValidationException("invalid", validationErrors);
        var notFound = new NotFoundException("custom message");
        var infra = new InfrastructureException("infra");
        var business = new BusinessRuleException("rule", new InvalidOperationException("inner"));

        vex.Errors.Should().ContainKey("Name");
        notFound.Message.Should().Be("custom message");
        infra.Message.Should().Be("infra");
        business.InnerException.Should().NotBeNull();
    }
}
