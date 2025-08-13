using Microsoft.EntityFrameworkCore.Migrations;

namespace EcoSenseAPI.Migrations
{
    public partial class AddUsuarioIdToFaqComentario : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "UsuarioId",
                table: "FaqComentarios",
                type: "int",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UsuarioId",
                table: "FaqComentarios");
        }
    }
}
