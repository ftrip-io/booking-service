using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ftrip.io.booking_service.Migrations
{
    public partial class addedreviewsandsummaries : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AccomodationReviews",
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
                    AccomodationGrade = table.Column<int>(nullable: true),
                    LocationGrade = table.Column<int>(nullable: true),
                    ValueForMoneyGrade = table.Column<int>(nullable: true),
                    RecensionText = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AccomodationReviews", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AccomodationReviewsSummaries",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Active = table.Column<bool>(nullable: false),
                    CreatedAt = table.Column<DateTime>(nullable: false),
                    ModifiedAt = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<string>(nullable: true),
                    ModifiedBy = table.Column<string>(nullable: true),
                    AccomodationId = table.Column<Guid>(nullable: false),
                    ReviewsCount = table.Column<int>(nullable: false),
                    AccomodationGrade = table.Column<decimal>(nullable: true),
                    LocationGrade = table.Column<decimal>(nullable: true),
                    ValueForMoneyGrade = table.Column<decimal>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AccomodationReviewsSummaries", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "HostReviews",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Active = table.Column<bool>(nullable: false),
                    CreatedAt = table.Column<DateTime>(nullable: false),
                    ModifiedAt = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<string>(nullable: true),
                    ModifiedBy = table.Column<string>(nullable: true),
                    GuestId = table.Column<Guid>(nullable: false),
                    HostId = table.Column<Guid>(nullable: false),
                    CommunicationGrade = table.Column<int>(nullable: true),
                    OverallGrade = table.Column<int>(nullable: true),
                    RecensionText = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HostReviews", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "HostReviewsSummaries",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Active = table.Column<bool>(nullable: false),
                    CreatedAt = table.Column<DateTime>(nullable: false),
                    ModifiedAt = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<string>(nullable: true),
                    ModifiedBy = table.Column<string>(nullable: true),
                    HostId = table.Column<Guid>(nullable: false),
                    ReviewsCount = table.Column<int>(nullable: false),
                    CommunicationGrade = table.Column<decimal>(nullable: true),
                    OverallGrade = table.Column<decimal>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HostReviewsSummaries", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AccomodationReviews_AccomodationId",
                table: "AccomodationReviews",
                column: "AccomodationId");

            migrationBuilder.CreateIndex(
                name: "IX_AccomodationReviewsSummaries_AccomodationId",
                table: "AccomodationReviewsSummaries",
                column: "AccomodationId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_HostReviews_HostId",
                table: "HostReviews",
                column: "HostId");

            migrationBuilder.CreateIndex(
                name: "IX_HostReviewsSummaries_HostId",
                table: "HostReviewsSummaries",
                column: "HostId",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AccomodationReviews");

            migrationBuilder.DropTable(
                name: "AccomodationReviewsSummaries");

            migrationBuilder.DropTable(
                name: "HostReviews");

            migrationBuilder.DropTable(
                name: "HostReviewsSummaries");
        }
    }
}
