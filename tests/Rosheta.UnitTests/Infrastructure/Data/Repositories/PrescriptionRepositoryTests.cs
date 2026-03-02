using FluentAssertions;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Rosheta.Core.Domain.Entities;
using Rosheta.Core.Domain.Enums;
using Rosheta.Infrastructure.Data;
using Rosheta.Infrastructure.Data.Repositories;
using Xunit;

namespace Rosheta.UnitTests.Infrastructure.Data.Repositories;

public class PrescriptionRepositoryTests
{
    static PrescriptionRepositoryTests()
    {
        SQLitePCL.Batteries.Init();
    }

    [Fact]
    public async Task GetByIdAsync_ShouldIncludePatientDoctorItemsAndMedication()
    {
        using var fixture = new SqlitePrescriptionFixture();
        var prescription = await AddPrescriptionAsync(fixture.DbContext, patientId: 1, dateIssued: new DateTime(2025, 1, 1));

        var result = await fixture.Repository.GetByIdAsync(prescription.Id);

        result.Should().NotBeNull();
        result!.Patient.Should().NotBeNull();
        result.Doctor.Should().NotBeNull();
        result.PrescriptionItems.Should().HaveCount(1);
        result.PrescriptionItems.First().Medication.Should().NotBeNull();
    }

    [Fact]
    public async Task GetAllAsync_ShouldReturnPrescriptionsInDescendingDateOrder_WithPatientIncluded()
    {
        using var fixture = new SqlitePrescriptionFixture();
        await AddPrescriptionAsync(fixture.DbContext, patientId: 1, dateIssued: new DateTime(2025, 1, 1));
        await AddPrescriptionAsync(fixture.DbContext, patientId: 2, dateIssued: new DateTime(2025, 1, 3));
        await AddPrescriptionAsync(fixture.DbContext, patientId: 3, dateIssued: new DateTime(2025, 1, 2));

        var results = (await fixture.Repository.GetAllAsync()).ToList();

        results.Should().HaveCount(3);
        results.Select(x => x.DateIssued).Should().BeInDescendingOrder();
        results.Should().OnlyContain(x => x.Patient != null);
    }

    [Fact]
    public async Task SearchAsync_ShouldFilterByPatientName_CaseInsensitive()
    {
        using var fixture = new SqlitePrescriptionFixture();
        await AddPrescriptionAsync(fixture.DbContext, patientId: 1, dateIssued: new DateTime(2025, 1, 1)); // Ahmed Zewail
        await AddPrescriptionAsync(fixture.DbContext, patientId: 2, dateIssued: new DateTime(2025, 1, 2)); // Naguib Mahfouz

        var results = (await fixture.Repository.SearchAsync("ahMeD")).ToList();

        results.Should().HaveCount(1);
        results[0].Patient.Should().NotBeNull();
        results[0].Patient.Name.Should().Contain("Ahmed");
    }

    [Fact]
    public async Task GetCountAsync_ShouldReturnFilteredCount_WhenSearchTermProvided()
    {
        using var fixture = new SqlitePrescriptionFixture();
        await AddPrescriptionAsync(fixture.DbContext, patientId: 1, dateIssued: new DateTime(2025, 1, 1)); // Ahmed
        await AddPrescriptionAsync(fixture.DbContext, patientId: 1, dateIssued: new DateTime(2025, 1, 2)); // Ahmed
        await AddPrescriptionAsync(fixture.DbContext, patientId: 2, dateIssued: new DateTime(2025, 1, 3)); // Naguib

        var allCount = await fixture.Repository.GetCountAsync();
        var ahmedCount = await fixture.Repository.GetCountAsync("ahmed");

        allCount.Should().Be(3);
        ahmedCount.Should().Be(2);
    }

    [Fact]
    public async Task GetPagedAsync_ShouldApplyPagingSortingFiltering_AndIncludePatientAndDoctor()
    {
        using var fixture = new SqlitePrescriptionFixture();
        await AddPrescriptionAsync(fixture.DbContext, patientId: 1, dateIssued: new DateTime(2025, 1, 1)); // Ahmed
        await AddPrescriptionAsync(fixture.DbContext, patientId: 2, dateIssued: new DateTime(2025, 1, 3)); // Naguib
        await AddPrescriptionAsync(fixture.DbContext, patientId: 1, dateIssued: new DateTime(2025, 1, 2)); // Ahmed

        var firstPageAscending = await fixture.Repository.GetPagedAsync(pageNumber: 1, pageSize: 2, sortOrder: "Date");
        var filteredByAhmed = await fixture.Repository.GetPagedAsync(pageNumber: 1, pageSize: 10, searchTerm: "ahmed");

        firstPageAscending.Should().HaveCount(2);
        firstPageAscending.Select(x => x.DateIssued).Should().BeInAscendingOrder();
        firstPageAscending.Should().OnlyContain(x => x.Patient != null && x.Doctor != null);

        filteredByAhmed.Should().HaveCount(2);
        filteredByAhmed.Should().OnlyContain(x => x.Patient.Name.Contains("Ahmed"));
    }

    [Fact]
    public async Task CancelAsync_ShouldUpdateStatusAndReturnExpectedResults()
    {
        using var fixture = new SqlitePrescriptionFixture();
        var prescription = await AddPrescriptionAsync(
            fixture.DbContext,
            patientId: 1,
            dateIssued: new DateTime(2025, 1, 1),
            status: PrescriptionStatus.Active);

        var firstCancel = await fixture.Repository.CancelAsync(prescription.Id);
        var secondCancel = await fixture.Repository.CancelAsync(prescription.Id);
        var missingCancel = await fixture.Repository.CancelAsync(9999);

        var updated = await fixture.DbContext.Prescriptions.FirstAsync(p => p.Id == prescription.Id);

        firstCancel.Should().BeTrue();
        secondCancel.Should().BeTrue();
        missingCancel.Should().BeFalse();
        updated.Status.Should().Be(PrescriptionStatus.Cancelled);
    }

    private static async Task<Prescription> AddPrescriptionAsync(
        ApplicationDbContext context,
        int patientId,
        DateTime dateIssued,
        PrescriptionStatus status = PrescriptionStatus.Active)
    {
        var prescription = new Prescription
        {
            PatientId = patientId,
            DoctorId = 1,
            DateIssued = dateIssued,
            ExpiryDate = dateIssued.AddDays(7),
            NextAppointmentDate = dateIssued.AddDays(3),
            Status = status
        };

        prescription.PrescriptionItems.Add(new PrescriptionItem
        {
            MedicationId = 1,
            Quantity = "1 box",
            Instructions = "Take one daily"
        });

        context.Prescriptions.Add(prescription);
        await context.SaveChangesAsync();
        return prescription;
    }

    private sealed class SqlitePrescriptionFixture : IDisposable
    {
        public SqliteConnection Connection { get; }
        public ApplicationDbContext DbContext { get; }
        public PrescriptionRepository Repository { get; }

        public SqlitePrescriptionFixture()
        {
            Connection = new SqliteConnection("DataSource=:memory:");
            Connection.Open();

            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseSqlite(Connection)
                .Options;

            DbContext = new ApplicationDbContext(options);
            DbContext.Database.EnsureCreated();

            Repository = new PrescriptionRepository(DbContext);
        }

        public void Dispose()
        {
            DbContext.Dispose();
            Connection.Dispose();
        }
    }
}
