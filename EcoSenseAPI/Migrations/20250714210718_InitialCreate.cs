using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EcoSenseAPI.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Usuarios",
                columns: table => new
                {
                    IdUsuario = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Nombre = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Correo = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Contraseña = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Telefono = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Rol = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Empresa = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Usuarios", x => x.IdUsuario);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "DatosSalud",
                columns: table => new
                {
                    IdUsuario = table.Column<int>(type: "int", nullable: false),
                    FechaNacimiento = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    Genero = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    AlturaCm = table.Column<ushort>(type: "smallint unsigned", nullable: false),
                    PesoKg = table.Column<ushort>(type: "smallint unsigned", nullable: false),
                    EnfermedadesRespiratorias = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Alergias = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Observaciones = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DatosSalud", x => x.IdUsuario);
                    table.ForeignKey(
                        name: "FK_DatosSalud_Usuarios_IdUsuario",
                        column: x => x.IdUsuario,
                        principalTable: "Usuarios",
                        principalColumn: "IdUsuario",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Dispositivos",
                columns: table => new
                {
                    IdDispositivo = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Mac = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Nombre = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Ubicacion = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IdUsuario = table.Column<int>(type: "int", nullable: false),
                    FechaRegistro = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    UsuarioIdUsuario = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Dispositivos", x => x.IdDispositivo);
                    table.ForeignKey(
                        name: "FK_Dispositivos_Usuarios_UsuarioIdUsuario",
                        column: x => x.UsuarioIdUsuario,
                        principalTable: "Usuarios",
                        principalColumn: "IdUsuario",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "ReportesExportados",
                columns: table => new
                {
                    IdReporte = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    IdUsuario = table.Column<int>(type: "int", nullable: false),
                    RangoInicio = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    RangoFin = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    TipoReporte = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    FechaExportacion = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    UsuarioIdUsuario = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReportesExportados", x => x.IdReporte);
                    table.ForeignKey(
                        name: "FK_ReportesExportados_Usuarios_UsuarioIdUsuario",
                        column: x => x.UsuarioIdUsuario,
                        principalTable: "Usuarios",
                        principalColumn: "IdUsuario",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "ConfiguracionUmbrales",
                columns: table => new
                {
                    IdUmbral = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    IdDispositivo = table.Column<int>(type: "int", nullable: false),
                    Co2Max = table.Column<int>(type: "int", nullable: false),
                    Pm1Max = table.Column<float>(type: "float", nullable: false),
                    Pm2_5Max = table.Column<float>(type: "float", nullable: false),
                    Pm10Max = table.Column<float>(type: "float", nullable: false),
                    TemperaturaMax = table.Column<float>(type: "float", nullable: false),
                    HumedadMax = table.Column<float>(type: "float", nullable: false),
                    PresionMax = table.Column<float>(type: "float", nullable: false),
                    FechaConfiguracion = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    DispositivoIdDispositivo = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ConfiguracionUmbrales", x => x.IdUmbral);
                    table.ForeignKey(
                        name: "FK_ConfiguracionUmbrales_Dispositivos_DispositivoIdDispositivo",
                        column: x => x.DispositivoIdDispositivo,
                        principalTable: "Dispositivos",
                        principalColumn: "IdDispositivo",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Lecturas",
                columns: table => new
                {
                    IdLectura = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    IdDispositivo = table.Column<int>(type: "int", nullable: false),
                    Timestamp = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    Co2 = table.Column<int>(type: "int", nullable: true),
                    Pm1_0 = table.Column<float>(type: "float", nullable: true),
                    Pm2_5 = table.Column<float>(type: "float", nullable: true),
                    Pm10 = table.Column<float>(type: "float", nullable: true),
                    Temperatura = table.Column<float>(type: "float", nullable: true),
                    Humedad = table.Column<float>(type: "float", nullable: true),
                    Presion = table.Column<float>(type: "float", nullable: true),
                    DispositivoIdDispositivo = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Lecturas", x => x.IdLectura);
                    table.ForeignKey(
                        name: "FK_Lecturas_Dispositivos_DispositivoIdDispositivo",
                        column: x => x.DispositivoIdDispositivo,
                        principalTable: "Dispositivos",
                        principalColumn: "IdDispositivo",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Alertas",
                columns: table => new
                {
                    IdAlerta = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    IdLectura = table.Column<long>(type: "bigint", nullable: false),
                    TipoAlerta = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ValorMedido = table.Column<float>(type: "float", nullable: false),
                    Umbral = table.Column<float>(type: "float", nullable: false),
                    Timestamp = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    LecturaIdLectura = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Alertas", x => x.IdAlerta);
                    table.ForeignKey(
                        name: "FK_Alertas_Lecturas_LecturaIdLectura",
                        column: x => x.LecturaIdLectura,
                        principalTable: "Lecturas",
                        principalColumn: "IdLectura",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_Alertas_LecturaIdLectura",
                table: "Alertas",
                column: "LecturaIdLectura");

            migrationBuilder.CreateIndex(
                name: "IX_ConfiguracionUmbrales_DispositivoIdDispositivo",
                table: "ConfiguracionUmbrales",
                column: "DispositivoIdDispositivo");

            migrationBuilder.CreateIndex(
                name: "IX_Dispositivos_UsuarioIdUsuario",
                table: "Dispositivos",
                column: "UsuarioIdUsuario");

            migrationBuilder.CreateIndex(
                name: "IX_Lecturas_DispositivoIdDispositivo",
                table: "Lecturas",
                column: "DispositivoIdDispositivo");

            migrationBuilder.CreateIndex(
                name: "IX_ReportesExportados_UsuarioIdUsuario",
                table: "ReportesExportados",
                column: "UsuarioIdUsuario");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Alertas");

            migrationBuilder.DropTable(
                name: "ConfiguracionUmbrales");

            migrationBuilder.DropTable(
                name: "DatosSalud");

            migrationBuilder.DropTable(
                name: "ReportesExportados");

            migrationBuilder.DropTable(
                name: "Lecturas");

            migrationBuilder.DropTable(
                name: "Dispositivos");

            migrationBuilder.DropTable(
                name: "Usuarios");
        }
    }
}
