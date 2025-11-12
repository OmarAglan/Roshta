using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Rosheta.Migrations
{
    /// <inheritdoc />
    public partial class AddSeedData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Doctors",
                columns: new[] { "Id", "ContactEmail", "ContactPhone", "CreatedAt", "IsActive", "IsSubscribed", "LicenseNumber", "Name", "Specialization", "UpdatedAt" },
                values: new object[] { 1, "dr.default@roshta.app", "01000000000", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), true, true, "0000", "Dr. Default", "General Practice", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.InsertData(
                table: "Medications",
                columns: new[] { "Id", "CreatedAt", "Dosage", "Form", "Manufacturer", "Name", "UpdatedAt" },
                values: new object[,]
                {
                    { 1, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "500mg/65mg", "Tablet", "GSK", "Panadol Extra", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 2, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "500mg", "Capsule", "Generic Pharma", "Amoxicillin", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 3, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "100mcg/puff", "Inhaler", "GSK", "Ventolin Inhaler", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 4, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "50mg", "Tablet", "Novartis", "Cataflam", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) }
                });

            migrationBuilder.InsertData(
                table: "Patients",
                columns: new[] { "Id", "ContactInfo", "CreatedAt", "DateOfBirth", "HasOutstandingBalance", "IsActive", "LastVisitDate", "Name", "UpdatedAt", "VisitCount" },
                values: new object[,]
                {
                    { 1, "ahmed.zewail@example.com", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(1946, 2, 26, 0, 0, 0, 0, DateTimeKind.Unspecified), false, true, new DateTime(2023, 12, 22, 0, 0, 0, 0, DateTimeKind.Utc), "Ahmed Zewail", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 5 },
                    { 2, "01112345678", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(1911, 12, 11, 0, 0, 0, 0, DateTimeKind.Unspecified), true, true, new DateTime(2023, 12, 2, 0, 0, 0, 0, DateTimeKind.Utc), "Naguib Mahfouz", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 2 },
                    { 3, "umm.kulthum@diva.net", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(1904, 5, 4, 0, 0, 0, 0, DateTimeKind.Unspecified), false, false, new DateTime(2023, 12, 27, 0, 0, 0, 0, DateTimeKind.Utc), "Umm Kulthum", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 10 }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Doctors",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Medications",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Medications",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Medications",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Medications",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Patients",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Patients",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Patients",
                keyColumn: "Id",
                keyValue: 3);
        }
    }
}
