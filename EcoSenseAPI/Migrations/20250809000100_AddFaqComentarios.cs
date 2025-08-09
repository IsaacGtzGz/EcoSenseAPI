using Microsoft.EntityFrameworkCore.Migrations;

namespace EcoSenseAPI.Migrations
{
    public partial class AddFaqComentarios : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "FaqComentarios",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Pregunta = table.Column<string>(nullable: false),
                    Respuesta = table.Column<string>(nullable: true),
                    Autor = table.Column<string>(nullable: true),
                    Fecha = table.Column<DateTime>(nullable: false),
                    Destacado = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FaqComentarios", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(name: "FaqComentarios");
        }
    }
}
