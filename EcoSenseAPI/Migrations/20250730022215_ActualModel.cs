using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EcoSenseAPI.Migrations
{
    /// <inheritdoc />
    public partial class ActualModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Alertas_Lecturas_LecturaIdLectura",
                table: "Alertas");

            migrationBuilder.DropForeignKey(
                name: "FK_ConfiguracionUmbrales_Dispositivos_DispositivoIdDispositivo",
                table: "ConfiguracionUmbrales");

            migrationBuilder.DropForeignKey(
                name: "FK_Dispositivos_Usuarios_UsuarioIdUsuario",
                table: "Dispositivos");

            migrationBuilder.DropForeignKey(
                name: "FK_Lecturas_Dispositivos_DispositivoIdDispositivo",
                table: "Lecturas");

            migrationBuilder.DropForeignKey(
                name: "FK_ReportesExportados_Usuarios_UsuarioIdUsuario",
                table: "ReportesExportados");

            migrationBuilder.UpdateData(
                table: "Usuarios",
                keyColumn: "Empresa",
                keyValue: null,
                column: "Empresa",
                value: "");

            migrationBuilder.AlterColumn<string>(
                name: "Empresa",
                table: "Usuarios",
                type: "longtext",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "longtext",
                oldNullable: true)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<int>(
                name: "UsuarioIdUsuario",
                table: "ReportesExportados",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<string>(
                name: "RutaArchivo",
                table: "ReportesExportados",
                type: "longtext",
                nullable: false)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<int>(
                name: "DispositivoIdDispositivo",
                table: "Lecturas",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "UsuarioIdUsuario",
                table: "Dispositivos",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<string>(
                name: "Sector",
                table: "Dispositivos",
                type: "longtext",
                nullable: false)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<int>(
                name: "DispositivoIdDispositivo",
                table: "ConfiguracionUmbrales",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<long>(
                name: "LecturaIdLectura",
                table: "Alertas",
                type: "bigint",
                nullable: true,
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.AddForeignKey(
                name: "FK_Alertas_Lecturas_LecturaIdLectura",
                table: "Alertas",
                column: "LecturaIdLectura",
                principalTable: "Lecturas",
                principalColumn: "IdLectura");

            migrationBuilder.AddForeignKey(
                name: "FK_ConfiguracionUmbrales_Dispositivos_DispositivoIdDispositivo",
                table: "ConfiguracionUmbrales",
                column: "DispositivoIdDispositivo",
                principalTable: "Dispositivos",
                principalColumn: "IdDispositivo");

            migrationBuilder.AddForeignKey(
                name: "FK_Dispositivos_Usuarios_UsuarioIdUsuario",
                table: "Dispositivos",
                column: "UsuarioIdUsuario",
                principalTable: "Usuarios",
                principalColumn: "IdUsuario");

            migrationBuilder.AddForeignKey(
                name: "FK_Lecturas_Dispositivos_DispositivoIdDispositivo",
                table: "Lecturas",
                column: "DispositivoIdDispositivo",
                principalTable: "Dispositivos",
                principalColumn: "IdDispositivo");

            migrationBuilder.AddForeignKey(
                name: "FK_ReportesExportados_Usuarios_UsuarioIdUsuario",
                table: "ReportesExportados",
                column: "UsuarioIdUsuario",
                principalTable: "Usuarios",
                principalColumn: "IdUsuario");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Alertas_Lecturas_LecturaIdLectura",
                table: "Alertas");

            migrationBuilder.DropForeignKey(
                name: "FK_ConfiguracionUmbrales_Dispositivos_DispositivoIdDispositivo",
                table: "ConfiguracionUmbrales");

            migrationBuilder.DropForeignKey(
                name: "FK_Dispositivos_Usuarios_UsuarioIdUsuario",
                table: "Dispositivos");

            migrationBuilder.DropForeignKey(
                name: "FK_Lecturas_Dispositivos_DispositivoIdDispositivo",
                table: "Lecturas");

            migrationBuilder.DropForeignKey(
                name: "FK_ReportesExportados_Usuarios_UsuarioIdUsuario",
                table: "ReportesExportados");

            migrationBuilder.DropColumn(
                name: "RutaArchivo",
                table: "ReportesExportados");

            migrationBuilder.DropColumn(
                name: "Sector",
                table: "Dispositivos");

            migrationBuilder.AlterColumn<string>(
                name: "Empresa",
                table: "Usuarios",
                type: "longtext",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "longtext")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<int>(
                name: "UsuarioIdUsuario",
                table: "ReportesExportados",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "DispositivoIdDispositivo",
                table: "Lecturas",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "UsuarioIdUsuario",
                table: "Dispositivos",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "DispositivoIdDispositivo",
                table: "ConfiguracionUmbrales",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<long>(
                name: "LecturaIdLectura",
                table: "Alertas",
                type: "bigint",
                nullable: false,
                defaultValue: 0L,
                oldClrType: typeof(long),
                oldType: "bigint",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Alertas_Lecturas_LecturaIdLectura",
                table: "Alertas",
                column: "LecturaIdLectura",
                principalTable: "Lecturas",
                principalColumn: "IdLectura",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ConfiguracionUmbrales_Dispositivos_DispositivoIdDispositivo",
                table: "ConfiguracionUmbrales",
                column: "DispositivoIdDispositivo",
                principalTable: "Dispositivos",
                principalColumn: "IdDispositivo",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Dispositivos_Usuarios_UsuarioIdUsuario",
                table: "Dispositivos",
                column: "UsuarioIdUsuario",
                principalTable: "Usuarios",
                principalColumn: "IdUsuario",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Lecturas_Dispositivos_DispositivoIdDispositivo",
                table: "Lecturas",
                column: "DispositivoIdDispositivo",
                principalTable: "Dispositivos",
                principalColumn: "IdDispositivo",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ReportesExportados_Usuarios_UsuarioIdUsuario",
                table: "ReportesExportados",
                column: "UsuarioIdUsuario",
                principalTable: "Usuarios",
                principalColumn: "IdUsuario",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
