using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EcoSenseAPI.Migrations
{
    /// <inheritdoc />
    public partial class AddEstrellasToFaqComentario : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Estrellas",
                table: "FaqComentarios",
                type: "int",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Estrellas",
                table: "FaqComentarios");
        }
    }
}
