using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Rosheta.Migrations
{
    /// <inheritdoc />
    public partial class AddDosageFrequencyDurationToItem : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Dosage",
                table: "PrescriptionItems",
                type: "TEXT",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Duration",
                table: "PrescriptionItems",
                type: "TEXT",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Frequency",
                table: "PrescriptionItems",
                type: "TEXT",
                maxLength: 100,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Dosage",
                table: "PrescriptionItems");

            migrationBuilder.DropColumn(
                name: "Duration",
                table: "PrescriptionItems");

            migrationBuilder.DropColumn(
                name: "Frequency",
                table: "PrescriptionItems");
        }
    }
}
