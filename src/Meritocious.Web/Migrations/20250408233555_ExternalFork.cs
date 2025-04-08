using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Meritocious.Web.Migrations
{
    /// <inheritdoc />
    public partial class ExternalFork : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ExternalForkContext",
                table: "Posts",
                type: "nvarchar(1000)",
                maxLength: 1000,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ExternalForkQuote",
                table: "Posts",
                type: "nvarchar(2000)",
                maxLength: 2000,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ExternalForkSourceId",
                table: "Posts",
                type: "varchar(26)",
                unicode: false,
                maxLength: 26,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ForkTypeId",
                table: "Posts",
                type: "varchar(26)",
                unicode: false,
                maxLength: 26,
                nullable: true);

            migrationBuilder.CreateTable(
                name: "ExternalForkSources",
                columns: table => new
                {
                    UlidId = table.Column<string>(type: "varchar(26)", unicode: false, maxLength: 26, nullable: false),
                    Type = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Platform = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Title = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    SourceUrl = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: false),
                    AuthorName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Timestamp = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Subtype = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    LocationMetadata = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Tags = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AdditionalMetadata = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExternalForkSources", x => x.UlidId);
                });

            migrationBuilder.CreateTable(
                name: "ForkTypes",
                columns: table => new
                {
                    UlidId = table.Column<string>(type: "varchar(26)", unicode: false, maxLength: 26, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    DisplayName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    Subtypes = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ForkTypes", x => x.UlidId);
                });

            migrationBuilder.CreateTable(
                name: "ForkRequests",
                columns: table => new
                {
                    UlidId = table.Column<string>(type: "varchar(26)", unicode: false, maxLength: 26, nullable: false),
                    SubmitterId = table.Column<string>(type: "varchar(26)", unicode: false, maxLength: 26, nullable: false),
                    ExternalForkSourceId = table.Column<string>(type: "varchar(26)", unicode: false, maxLength: 26, nullable: false),
                    SuggestedFocus = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    ClaimedById = table.Column<string>(type: "varchar(26)", unicode: false, maxLength: 26, nullable: true),
                    ClaimedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    FulfilledByPostId = table.Column<string>(type: "varchar(26)", unicode: false, maxLength: 26, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ForkRequests", x => x.UlidId);
                    table.ForeignKey(
                        name: "FK_ForkRequests_AspNetUsers_ClaimedById",
                        column: x => x.ClaimedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_ForkRequests_AspNetUsers_SubmitterId",
                        column: x => x.SubmitterId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ForkRequests_ExternalForkSources_ExternalForkSourceId",
                        column: x => x.ExternalForkSourceId,
                        principalTable: "ExternalForkSources",
                        principalColumn: "UlidId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ForkRequests_Posts_FulfilledByPostId",
                        column: x => x.FulfilledByPostId,
                        principalTable: "Posts",
                        principalColumn: "UlidId",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Posts_ExternalForkSourceId_CreatedAt",
                table: "Posts",
                columns: new[] { "ExternalForkSourceId", "CreatedAt" });

            migrationBuilder.CreateIndex(
                name: "IX_Posts_ForkTypeId_CreatedAt",
                table: "Posts",
                columns: new[] { "ForkTypeId", "CreatedAt" });

            migrationBuilder.CreateIndex(
                name: "IX_ExternalForkSources_Platform",
                table: "ExternalForkSources",
                column: "Platform");

            migrationBuilder.CreateIndex(
                name: "IX_ExternalForkSources_SourceUrl",
                table: "ExternalForkSources",
                column: "SourceUrl",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ExternalForkSources_Timestamp",
                table: "ExternalForkSources",
                column: "Timestamp");

            migrationBuilder.CreateIndex(
                name: "IX_ExternalForkSources_Type",
                table: "ExternalForkSources",
                column: "Type");

            migrationBuilder.CreateIndex(
                name: "IX_ForkRequests_ClaimedAt",
                table: "ForkRequests",
                column: "ClaimedAt");

            migrationBuilder.CreateIndex(
                name: "IX_ForkRequests_ClaimedById",
                table: "ForkRequests",
                column: "ClaimedById");

            migrationBuilder.CreateIndex(
                name: "IX_ForkRequests_ExternalForkSourceId",
                table: "ForkRequests",
                column: "ExternalForkSourceId");

            migrationBuilder.CreateIndex(
                name: "IX_ForkRequests_FulfilledByPostId",
                table: "ForkRequests",
                column: "FulfilledByPostId");

            migrationBuilder.CreateIndex(
                name: "IX_ForkRequests_Status",
                table: "ForkRequests",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_ForkRequests_Status_ClaimedById",
                table: "ForkRequests",
                columns: new[] { "Status", "ClaimedById" });

            migrationBuilder.CreateIndex(
                name: "IX_ForkRequests_Status_ExternalForkSourceId",
                table: "ForkRequests",
                columns: new[] { "Status", "ExternalForkSourceId" });

            migrationBuilder.CreateIndex(
                name: "IX_ForkRequests_SubmitterId",
                table: "ForkRequests",
                column: "SubmitterId");

            migrationBuilder.CreateIndex(
                name: "IX_ForkTypes_IsActive",
                table: "ForkTypes",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_ForkTypes_Name",
                table: "ForkTypes",
                column: "Name",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Posts_ExternalForkSources_ExternalForkSourceId",
                table: "Posts",
                column: "ExternalForkSourceId",
                principalTable: "ExternalForkSources",
                principalColumn: "UlidId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Posts_ForkTypes_ForkTypeId",
                table: "Posts",
                column: "ForkTypeId",
                principalTable: "ForkTypes",
                principalColumn: "UlidId",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Posts_ExternalForkSources_ExternalForkSourceId",
                table: "Posts");

            migrationBuilder.DropForeignKey(
                name: "FK_Posts_ForkTypes_ForkTypeId",
                table: "Posts");

            migrationBuilder.DropTable(
                name: "ForkRequests");

            migrationBuilder.DropTable(
                name: "ForkTypes");

            migrationBuilder.DropTable(
                name: "ExternalForkSources");

            migrationBuilder.DropIndex(
                name: "IX_Posts_ExternalForkSourceId_CreatedAt",
                table: "Posts");

            migrationBuilder.DropIndex(
                name: "IX_Posts_ForkTypeId_CreatedAt",
                table: "Posts");

            migrationBuilder.DropColumn(
                name: "ExternalForkContext",
                table: "Posts");

            migrationBuilder.DropColumn(
                name: "ExternalForkQuote",
                table: "Posts");

            migrationBuilder.DropColumn(
                name: "ExternalForkSourceId",
                table: "Posts");

            migrationBuilder.DropColumn(
                name: "ForkTypeId",
                table: "Posts");
        }
    }
}
