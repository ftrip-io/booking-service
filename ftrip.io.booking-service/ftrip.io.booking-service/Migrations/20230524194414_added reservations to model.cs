using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ftrip.io.booking_service.Migrations
{
    public partial class addedreservationstomodel : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Reservations",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Active = table.Column<bool>(nullable: false),
                    CreatedAt = table.Column<DateTime>(nullable: false),
                    ModifiedAt = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<string>(nullable: true),
                    ModifiedBy = table.Column<string>(nullable: true),
                    GuestId = table.Column<Guid>(nullable: false),
                    AccomodationId = table.Column<Guid>(nullable: false),
                    DatePeriod_DateFrom = table.Column<DateTime>(nullable: true),
                    DatePeriod_DateTo = table.Column<DateTime>(nullable: true),
                    GuestNumber = table.Column<int>(nullable: false),
                    IsDeclined = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Reservations", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Reservations");
        }
    }
}
