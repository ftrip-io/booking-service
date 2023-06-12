using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ftrip.io.booking_service.Migrations
{
    public partial class addedhostidtoaccommodation : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "HostId",
                table: "Accommodations",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "HostId",
                table: "Accommodations");
        }
    }
}
