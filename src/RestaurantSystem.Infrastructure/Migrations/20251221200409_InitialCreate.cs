using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RestaurantSystem.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "MenuDia",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Fecha = table.Column<DateTime>(type: "date", nullable: false),
                    Activo = table.Column<bool>(type: "bit", nullable: false),
                    CreadoEn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MenuDia", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Mesa",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Nombre = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    Estado = table.Column<int>(type: "int", nullable: false),
                    NroPersonas = table.Column<int>(type: "int", nullable: true),
                    CreadoEn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Mesa", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Producto",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Nombre = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Precio = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    CostoEstandar = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    Tipo = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    Activo = table.Column<bool>(type: "bit", nullable: false),
                    CreadoEn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Producto", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Usuario",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Nombre = table.Column<string>(type: "nvarchar(60)", maxLength: 60, nullable: false),
                    Apellido = table.Column<string>(type: "nvarchar(60)", maxLength: 60, nullable: false),
                    NombreCompleto = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Username = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    Rol = table.Column<int>(type: "int", nullable: false),
                    Activo = table.Column<bool>(type: "bit", nullable: false),
                    CreadoEn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Usuario", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MenuDiaItem",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    MenuDiaId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ProductoId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreadoEn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MenuDiaItem", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MenuDiaItem_MenuDia_MenuDiaId",
                        column: x => x.MenuDiaId,
                        principalTable: "MenuDia",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MenuDiaItem_Producto_ProductoId",
                        column: x => x.ProductoId,
                        principalTable: "Producto",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "AuditoriaEvento",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Fecha = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UsuarioId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Accion = table.Column<string>(type: "nvarchar(80)", maxLength: 80, nullable: false),
                    Entidad = table.Column<string>(type: "nvarchar(80)", maxLength: 80, nullable: false),
                    EntidadId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Motivo = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    CreadoEn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AuditoriaEvento", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AuditoriaEvento_Usuario_UsuarioId",
                        column: x => x.UsuarioId,
                        principalTable: "Usuario",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CajaSesion",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UsuarioId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Estado = table.Column<int>(type: "int", nullable: false),
                    AperturaEn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    MontoApertura = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    CierreEn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    MontoCierreContado = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: true),
                    EfectivoEsperado = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: true),
                    Diferencia = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: true),
                    CreadoEn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CajaSesion", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CajaSesion_Usuario_UsuarioId",
                        column: x => x.UsuarioId,
                        principalTable: "Usuario",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Cuenta",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Tipo = table.Column<int>(type: "int", nullable: false),
                    Estado = table.Column<int>(type: "int", nullable: false),
                    MesaId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreadaPorUsuarioId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AperturaEn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CierreEn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ObservacionGeneral = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: true),
                    CreadoEn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Cuenta", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Cuenta_Mesa_MesaId",
                        column: x => x.MesaId,
                        principalTable: "Mesa",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Cuenta_Usuario_CreadaPorUsuarioId",
                        column: x => x.CreadaPorUsuarioId,
                        principalTable: "Usuario",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "MovimientoCaja",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CajaSesionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UsuarioId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Tipo = table.Column<int>(type: "int", nullable: false),
                    Monto = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    Motivo = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    Fecha = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreadoEn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MovimientoCaja", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MovimientoCaja_CajaSesion_CajaSesionId",
                        column: x => x.CajaSesionId,
                        principalTable: "CajaSesion",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_MovimientoCaja_Usuario_UsuarioId",
                        column: x => x.UsuarioId,
                        principalTable: "Usuario",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Comanda",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CuentaId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Estado = table.Column<int>(type: "int", nullable: false),
                    CreadaEn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreadaPorUsuarioId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    NumeroSecuencia = table.Column<int>(type: "int", nullable: false),
                    CreadoEn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Comanda", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Comanda_Cuenta_CuentaId",
                        column: x => x.CuentaId,
                        principalTable: "Cuenta",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Comanda_Usuario_CreadaPorUsuarioId",
                        column: x => x.CreadaPorUsuarioId,
                        principalTable: "Usuario",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Pago",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CuentaId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CajaSesionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PagadoEn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Total = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    Anulado = table.Column<bool>(type: "bit", nullable: false),
                    MotivoAnulacion = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    CreadoEn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Pago", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Pago_CajaSesion_CajaSesionId",
                        column: x => x.CajaSesionId,
                        principalTable: "CajaSesion",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Pago_Cuenta_CuentaId",
                        column: x => x.CuentaId,
                        principalTable: "Cuenta",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ComandaDetalle",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ComandaId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ProductoId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Cantidad = table.Column<int>(type: "int", nullable: false),
                    PrecioUnitario = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    CostoUnitarioEstandar = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    Observacion = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    EstadoCocina = table.Column<int>(type: "int", nullable: false),
                    CantidadPagada = table.Column<int>(type: "int", nullable: false),
                    Anulado = table.Column<bool>(type: "bit", nullable: false),
                    CreadoEn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ComandaDetalle", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ComandaDetalle_Comanda_ComandaId",
                        column: x => x.ComandaId,
                        principalTable: "Comanda",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ComandaDetalle_Producto_ProductoId",
                        column: x => x.ProductoId,
                        principalTable: "Producto",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PagoMetodo",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PagoId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Metodo = table.Column<int>(type: "int", nullable: false),
                    Monto = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    ReferenciaOperacion = table.Column<string>(type: "nvarchar(60)", maxLength: 60, nullable: true),
                    CreadoEn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PagoMetodo", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PagoMetodo_Pago_PagoId",
                        column: x => x.PagoId,
                        principalTable: "Pago",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PagoDetalle",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PagoId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ComandaDetalleId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CantidadPagada = table.Column<int>(type: "int", nullable: false),
                    MontoAsignado = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    CreadoEn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PagoDetalle", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PagoDetalle_ComandaDetalle_ComandaDetalleId",
                        column: x => x.ComandaDetalleId,
                        principalTable: "ComandaDetalle",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PagoDetalle_Pago_PagoId",
                        column: x => x.PagoId,
                        principalTable: "Pago",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AuditoriaEvento_Entidad_EntidadId_Fecha",
                table: "AuditoriaEvento",
                columns: new[] { "Entidad", "EntidadId", "Fecha" });

            migrationBuilder.CreateIndex(
                name: "IX_AuditoriaEvento_UsuarioId",
                table: "AuditoriaEvento",
                column: "UsuarioId");

            migrationBuilder.CreateIndex(
                name: "IX_CajaSesion_Estado",
                table: "CajaSesion",
                column: "Estado");

            migrationBuilder.CreateIndex(
                name: "IX_CajaSesion_UsuarioId_AperturaEn",
                table: "CajaSesion",
                columns: new[] { "UsuarioId", "AperturaEn" });

            migrationBuilder.CreateIndex(
                name: "IX_Comanda_CreadaPorUsuarioId",
                table: "Comanda",
                column: "CreadaPorUsuarioId");

            migrationBuilder.CreateIndex(
                name: "IX_Comanda_CuentaId_CreadaEn",
                table: "Comanda",
                columns: new[] { "CuentaId", "CreadaEn" });

            migrationBuilder.CreateIndex(
                name: "IX_Comanda_CuentaId_NumeroSecuencia",
                table: "Comanda",
                columns: new[] { "CuentaId", "NumeroSecuencia" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ComandaDetalle_ComandaId",
                table: "ComandaDetalle",
                column: "ComandaId");

            migrationBuilder.CreateIndex(
                name: "IX_ComandaDetalle_ComandaId_Anulado",
                table: "ComandaDetalle",
                columns: new[] { "ComandaId", "Anulado" });

            migrationBuilder.CreateIndex(
                name: "IX_ComandaDetalle_ComandaId_EstadoCocina",
                table: "ComandaDetalle",
                columns: new[] { "ComandaId", "EstadoCocina" });

            migrationBuilder.CreateIndex(
                name: "IX_ComandaDetalle_ProductoId",
                table: "ComandaDetalle",
                column: "ProductoId");

            migrationBuilder.CreateIndex(
                name: "IX_Cuenta_AperturaEn",
                table: "Cuenta",
                column: "AperturaEn");

            migrationBuilder.CreateIndex(
                name: "IX_Cuenta_CreadaPorUsuarioId",
                table: "Cuenta",
                column: "CreadaPorUsuarioId");

            migrationBuilder.CreateIndex(
                name: "IX_Cuenta_MesaId_Estado",
                table: "Cuenta",
                columns: new[] { "MesaId", "Estado" });

            migrationBuilder.CreateIndex(
                name: "IX_MenuDia_Fecha",
                table: "MenuDia",
                column: "Fecha",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_MenuDiaItem_MenuDiaId_ProductoId",
                table: "MenuDiaItem",
                columns: new[] { "MenuDiaId", "ProductoId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_MenuDiaItem_ProductoId",
                table: "MenuDiaItem",
                column: "ProductoId");

            migrationBuilder.CreateIndex(
                name: "IX_Mesa_Estado",
                table: "Mesa",
                column: "Estado");

            migrationBuilder.CreateIndex(
                name: "IX_Mesa_Nombre",
                table: "Mesa",
                column: "Nombre",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_MovimientoCaja_CajaSesionId_Fecha",
                table: "MovimientoCaja",
                columns: new[] { "CajaSesionId", "Fecha" });

            migrationBuilder.CreateIndex(
                name: "IX_MovimientoCaja_UsuarioId",
                table: "MovimientoCaja",
                column: "UsuarioId");

            migrationBuilder.CreateIndex(
                name: "IX_Pago_CajaSesionId_PagadoEn",
                table: "Pago",
                columns: new[] { "CajaSesionId", "PagadoEn" });

            migrationBuilder.CreateIndex(
                name: "IX_Pago_CuentaId",
                table: "Pago",
                column: "CuentaId");

            migrationBuilder.CreateIndex(
                name: "IX_PagoDetalle_ComandaDetalleId",
                table: "PagoDetalle",
                column: "ComandaDetalleId");

            migrationBuilder.CreateIndex(
                name: "IX_PagoDetalle_PagoId",
                table: "PagoDetalle",
                column: "PagoId");

            migrationBuilder.CreateIndex(
                name: "IX_PagoMetodo_PagoId",
                table: "PagoMetodo",
                column: "PagoId");

            migrationBuilder.CreateIndex(
                name: "IX_PagoMetodo_PagoId_Metodo",
                table: "PagoMetodo",
                columns: new[] { "PagoId", "Metodo" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Producto_Nombre",
                table: "Producto",
                column: "Nombre");

            migrationBuilder.CreateIndex(
                name: "IX_Producto_Tipo_Activo",
                table: "Producto",
                columns: new[] { "Tipo", "Activo" });

            migrationBuilder.CreateIndex(
                name: "IX_Usuario_Username",
                table: "Usuario",
                column: "Username",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AuditoriaEvento");

            migrationBuilder.DropTable(
                name: "MenuDiaItem");

            migrationBuilder.DropTable(
                name: "MovimientoCaja");

            migrationBuilder.DropTable(
                name: "PagoDetalle");

            migrationBuilder.DropTable(
                name: "PagoMetodo");

            migrationBuilder.DropTable(
                name: "MenuDia");

            migrationBuilder.DropTable(
                name: "ComandaDetalle");

            migrationBuilder.DropTable(
                name: "Pago");

            migrationBuilder.DropTable(
                name: "Comanda");

            migrationBuilder.DropTable(
                name: "Producto");

            migrationBuilder.DropTable(
                name: "CajaSesion");

            migrationBuilder.DropTable(
                name: "Cuenta");

            migrationBuilder.DropTable(
                name: "Mesa");

            migrationBuilder.DropTable(
                name: "Usuario");
        }
    }
}
