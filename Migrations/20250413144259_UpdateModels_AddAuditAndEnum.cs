using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Rosheta.Migrations
{
    /// <inheritdoc />
    public partial class UpdateModels_AddAuditAndEnum : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "Prescriptions",
                type: "TEXT",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "NextAppointmentDate",
                table: "Prescriptions",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "Prescriptions",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "Prescriptions",
                type: "TEXT",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "PrescriptionItems",
                type: "TEXT",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "Notes",
                table: "PrescriptionItems",
                type: "TEXT",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "PrescriptionItems",
                type: "TEXT",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "Patients",
                type: "TEXT",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<bool>(
                name: "HasOutstandingBalance",
                table: "Patients",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "Patients",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "LastVisitDate",
                table: "Patients",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "Patients",
                type: "TEXT",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<int>(
                name: "VisitCount",
                table: "Patients",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "Medications",
                type: "TEXT",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "Medications",
                type: "TEXT",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "ContactEmail",
                table: "Doctors",
                type: "TEXT",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ContactPhone",
                table: "Doctors",
                type: "TEXT",
                maxLength: 20,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "Doctors",
                type: "TEXT",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "Doctors",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsSubscribed",
                table: "Doctors",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "Doctors",
                type: "TEXT",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "Prescriptions");

            migrationBuilder.DropColumn(
                name: "NextAppointmentDate",
                table: "Prescriptions");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "Prescriptions");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "Prescriptions");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "PrescriptionItems");

            migrationBuilder.DropColumn(
                name: "Notes",
                table: "PrescriptionItems");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "PrescriptionItems");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "Patients");

            migrationBuilder.DropColumn(
                name: "HasOutstandingBalance",
                table: "Patients");

            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "Patients");

            migrationBuilder.DropColumn(
                name: "LastVisitDate",
                table: "Patients");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "Patients");

            migrationBuilder.DropColumn(
                name: "VisitCount",
                table: "Patients");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "Medications");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "Medications");

            migrationBuilder.DropColumn(
                name: "ContactEmail",
                table: "Doctors");

            migrationBuilder.DropColumn(
                name: "ContactPhone",
                table: "Doctors");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "Doctors");

            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "Doctors");

            migrationBuilder.DropColumn(
                name: "IsSubscribed",
                table: "Doctors");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "Doctors");
        }
    }
}
