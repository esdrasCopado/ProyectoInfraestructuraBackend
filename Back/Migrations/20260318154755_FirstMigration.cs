using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SolicitudServidores.Migrations
{
    /// <inheritdoc />
    public partial class FirstMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "public");

            migrationBuilder.CreateTable(
                name: "SolicitudServidores",
                columns: table => new
                {
                    Id = table.Column<long>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ProjectName = table.Column<string>(type: "TEXT", nullable: true),
                    RequestedBy = table.Column<string>(type: "TEXT", nullable: true),
                    Description = table.Column<string>(type: "TEXT", nullable: true),
                    Architecture = table.Column<string>(type: "TEXT", nullable: true),
                    RequiredServices = table.Column<string>(type: "TEXT", nullable: true),
                    TargetDate = table.Column<string>(type: "TEXT", nullable: true),
                    Status = table.Column<string>(type: "TEXT", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SolicitudServidores", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "usuarios",
                schema: "public",
                columns: table => new
                {
                    id = table.Column<long>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    nombre_completo = table.Column<string>(type: "TEXT", maxLength: 120, nullable: false),
                    permisos = table.Column<string>(type: "text", nullable: false),
                    correo = table.Column<string>(type: "TEXT", maxLength: 80, nullable: false),
                    imagen = table.Column<string>(type: "text", nullable: true),
                    contrasena = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_usuarios", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "ServidoresEntrada",
                columns: table => new
                {
                    Id = table.Column<long>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ServerRequestId = table.Column<int>(type: "INTEGER", nullable: false),
                    Hostname = table.Column<string>(type: "TEXT", nullable: true),
                    Ip = table.Column<string>(type: "TEXT", nullable: true),
                    Role = table.Column<string>(type: "TEXT", nullable: true),
                    Os = table.Column<string>(type: "TEXT", nullable: true),
                    Cpu = table.Column<string>(type: "TEXT", nullable: true),
                    Ram = table.Column<string>(type: "TEXT", nullable: true),
                    Disk = table.Column<string>(type: "TEXT", nullable: true),
                    Purpose = table.Column<string>(type: "TEXT", nullable: true),
                    ServerRequestId1 = table.Column<long>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ServidoresEntrada", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ServidoresEntrada_SolicitudServidores_ServerRequestId1",
                        column: x => x.ServerRequestId1,
                        principalTable: "SolicitudServidores",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "PermisoCategoria",
                columns: table => new
                {
                    id = table.Column<long>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    categoria = table.Column<string>(type: "TEXT", nullable: false),
                    id_usuario = table.Column<long>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PermisoCategoria", x => x.id);
                    table.ForeignKey(
                        name: "FK_PermisoCategoria_usuarios_id_usuario",
                        column: x => x.id_usuario,
                        principalSchema: "public",
                        principalTable: "usuarios",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "solicitud",
                schema: "public",
                columns: table => new
                {
                    id = table.Column<long>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Id_Usuario = table.Column<long>(type: "INTEGER", nullable: true),
                    id_usuario = table.Column<long>(type: "INTEGER", nullable: true),
                    Estado = table.Column<string>(type: "TEXT", maxLength: 120, nullable: false),
                    Fecha_Creacion = table.Column<DateTime>(type: "date", nullable: true),
                    Arquitectura = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    Descripcion = table.Column<string>(type: "text", nullable: true),
                    Servicios = table.Column<string>(type: "TEXT", maxLength: 300, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_solicitud", x => x.id);
                    table.ForeignKey(
                        name: "FK_solicitud_usuarios_id_usuario",
                        column: x => x.id_usuario,
                        principalSchema: "public",
                        principalTable: "usuarios",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "servidor",
                schema: "public",
                columns: table => new
                {
                    id = table.Column<long>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Id_Solicitud = table.Column<long>(type: "INTEGER", nullable: false),
                    id_solicitud = table.Column<long>(type: "INTEGER", nullable: true),
                    Estado = table.Column<string>(type: "TEXT", maxLength: 120, nullable: false),
                    expiracion = table.Column<DateTime>(type: "date", nullable: true),
                    hostname = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    ip = table.Column<string>(type: "text", nullable: true),
                    funcion = table.Column<string>(type: "TEXT", maxLength: 300, nullable: false),
                    sistemaOperativo = table.Column<string>(type: "TEXT", maxLength: 300, nullable: false),
                    llaveOS = table.Column<string>(type: "TEXT", maxLength: 300, nullable: false),
                    nucleos = table.Column<int>(type: "INTEGER", maxLength: 300, nullable: false),
                    ram = table.Column<int>(type: "INTEGER", maxLength: 300, nullable: false),
                    almacenamiento = table.Column<int>(type: "INTEGER", maxLength: 300, nullable: false),
                    descripcion = table.Column<string>(type: "TEXT", maxLength: 300, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_servidor", x => x.id);
                    table.ForeignKey(
                        name: "FK_servidor_solicitud_id_solicitud",
                        column: x => x.id_solicitud,
                        principalSchema: "public",
                        principalTable: "solicitud",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "evidencias_pruebas",
                schema: "public",
                columns: table => new
                {
                    id = table.Column<long>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Id_Servidor = table.Column<long>(type: "INTEGER", nullable: false),
                    id_servidor = table.Column<long>(type: "INTEGER", nullable: true),
                    Id_usuario = table.Column<long>(type: "INTEGER", nullable: false),
                    id_usuario = table.Column<long>(type: "INTEGER", nullable: true),
                    ruta_pdf = table.Column<string>(type: "TEXT", nullable: false),
                    fecha = table.Column<DateTime>(type: "date", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_evidencias_pruebas", x => x.id);
                    table.ForeignKey(
                        name: "FK_evidencias_pruebas_servidor_id_servidor",
                        column: x => x.id_servidor,
                        principalSchema: "public",
                        principalTable: "servidor",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_evidencias_pruebas_usuarios_id_usuario",
                        column: x => x.id_usuario,
                        principalSchema: "public",
                        principalTable: "usuarios",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "subdominio",
                schema: "public",
                columns: table => new
                {
                    id = table.Column<long>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Id_Servidor = table.Column<long>(type: "INTEGER", nullable: true),
                    id_servidor = table.Column<long>(type: "INTEGER", nullable: true),
                    Id_usuario = table.Column<long>(type: "INTEGER", nullable: false),
                    id_usuario = table.Column<long>(type: "INTEGER", nullable: true),
                    nombre_url = table.Column<string>(type: "TEXT", nullable: false),
                    fecha_asignacion = table.Column<DateTime>(type: "date", nullable: true),
                    fecha_expiracion = table.Column<DateTime>(type: "date", nullable: true),
                    estado = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_subdominio", x => x.id);
                    table.ForeignKey(
                        name: "FK_subdominio_servidor_id_servidor",
                        column: x => x.id_servidor,
                        principalSchema: "public",
                        principalTable: "servidor",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_subdominio_usuarios_id_usuario",
                        column: x => x.id_usuario,
                        principalSchema: "public",
                        principalTable: "usuarios",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "vpn",
                schema: "public",
                columns: table => new
                {
                    id = table.Column<long>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Id_servidor = table.Column<long>(type: "INTEGER", nullable: false),
                    id_servidor = table.Column<long>(type: "INTEGER", nullable: true),
                    Id_usuario_Responsable = table.Column<long>(type: "INTEGER", nullable: false),
                    id_usuario = table.Column<long>(type: "INTEGER", nullable: true),
                    tipo = table.Column<string>(type: "TEXT", nullable: false),
                    fecha_asignacion = table.Column<DateTime>(type: "date", nullable: true),
                    fecha_expiracion = table.Column<DateTime>(type: "date", nullable: true),
                    estado = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_vpn", x => x.id);
                    table.ForeignKey(
                        name: "FK_vpn_servidor_id_servidor",
                        column: x => x.id_servidor,
                        principalSchema: "public",
                        principalTable: "servidor",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_vpn_usuarios_id_usuario",
                        column: x => x.id_usuario,
                        principalSchema: "public",
                        principalTable: "usuarios",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "waf",
                schema: "public",
                columns: table => new
                {
                    id = table.Column<long>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Id_Servidor = table.Column<long>(type: "INTEGER", nullable: false),
                    id_servidor = table.Column<long>(type: "INTEGER", nullable: true),
                    Id_usuario = table.Column<long>(type: "INTEGER", nullable: false),
                    id_usuario = table.Column<long>(type: "INTEGER", nullable: true),
                    fecha = table.Column<DateTime>(type: "date", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_waf", x => x.id);
                    table.ForeignKey(
                        name: "FK_waf_servidor_id_servidor",
                        column: x => x.id_servidor,
                        principalSchema: "public",
                        principalTable: "servidor",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_waf_usuarios_id_usuario",
                        column: x => x.id_usuario,
                        principalSchema: "public",
                        principalTable: "usuarios",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateIndex(
                name: "IX_evidencias_pruebas_id_servidor",
                schema: "public",
                table: "evidencias_pruebas",
                column: "id_servidor");

            migrationBuilder.CreateIndex(
                name: "IX_evidencias_pruebas_id_usuario",
                schema: "public",
                table: "evidencias_pruebas",
                column: "id_usuario");

            migrationBuilder.CreateIndex(
                name: "IX_PermisoCategoria_id_usuario",
                table: "PermisoCategoria",
                column: "id_usuario");

            migrationBuilder.CreateIndex(
                name: "IX_servidor_id_solicitud",
                schema: "public",
                table: "servidor",
                column: "id_solicitud");

            migrationBuilder.CreateIndex(
                name: "IX_ServidoresEntrada_ServerRequestId1",
                table: "ServidoresEntrada",
                column: "ServerRequestId1");

            migrationBuilder.CreateIndex(
                name: "IX_solicitud_id_usuario",
                schema: "public",
                table: "solicitud",
                column: "id_usuario");

            migrationBuilder.CreateIndex(
                name: "IX_subdominio_id_servidor",
                schema: "public",
                table: "subdominio",
                column: "id_servidor");

            migrationBuilder.CreateIndex(
                name: "IX_subdominio_id_usuario",
                schema: "public",
                table: "subdominio",
                column: "id_usuario");

            migrationBuilder.CreateIndex(
                name: "IX_vpn_id_servidor",
                schema: "public",
                table: "vpn",
                column: "id_servidor");

            migrationBuilder.CreateIndex(
                name: "IX_vpn_id_usuario",
                schema: "public",
                table: "vpn",
                column: "id_usuario");

            migrationBuilder.CreateIndex(
                name: "IX_waf_id_servidor",
                schema: "public",
                table: "waf",
                column: "id_servidor");

            migrationBuilder.CreateIndex(
                name: "IX_waf_id_usuario",
                schema: "public",
                table: "waf",
                column: "id_usuario");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "evidencias_pruebas",
                schema: "public");

            migrationBuilder.DropTable(
                name: "PermisoCategoria");

            migrationBuilder.DropTable(
                name: "ServidoresEntrada");

            migrationBuilder.DropTable(
                name: "subdominio",
                schema: "public");

            migrationBuilder.DropTable(
                name: "vpn",
                schema: "public");

            migrationBuilder.DropTable(
                name: "waf",
                schema: "public");

            migrationBuilder.DropTable(
                name: "SolicitudServidores");

            migrationBuilder.DropTable(
                name: "servidor",
                schema: "public");

            migrationBuilder.DropTable(
                name: "solicitud",
                schema: "public");

            migrationBuilder.DropTable(
                name: "usuarios",
                schema: "public");
        }
    }
}
