using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TrackingAnimal.Migrations
{
    /// <inheritdoc />
    public partial class addedAnimalsTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Animals",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    weight = table.Column<float>(type: "real", nullable: false),
                    length = table.Column<float>(type: "real", nullable: false),
                    height = table.Column<float>(type: "real", nullable: false),
                    gender = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    lifeStatus = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    chipperId = table.Column<int>(type: "int", nullable: false),
                    chippingLocationId = table.Column<long>(type: "bigint", nullable: false),
                    chippingDateTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    deathDateTime = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Animals", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AnimalAnimalType",
                columns: table => new
                {
                    AnimalId = table.Column<long>(type: "bigint", nullable: false),
                    animalTypesId = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AnimalAnimalType", x => new { x.AnimalId, x.animalTypesId });
                    table.ForeignKey(
                        name: "FK_AnimalAnimalType_AnimalTypes_animalTypesId",
                        column: x => x.animalTypesId,
                        principalTable: "AnimalTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AnimalAnimalType_Animals_AnimalId",
                        column: x => x.AnimalId,
                        principalTable: "Animals",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AnimalAnimalType_animalTypesId",
                table: "AnimalAnimalType",
                column: "animalTypesId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AnimalAnimalType");

            migrationBuilder.DropTable(
                name: "Animals");
        }
    }
}
