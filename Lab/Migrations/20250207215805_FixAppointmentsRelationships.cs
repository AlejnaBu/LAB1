using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Lab.Migrations
{
    /// <inheritdoc />
    public partial class FixAppointmentsRelationships : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Appointments_Doktoret_DoktoriId",
                table: "Appointments");

            migrationBuilder.DropForeignKey(
                name: "FK_Appointments_Pacientet_PacientiId",
                table: "Appointments");

            migrationBuilder.AddForeignKey(
                name: "FK_Appointments_Doktoret_DoktoriId",
                table: "Appointments",
                column: "DoktoriId",
                principalTable: "Doktoret",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Appointments_Pacientet_PacientiId",
                table: "Appointments",
                column: "PacientiId",
                principalTable: "Pacientet",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Appointments_Doktoret_DoktoriId",
                table: "Appointments");

            migrationBuilder.DropForeignKey(
                name: "FK_Appointments_Pacientet_PacientiId",
                table: "Appointments");

            migrationBuilder.AddForeignKey(
                name: "FK_Appointments_Doktoret_DoktoriId",
                table: "Appointments",
                column: "DoktoriId",
                principalTable: "Doktoret",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Appointments_Pacientet_PacientiId",
                table: "Appointments",
                column: "PacientiId",
                principalTable: "Pacientet",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
