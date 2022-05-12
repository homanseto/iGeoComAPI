using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace iGeoComAPI.Migrations
{
    public partial class CreateInitial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "IGeoComGrabModels",
                columns: table => new
                {
                    GrabId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Latitude = table.Column<double>(type: "float", nullable: false),
                    Longitude = table.Column<double>(type: "float", nullable: false),
                    GeoNameId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    EnglishName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ChineseName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Class = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Type = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Subcat = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Easting = table.Column<double>(type: "float", nullable: false),
                    Northing = table.Column<double>(type: "float", nullable: false),
                    Source = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    E_floor = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    C_floor = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    E_sitename = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    C_sitename = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    E_area = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    C_area = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    E_District = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    C_District = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    E_Region = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    C_Region = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    E_Address = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    C_Address = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Tel_No = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Fax_No = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Web_Site = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Rev_Date = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IGeoComGrabModels", x => x.GrabId);
                });

            migrationBuilder.CreateTable(
                name: "IGeoComModels",
                columns: table => new
                {
                    GeoNameId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    EnglishName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ChineseName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Class = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Type = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Subcat = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Easting = table.Column<double>(type: "float", nullable: false),
                    Northing = table.Column<double>(type: "float", nullable: false),
                    Source = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    E_floor = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    C_floor = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    E_sitename = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    C_sitename = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    E_area = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    C_area = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    E_District = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    C_District = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    E_Region = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    C_Region = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    E_Address = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    C_Address = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Tel_No = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Fax_No = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Web_Site = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Rev_Date = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IGeoComModels", x => x.GeoNameId);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "IGeoComGrabModels");

            migrationBuilder.DropTable(
                name: "IGeoComModels");
        }
    }
}
