using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Services.Migrations
{
    /// <inheritdoc />
    public partial class IntialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ApiKeys",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Key = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Expiration = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedByUserId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ApiKeys", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ApiKeys_Users_CreatedByUserId",
                        column: x => x.CreatedByUserId,
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Appointments",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TimeSlot = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ReservedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ConfirmationDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ProviderUserId = table.Column<int>(type: "int", nullable: false),
                    ClientUserId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Appointments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Appointments_Users_ClientUserId",
                        column: x => x.ClientUserId,
                        principalTable: "Users",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Appointments_Users_ProviderUserId",
                        column: x => x.ProviderUserId,
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "UserRoles",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    Role = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserRoles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserRoles_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { 1, "Brandon" },
                    { 2, "Lora" },
                    { 3, "Zach" },
                    { 4, "Colette" },
                    { 5, "Noah" },
                    { 6, "Sharla" }
                });

            migrationBuilder.InsertData(
                table: "ApiKeys",
                columns: new[] { "Id", "CreatedByUserId", "Expiration", "Key" },
                values: new object[,]
                {
                    { 1, 1, new DateTime(2026, 7, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), "efe33b8d-9311-4858-8cb5-ab38dd97604b" },
                    { 2, 2, new DateTime(2026, 7, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), "2011ea9a-ae39-8be4-7656-f46f48d289ff" },
                    { 3, 3, new DateTime(2023, 7, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), "2dc26fe3-df8c-7ff7-7f12-c72f3973f651" },
                    { 4, 4, new DateTime(2026, 7, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), "e28e8a47-3938-1836-367a-1c4fd9f9997d" },
                    { 5, 5, new DateTime(2026, 7, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), "f5fe1945-97e2-f352-e7ff-43ff0dc78580" },
                    { 6, 6, new DateTime(2026, 7, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), "8948c70e-6805-e5d1-4b1c-c387cd8e3c7f" }
                });

            migrationBuilder.InsertData(
                table: "UserRoles",
                columns: new[] { "Id", "Role", "UserId" },
                values: new object[,]
                {
                    { 1, 3, 1 },
                    { 2, 2, 2 },
                    { 3, 1, 3 },
                    { 4, 1, 4 },
                    { 5, 1, 5 },
                    { 6, 2, 6 },
                    { 7, 1, 6 }
                });

            migrationBuilder.CreateIndex(
                name: "IX_ApiKeys_CreatedByUserId",
                table: "ApiKeys",
                column: "CreatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Appointments_ClientUserId",
                table: "Appointments",
                column: "ClientUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Appointments_ProviderUserId",
                table: "Appointments",
                column: "ProviderUserId");

            migrationBuilder.CreateIndex(
                name: "IX_UserRoles_UserId",
                table: "UserRoles",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ApiKeys");

            migrationBuilder.DropTable(
                name: "Appointments");

            migrationBuilder.DropTable(
                name: "UserRoles");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
