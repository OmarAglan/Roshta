using Microsoft.EntityFrameworkCore;
using Roshta.Models.Entities;
using Roshta.Models.Base;
using System.Reflection.Emit;

namespace Roshta.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    // Define DbSet properties for each of your entities
    public DbSet<Patient> Patients { get; set; } = default!;
    public DbSet<Doctor> Doctors { get; set; } = default!;
    public DbSet<Medication> Medications { get; set; } = default!;
    public DbSet<Prescription> Prescriptions { get; set; } = default!;
    public DbSet<PrescriptionItem> PrescriptionItems { get; set; } = default!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configure relationships and constraints here if not using Data Annotations
        // or if you need more complex configurations.

        // Example: If you wanted a unique constraint on Doctor LicenseNumber
        // modelBuilder.Entity<Doctor>()
        //     .HasIndex(d => d.LicenseNumber)
        //     .IsUnique();

        // Relationships are mostly inferred by EF Core based on navigation properties
        // and foreign key attributes used in the models, but you can be explicit:

        modelBuilder.Entity<Prescription>()
            .HasOne(p => p.Patient)
            .WithMany(pt => pt.Prescriptions)
            .HasForeignKey(p => p.PatientId);

        modelBuilder.Entity<Prescription>()
            .HasOne(p => p.Doctor)
            .WithMany(d => d.Prescriptions)
            .HasForeignKey(p => p.DoctorId);

        modelBuilder.Entity<PrescriptionItem>()
            .HasOne(pi => pi.Prescription)
            .WithMany(p => p.PrescriptionItems)
            .HasForeignKey(pi => pi.PrescriptionId);

        modelBuilder.Entity<PrescriptionItem>()
            .HasOne(pi => pi.Medication)
            .WithMany(m => m.PrescriptionItems)
            .HasForeignKey(pi => pi.MedicationId);

        // --- Seed Data ---
        var seedDate = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc); // Consistent seed date

        modelBuilder.Entity<Doctor>().HasData(
            new Doctor
            {
                Id = 1, // Explicit ID for seeding
                Name = "Dr. Default",
                Specialization = "General Practice", // Corrected property name
                LicenseNumber = "0000", // Placeholder
                ContactPhone = "01000000000",
                ContactEmail = "dr.default@roshta.app",
                IsSubscribed = true, // Assuming the default doctor is subscribed
                CreatedAt = seedDate,
                UpdatedAt = seedDate
            }
        );

        modelBuilder.Entity<Patient>().HasData(
            new Patient { Id = 1, Name = "Ahmed Zewail", DateOfBirth = new DateTime(1946, 2, 26), ContactInfo = "ahmed.zewail@example.com", VisitCount = 5, LastVisitDate = seedDate.AddDays(-10), HasOutstandingBalance = false, IsActive = true, CreatedAt = seedDate, UpdatedAt = seedDate },
            new Patient { Id = 2, Name = "Naguib Mahfouz", DateOfBirth = new DateTime(1911, 12, 11), ContactInfo = "01112345678", VisitCount = 2, LastVisitDate = seedDate.AddDays(-30), HasOutstandingBalance = true, IsActive = true, CreatedAt = seedDate, UpdatedAt = seedDate },
            new Patient { Id = 3, Name = "Umm Kulthum", DateOfBirth = new DateTime(1904, 5, 4), ContactInfo = "umm.kulthum@diva.net", VisitCount = 10, LastVisitDate = seedDate.AddDays(-5), HasOutstandingBalance = false, IsActive = false, CreatedAt = seedDate, UpdatedAt = seedDate } // Inactive patient example
        );

        modelBuilder.Entity<Medication>().HasData(
            new Medication { Id = 1, Name = "Panadol Extra", Dosage = "500mg/65mg", Form = "Tablet", Manufacturer = "GSK", CreatedAt = seedDate, UpdatedAt = seedDate },
            new Medication { Id = 2, Name = "Amoxicillin", Dosage = "500mg", Form = "Capsule", Manufacturer = "Generic Pharma", CreatedAt = seedDate, UpdatedAt = seedDate },
            new Medication { Id = 3, Name = "Ventolin Inhaler", Dosage = "100mcg/puff", Form = "Inhaler", Manufacturer = "GSK", CreatedAt = seedDate, UpdatedAt = seedDate },
            new Medication { Id = 4, Name = "Cataflam", Dosage = "50mg", Form = "Tablet", Manufacturer = "Novartis", CreatedAt = seedDate, UpdatedAt = seedDate }
        );
        // -----------------

    }

    // Override SaveChanges to update audit fields
    public override int SaveChanges(bool acceptAllChangesOnSuccess)
    {
        OnBeforeSaving();
        return base.SaveChanges(acceptAllChangesOnSuccess);
    }

    public override Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default)
    {
        OnBeforeSaving();
        return base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
    }

    private void OnBeforeSaving()
    {
        var entries = ChangeTracker.Entries();
        var utcNow = DateTime.UtcNow;

        foreach (var entry in entries)
        {
            // Use interface instead of type checking each entity
            if (entry.Entity is IAuditable auditableEntity)
            {
                switch (entry.State)
                {
                    case EntityState.Modified:
                        auditableEntity.UpdatedAt = utcNow;
                        break;

                    case EntityState.Added:
                        auditableEntity.CreatedAt = utcNow;
                        auditableEntity.UpdatedAt = utcNow;
                        break;
                }
            }
        }
    }
}
