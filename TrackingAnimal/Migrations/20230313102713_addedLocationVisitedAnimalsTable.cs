using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TrackingAnimal.Migrations
{
    /// <inheritdoc />
    public partial class addedLocationVisitedAnimalsTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "locationVisitedAnimals",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    dateTimeOfVisitLocationPoint = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LocationPointId = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_locationVisitedAnimals", x => x.Id);
                    table.ForeignKey(
                        name: "FK_locationVisitedAnimals_Locations_LocationPointId",
                        column: x => x.LocationPointId,
                        principalTable: "Locations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_locationVisitedAnimals_LocationPointId",
                table: "locationVisitedAnimals",
                column: "LocationPointId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "locationVisitedAnimals");
        }
    }
}
