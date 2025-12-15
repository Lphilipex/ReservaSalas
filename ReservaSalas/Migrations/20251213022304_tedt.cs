using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ReservaSalas.Migrations
{
    /// <inheritdoc />
    public partial class tedt : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Reservas_Salas_SalaId",
                table: "Reservas");

            migrationBuilder.DropIndex(
                name: "IX_Reservas_SalaId",
                table: "Reservas");

            migrationBuilder.AddColumn<string>(
                name: "Sala",
                table: "Reservas",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Sala",
                table: "Reservas");

            migrationBuilder.CreateIndex(
                name: "IX_Reservas_SalaId",
                table: "Reservas",
                column: "SalaId");

            migrationBuilder.AddForeignKey(
                name: "FK_Reservas_Salas_SalaId",
                table: "Reservas",
                column: "SalaId",
                principalTable: "Salas",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
