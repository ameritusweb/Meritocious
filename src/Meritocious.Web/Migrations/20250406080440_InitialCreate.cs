using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Meritocious.Web.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AspNetRoles",
                columns: table => new
                {
                    Id = table.Column<string>(type: "varchar(26)", unicode: false, maxLength: 26, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUsers",
                columns: table => new
                {
                    Id = table.Column<string>(type: "varchar(26)", unicode: false, maxLength: 26, nullable: false),
                    MeritScore = table.Column<decimal>(type: "decimal(5,2)", precision: 5, scale: 2, nullable: false, defaultValue: 0.00m),
                    LastCalculated = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastLoginAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    AvatarUrl = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DisplayName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Bio = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    EmailNotificationsEnabled = table.Column<bool>(type: "bit", nullable: false),
                    PublicProfile = table.Column<bool>(type: "bit", nullable: false),
                    PreferredTags = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TimeZone = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Language = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsLocked = table.Column<bool>(type: "bit", nullable: false),
                    IsEmailVerified = table.Column<bool>(type: "bit", nullable: false),
                    LastActivityAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastActiveAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    TwoFactorRequired = table.Column<bool>(type: "bit", nullable: false),
                    TwoFactorEnabled = table.Column<bool>(type: "bit", nullable: false),
                    UserName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    NormalizedUserName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    Email = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    NormalizedEmail = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    EmailConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    SecurityStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumberConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    LockoutEnd = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    LockoutEnabled = table.Column<bool>(type: "bit", nullable: false),
                    AccessFailedCount = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUsers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ContentSimilarity",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ContentId1 = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ContentId2 = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    SimilarityScore = table.Column<decimal>(type: "decimal(5,2)", precision: 5, scale: 2, nullable: false),
                    CalculatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UlidId = table.Column<string>(type: "varchar(26)", unicode: false, maxLength: 26, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ContentSimilarity", x => x.Id);
                    table.CheckConstraint("CK_ContentSimilarity_IdOrder", "ContentId1 < ContentId2");
                });

            migrationBuilder.CreateTable(
                name: "ContentTopics",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ContentId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ContentType = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Topic = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Relevance = table.Column<decimal>(type: "decimal(5,2)", precision: 5, scale: 2, nullable: false),
                    ExtractedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UlidId = table.Column<string>(type: "varchar(26)", unicode: false, maxLength: 26, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ContentTopics", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "LoginAttempts",
                columns: table => new
                {
                    UlidId = table.Column<string>(type: "varchar(26)", unicode: false, maxLength: 26, nullable: false),
                    Username = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    Success = table.Column<bool>(type: "bit", nullable: false),
                    IsSuspicious = table.Column<bool>(type: "bit", nullable: false),
                    IpAddress = table.Column<string>(type: "nvarchar(45)", maxLength: 45, nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UserAgent = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    FailureReason = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Timestamp = table.Column<DateTime>(type: "datetime2", nullable: false),
                    AuthMethod = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Location = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Device = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LoginAttempts", x => x.UlidId);
                });

            migrationBuilder.CreateTable(
                name: "MeritScoreTypes",
                columns: table => new
                {
                    UlidId = table.Column<string>(type: "varchar(26)", unicode: false, maxLength: 26, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Weight = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MeritScoreTypes", x => x.UlidId);
                });

            migrationBuilder.CreateTable(
                name: "Substacks",
                columns: table => new
                {
                    UlidId = table.Column<string>(type: "varchar(26)", unicode: false, maxLength: 26, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Subdomain = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    CustomDomain = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    AuthorName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: false),
                    LogoUrl = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    CoverImageUrl = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    TwitterHandle = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastUpdated = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastPostDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FollowerCount = table.Column<int>(type: "int", nullable: false),
                    PostCount = table.Column<int>(type: "int", nullable: false),
                    ImportedPostCount = table.Column<int>(type: "int", nullable: false),
                    TotalRemixes = table.Column<int>(type: "int", nullable: false),
                    TotalComments = table.Column<int>(type: "int", nullable: false),
                    TotalViews = table.Column<int>(type: "int", nullable: false),
                    UniqueViewers = table.Column<int>(type: "int", nullable: false),
                    AvgPostLength = table.Column<int>(type: "int", nullable: false),
                    AvgCommentLength = table.Column<int>(type: "int", nullable: false),
                    AvgMeritScore = table.Column<double>(type: "float", nullable: false),
                    PostsLastWeek = table.Column<int>(type: "int", nullable: false),
                    PostsLastMonth = table.Column<int>(type: "int", nullable: false),
                    EngagementRate = table.Column<int>(type: "int", nullable: false),
                    GrowthRate = table.Column<double>(type: "float", nullable: false),
                    IsVerified = table.Column<bool>(type: "bit", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Substacks", x => x.UlidId);
                });

            migrationBuilder.CreateTable(
                name: "TrendingContents",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ContentId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ContentType = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    TrendingScore = table.Column<decimal>(type: "decimal(5,2)", precision: 5, scale: 2, nullable: false),
                    ViewCount = table.Column<int>(type: "int", nullable: false),
                    InteractionCount = table.Column<int>(type: "int", nullable: false),
                    AverageMeritScore = table.Column<decimal>(type: "decimal(5,2)", precision: 5, scale: 2, nullable: false),
                    WindowStart = table.Column<DateTime>(type: "datetime2", nullable: false),
                    WindowEnd = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UlidId = table.Column<string>(type: "varchar(26)", unicode: false, maxLength: 26, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TrendingContents", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetRoleClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RoleId = table.Column<string>(type: "varchar(26)", unicode: false, maxLength: 26, nullable: false),
                    ClaimType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClaimValue = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoleClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetRoleClaims_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AdminActionLogs",
                columns: table => new
                {
                    UlidId = table.Column<string>(type: "varchar(26)", unicode: false, maxLength: 26, nullable: false),
                    AdminUserId = table.Column<string>(type: "varchar(26)", unicode: false, maxLength: 26, nullable: false),
                    Action = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Category = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Details = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: false),
                    IpAddress = table.Column<string>(type: "nvarchar(45)", maxLength: 45, nullable: false),
                    Timestamp = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Metadata = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AdminActionLogs", x => x.UlidId);
                    table.ForeignKey(
                        name: "FK_AdminActionLogs_AspNetUsers_AdminUserId",
                        column: x => x.AdminUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ApiUsageLogs",
                columns: table => new
                {
                    UlidId = table.Column<string>(type: "varchar(26)", unicode: false, maxLength: 26, nullable: false),
                    Endpoint = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    Method = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    StatusCode = table.Column<int>(type: "int", nullable: false),
                    UserId = table.Column<string>(type: "varchar(26)", unicode: false, maxLength: 26, nullable: true),
                    IpAddress = table.Column<string>(type: "nvarchar(45)", maxLength: 45, nullable: false),
                    Timestamp = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DurationMs = table.Column<int>(type: "int", nullable: false),
                    ResponseSize = table.Column<long>(type: "bigint", nullable: false),
                    RequestMetadata = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ApiUsageLogs", x => x.UlidId);
                    table.ForeignKey(
                        name: "FK_ApiUsageLogs_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "varchar(26)", unicode: false, maxLength: 26, nullable: false),
                    ClaimType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClaimValue = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetUserClaims_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserLogins",
                columns: table => new
                {
                    LoginProvider = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ProviderKey = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ProviderDisplayName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UserId = table.Column<string>(type: "varchar(26)", unicode: false, maxLength: 26, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserLogins", x => new { x.LoginProvider, x.ProviderKey });
                    table.ForeignKey(
                        name: "FK_AspNetUserLogins_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserRoles",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "varchar(26)", unicode: false, maxLength: 26, nullable: false),
                    RoleId = table.Column<string>(type: "varchar(26)", unicode: false, maxLength: 26, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserRoles", x => new { x.UserId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserTokens",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "varchar(26)", unicode: false, maxLength: 26, nullable: false),
                    LoginProvider = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Value = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserTokens", x => new { x.UserId, x.LoginProvider, x.Name });
                    table.ForeignKey(
                        name: "FK_AspNetUserTokens_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "BlockedIpAddresses",
                columns: table => new
                {
                    UlidId = table.Column<string>(type: "varchar(26)", unicode: false, maxLength: 26, nullable: false),
                    IpAddress = table.Column<string>(type: "nvarchar(45)", maxLength: 45, nullable: false),
                    Reason = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    BlockedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ExpiresAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    BlockedByUserId = table.Column<string>(type: "varchar(26)", unicode: false, maxLength: 26, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BlockedIpAddresses", x => x.UlidId);
                    table.ForeignKey(
                        name: "FK_BlockedIpAddresses_AspNetUsers_BlockedByUserId",
                        column: x => x.BlockedByUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "ContentReports",
                columns: table => new
                {
                    UlidId = table.Column<string>(type: "varchar(26)", unicode: false, maxLength: 26, nullable: false),
                    ContentId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ContentType = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ReporterId = table.Column<string>(type: "varchar(26)", unicode: false, maxLength: 26, nullable: false),
                    ReportType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    Status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    ModeratorId = table.Column<string>(type: "varchar(26)", unicode: false, maxLength: 26, nullable: true),
                    Resolution = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    ResolvedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ContentReports", x => x.UlidId);
                    table.ForeignKey(
                        name: "FK_ContentReports_AspNetUsers_ModeratorId",
                        column: x => x.ModeratorId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ContentReports_AspNetUsers_ReporterId",
                        column: x => x.ReporterId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ExternalLogins",
                columns: table => new
                {
                    UlidId = table.Column<string>(type: "varchar(26)", unicode: false, maxLength: 26, nullable: false),
                    UserId = table.Column<string>(type: "varchar(26)", unicode: false, maxLength: 26, nullable: false),
                    Provider = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ProviderKey = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    PictureUrl = table.Column<string>(type: "nvarchar(2048)", maxLength: 2048, nullable: false),
                    LastLoginAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    RefreshToken = table.Column<string>(type: "nvarchar(1024)", maxLength: 1024, nullable: false),
                    RefreshTokenExpiresAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExternalLogins", x => x.UlidId);
                    table.ForeignKey(
                        name: "FK_ExternalLogins_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ModerationActions",
                columns: table => new
                {
                    UlidId = table.Column<string>(type: "varchar(26)", unicode: false, maxLength: 26, nullable: false),
                    ContentId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ContentType = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ModeratorId = table.Column<string>(type: "varchar(26)", unicode: false, maxLength: 26, nullable: false),
                    ActionType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Reason = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    ToxicityScores = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AutomatedAnalysis = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ModeratorNotes = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: false),
                    IsAutomated = table.Column<bool>(type: "bit", nullable: false),
                    Outcome = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Severity = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    AppealId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ReviewedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ReviewedById = table.Column<string>(type: "varchar(26)", unicode: false, maxLength: 26, nullable: true),
                    ReviewNotes = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: false),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TagId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PreviousState = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NewState = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Action = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ModerationActions", x => x.UlidId);
                    table.ForeignKey(
                        name: "FK_ModerationActions_AspNetUsers_ModeratorId",
                        column: x => x.ModeratorId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ModerationActions_AspNetUsers_ReviewedById",
                        column: x => x.ReviewedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ReputationBadges",
                columns: table => new
                {
                    UlidId = table.Column<string>(type: "varchar(26)", unicode: false, maxLength: 26, nullable: false),
                    UserId = table.Column<string>(type: "varchar(26)", unicode: false, maxLength: 26, nullable: false),
                    BadgeType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Category = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Level = table.Column<int>(type: "int", nullable: false),
                    Criteria = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Progress = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AwardedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    AwardReason = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReputationBadges", x => x.UlidId);
                    table.ForeignKey(
                        name: "FK_ReputationBadges_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ReputationSnapshots",
                columns: table => new
                {
                    UlidId = table.Column<string>(type: "varchar(26)", unicode: false, maxLength: 26, nullable: false),
                    UserId = table.Column<string>(type: "varchar(26)", unicode: false, maxLength: 26, nullable: false),
                    OverallMeritScore = table.Column<decimal>(type: "decimal(5,2)", precision: 5, scale: 2, nullable: false),
                    MetricSnapshots = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Level = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    TimeFrame = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    StartDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EndDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReputationSnapshots", x => x.UlidId);
                    table.ForeignKey(
                        name: "FK_ReputationSnapshots_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SecurityAuditLogs",
                columns: table => new
                {
                    UlidId = table.Column<string>(type: "varchar(26)", unicode: false, maxLength: 26, nullable: false),
                    EventType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Severity = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: false),
                    IpAddress = table.Column<string>(type: "nvarchar(45)", maxLength: 45, nullable: false),
                    UserId = table.Column<string>(type: "varchar(26)", unicode: false, maxLength: 26, nullable: true),
                    UserAgent = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    Timestamp = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Context = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Level = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Message = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: false),
                    Source = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Exception = table.Column<string>(type: "ntext", nullable: true),
                    AdditionalData = table.Column<string>(type: "ntext", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SecurityAuditLogs", x => x.UlidId);
                    table.ForeignKey(
                        name: "FK_SecurityAuditLogs_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "SecurityEvents",
                columns: table => new
                {
                    UlidId = table.Column<string>(type: "varchar(26)", unicode: false, maxLength: 26, nullable: false),
                    EventType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    UserId = table.Column<string>(type: "varchar(26)", unicode: false, maxLength: 26, nullable: true),
                    IpAddress = table.Column<string>(type: "nvarchar(45)", maxLength: 45, nullable: false),
                    UserAgent = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: false),
                    Severity = table.Column<int>(type: "int", nullable: false),
                    RequiresAction = table.Column<bool>(type: "bit", nullable: false),
                    IsResolved = table.Column<bool>(type: "bit", nullable: false),
                    Timestamp = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ResolvedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ResolvedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ResolutionNotes = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SecurityEvents", x => x.UlidId);
                    table.ForeignKey(
                        name: "FK_SecurityEvents_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "Tags",
                columns: table => new
                {
                    UlidId = table.Column<string>(type: "varchar(26)", unicode: false, maxLength: 26, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Slug = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    ParentTagId = table.Column<string>(type: "varchar(26)", unicode: false, maxLength: 26, nullable: true),
                    Category = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    UseCount = table.Column<int>(type: "int", nullable: false),
                    MeritThreshold = table.Column<decimal>(type: "decimal(5,2)", precision: 5, scale: 2, nullable: false),
                    Status = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Metadata = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FollowerCount = table.Column<int>(type: "int", nullable: false),
                    UserId = table.Column<string>(type: "varchar(26)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tags", x => x.UlidId);
                    table.ForeignKey(
                        name: "FK_Tags_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Tags_Tags_ParentTagId",
                        column: x => x.ParentTagId,
                        principalTable: "Tags",
                        principalColumn: "UlidId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "UserContentInteractions",
                columns: table => new
                {
                    UlidId = table.Column<string>(type: "varchar(26)", unicode: false, maxLength: 26, nullable: false),
                    UserId = table.Column<string>(type: "varchar(26)", unicode: false, maxLength: 26, nullable: false),
                    ContentId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ContentType = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    InteractionType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    EngagementScore = table.Column<decimal>(type: "decimal(5,2)", precision: 5, scale: 2, nullable: false),
                    InteractedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserContentInteractions", x => x.UlidId);
                    table.ForeignKey(
                        name: "FK_UserContentInteractions_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserReputationMetrics",
                columns: table => new
                {
                    UlidId = table.Column<string>(type: "varchar(26)", unicode: false, maxLength: 26, nullable: false),
                    UserId = table.Column<string>(type: "varchar(26)", unicode: false, maxLength: 26, nullable: false),
                    OverallMeritScore = table.Column<decimal>(type: "decimal(5,2)", precision: 5, scale: 2, nullable: false),
                    CategoryScores = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ContributionCounts = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TopicExpertise = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Level = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    TotalContributions = table.Column<int>(type: "int", nullable: false),
                    PositiveInteractions = table.Column<int>(type: "int", nullable: false),
                    NegativeInteractions = table.Column<int>(type: "int", nullable: false),
                    ContentQualityAverage = table.Column<decimal>(type: "decimal(5,2)", precision: 5, scale: 2, nullable: false),
                    CommunityImpact = table.Column<decimal>(type: "decimal(5,2)", precision: 5, scale: 2, nullable: false),
                    BadgeProgress = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserReputationMetrics", x => x.UlidId);
                    table.ForeignKey(
                        name: "FK_UserReputationMetrics_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserSessions",
                columns: table => new
                {
                    UlidId = table.Column<string>(type: "varchar(26)", unicode: false, maxLength: 26, nullable: false),
                    UserId = table.Column<string>(type: "varchar(26)", unicode: false, maxLength: 26, nullable: false),
                    SessionId = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    IpAddress = table.Column<string>(type: "nvarchar(45)", maxLength: 45, nullable: false),
                    UserAgent = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastActivityAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ExpiresAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserSessions", x => x.UlidId);
                    table.ForeignKey(
                        name: "FK_UserSessions_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserTopicPreferences",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    UserId = table.Column<string>(type: "varchar(26)", unicode: false, maxLength: 26, nullable: false),
                    Topic = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Weight = table.Column<decimal>(type: "decimal(5,2)", precision: 5, scale: 2, nullable: false),
                    LastUpdated = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UlidId = table.Column<string>(type: "varchar(26)", unicode: false, maxLength: 26, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserTopicPreferences", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserTopicPreferences_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Posts",
                columns: table => new
                {
                    UlidId = table.Column<string>(type: "varchar(26)", unicode: false, maxLength: 26, nullable: false),
                    Title = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Content = table.Column<string>(type: "ntext", nullable: false),
                    AuthorId = table.Column<string>(type: "varchar(26)", unicode: false, maxLength: 26, nullable: false),
                    ParentPostId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ParentPostUlidId = table.Column<string>(type: "varchar(26)", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    IsDraft = table.Column<bool>(type: "bit", nullable: false),
                    SubstackId = table.Column<string>(type: "varchar(26)", unicode: false, maxLength: 26, nullable: false),
                    EngagementScore = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    MeritComponents = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AverageTimeSpentSeconds = table.Column<decimal>(type: "decimal(10,2)", precision: 10, scale: 2, nullable: false, defaultValue: 0.00m),
                    PublishedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    SynthesisMap = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Posts", x => x.UlidId);
                    table.UniqueConstraint("AK_Posts_Id", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Posts_AspNetUsers_AuthorId",
                        column: x => x.AuthorId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Posts_Posts_ParentPostUlidId",
                        column: x => x.ParentPostUlidId,
                        principalTable: "Posts",
                        principalColumn: "UlidId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Posts_Substacks_SubstackId",
                        column: x => x.SubstackId,
                        principalTable: "Substacks",
                        principalColumn: "UlidId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SubstackFollowers",
                columns: table => new
                {
                    SubstackId = table.Column<string>(type: "varchar(26)", unicode: false, maxLength: 26, nullable: false),
                    UserId = table.Column<string>(type: "varchar(26)", unicode: false, maxLength: 26, nullable: false),
                    UlidId = table.Column<string>(type: "varchar(26)", unicode: false, maxLength: 26, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SubstackFollowers", x => new { x.SubstackId, x.UserId });
                    table.ForeignKey(
                        name: "FK_SubstackFollowers_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SubstackFollowers_Substacks_SubstackId",
                        column: x => x.SubstackId,
                        principalTable: "Substacks",
                        principalColumn: "UlidId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SubstackTopics",
                columns: table => new
                {
                    SubstacksUlidId = table.Column<string>(type: "varchar(26)", nullable: false),
                    TopicsId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SubstackTopics", x => new { x.SubstacksUlidId, x.TopicsId });
                    table.ForeignKey(
                        name: "FK_SubstackTopics_ContentTopics_TopicsId",
                        column: x => x.TopicsId,
                        principalTable: "ContentTopics",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SubstackTopics_Substacks_SubstacksUlidId",
                        column: x => x.SubstacksUlidId,
                        principalTable: "Substacks",
                        principalColumn: "UlidId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SubstackUser",
                columns: table => new
                {
                    FollowedSubstacksUlidId = table.Column<string>(type: "varchar(26)", nullable: false),
                    FollowersId = table.Column<string>(type: "varchar(26)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SubstackUser", x => new { x.FollowedSubstacksUlidId, x.FollowersId });
                    table.ForeignKey(
                        name: "FK_SubstackUser_AspNetUsers_FollowersId",
                        column: x => x.FollowersId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SubstackUser_Substacks_FollowedSubstacksUlidId",
                        column: x => x.FollowedSubstacksUlidId,
                        principalTable: "Substacks",
                        principalColumn: "UlidId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ContentModerationEvents",
                columns: table => new
                {
                    UlidId = table.Column<string>(type: "varchar(26)", unicode: false, maxLength: 26, nullable: false),
                    ContentId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ContentType = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ActionId = table.Column<string>(type: "varchar(26)", unicode: false, maxLength: 26, nullable: false),
                    Reason = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    IsAutomated = table.Column<bool>(type: "bit", nullable: false),
                    ModeratorId = table.Column<string>(type: "varchar(26)", unicode: false, maxLength: 26, nullable: false),
                    ModeratedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ContentModerationEvents", x => x.UlidId);
                    table.ForeignKey(
                        name: "FK_ContentModerationEvents_AspNetUsers_ModeratorId",
                        column: x => x.ModeratorId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ContentModerationEvents_ModerationActions_ActionId",
                        column: x => x.ActionId,
                        principalTable: "ModerationActions",
                        principalColumn: "UlidId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ModerationAppeals",
                columns: table => new
                {
                    UlidId = table.Column<string>(type: "varchar(26)", unicode: false, maxLength: 26, nullable: false),
                    ModerationActionId = table.Column<string>(type: "varchar(26)", unicode: false, maxLength: 26, nullable: false),
                    AppealerId = table.Column<string>(type: "varchar(26)", unicode: false, maxLength: 26, nullable: false),
                    Reason = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: false),
                    AdditionalContext = table.Column<string>(type: "nvarchar(max)", maxLength: 5000, nullable: false),
                    Status = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ReviewerId = table.Column<string>(type: "varchar(26)", unicode: false, maxLength: 26, nullable: true),
                    ReviewerNotes = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: false),
                    Decision = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ReviewedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ModerationAppeals", x => x.UlidId);
                    table.ForeignKey(
                        name: "FK_ModerationAppeals_AspNetUsers_AppealerId",
                        column: x => x.AppealerId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ModerationAppeals_AspNetUsers_ReviewerId",
                        column: x => x.ReviewerId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ModerationAppeals_ModerationActions_ModerationActionId",
                        column: x => x.ModerationActionId,
                        principalTable: "ModerationActions",
                        principalColumn: "UlidId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ModerationEffects",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ModerationActionId = table.Column<string>(type: "varchar(26)", unicode: false, maxLength: 26, nullable: false),
                    EffectType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    EffectData = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ExpiresAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsReverted = table.Column<bool>(type: "bit", nullable: false),
                    RevertedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    RevertReason = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    UlidId = table.Column<string>(type: "varchar(26)", unicode: false, maxLength: 26, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ModerationEffects", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ModerationEffects_ModerationActions_ModerationActionId",
                        column: x => x.ModerationActionId,
                        principalTable: "ModerationActions",
                        principalColumn: "UlidId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TagRelationships",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    SourceTagId = table.Column<string>(type: "varchar(26)", unicode: false, maxLength: 26, nullable: false),
                    RelatedTagId = table.Column<string>(type: "varchar(26)", unicode: false, maxLength: 26, nullable: false),
                    RelationType = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Strength = table.Column<decimal>(type: "decimal(5,2)", precision: 5, scale: 2, nullable: false),
                    CreatorId = table.Column<string>(type: "varchar(26)", unicode: false, maxLength: 26, nullable: false),
                    IsBidirectional = table.Column<bool>(type: "bit", nullable: false),
                    IsApproved = table.Column<bool>(type: "bit", nullable: false),
                    ApprovedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ApprovedById = table.Column<string>(type: "varchar(26)", unicode: false, maxLength: 26, nullable: true),
                    ParentTagId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ChildTagId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UlidId = table.Column<string>(type: "varchar(26)", unicode: false, maxLength: 26, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TagRelationships", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TagRelationships_AspNetUsers_ApprovedById",
                        column: x => x.ApprovedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TagRelationships_AspNetUsers_CreatorId",
                        column: x => x.CreatorId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TagRelationships_Tags_RelatedTagId",
                        column: x => x.RelatedTagId,
                        principalTable: "Tags",
                        principalColumn: "UlidId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TagRelationships_Tags_SourceTagId",
                        column: x => x.SourceTagId,
                        principalTable: "Tags",
                        principalColumn: "UlidId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "TagSynonyms",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    SourceTagId = table.Column<string>(type: "varchar(26)", unicode: false, maxLength: 26, nullable: false),
                    TargetTagId = table.Column<string>(type: "varchar(26)", unicode: false, maxLength: 26, nullable: false),
                    CreatedById = table.Column<string>(type: "varchar(26)", unicode: false, maxLength: 26, nullable: false),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsApproved = table.Column<bool>(type: "bit", nullable: false),
                    ApprovedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ApprovedById = table.Column<string>(type: "varchar(26)", unicode: false, maxLength: 26, nullable: true),
                    UlidId = table.Column<string>(type: "varchar(26)", unicode: false, maxLength: 26, nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TagSynonyms", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TagSynonyms_AspNetUsers_ApprovedById",
                        column: x => x.ApprovedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TagSynonyms_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TagSynonyms_Tags_SourceTagId",
                        column: x => x.SourceTagId,
                        principalTable: "Tags",
                        principalColumn: "UlidId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TagSynonyms_Tags_TargetTagId",
                        column: x => x.TargetTagId,
                        principalTable: "Tags",
                        principalColumn: "UlidId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TagWikis",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    TagId = table.Column<string>(type: "varchar(26)", unicode: false, maxLength: 26, nullable: false),
                    Content = table.Column<string>(type: "ntext", nullable: false),
                    VersionNumber = table.Column<int>(type: "int", nullable: false),
                    EditorId = table.Column<string>(type: "varchar(26)", unicode: false, maxLength: 26, nullable: false),
                    EditReason = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    IsApproved = table.Column<bool>(type: "bit", nullable: false),
                    ApprovedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ApprovedById = table.Column<string>(type: "varchar(26)", unicode: false, maxLength: 26, nullable: true),
                    UlidId = table.Column<string>(type: "varchar(26)", unicode: false, maxLength: 26, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TagWikis", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TagWikis_AspNetUsers_ApprovedById",
                        column: x => x.ApprovedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TagWikis_AspNetUsers_EditorId",
                        column: x => x.EditorId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TagWikis_Tags_TagId",
                        column: x => x.TagId,
                        principalTable: "Tags",
                        principalColumn: "UlidId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Comments",
                columns: table => new
                {
                    UlidId = table.Column<string>(type: "varchar(26)", unicode: false, maxLength: 26, nullable: false),
                    Content = table.Column<string>(type: "ntext", nullable: false),
                    PostId = table.Column<string>(type: "varchar(26)", unicode: false, maxLength: 26, nullable: false),
                    AuthorId = table.Column<string>(type: "varchar(26)", unicode: false, maxLength: 26, nullable: false),
                    ParentCommentId = table.Column<string>(type: "varchar(26)", unicode: false, maxLength: 26, nullable: true),
                    MeritScore = table.Column<decimal>(type: "decimal(5,2)", precision: 5, scale: 2, nullable: false, defaultValue: 0.00m),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Comments", x => x.UlidId);
                    table.ForeignKey(
                        name: "FK_Comments_AspNetUsers_AuthorId",
                        column: x => x.AuthorId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Comments_Comments_ParentCommentId",
                        column: x => x.ParentCommentId,
                        principalTable: "Comments",
                        principalColumn: "UlidId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Comments_Posts_PostId",
                        column: x => x.PostId,
                        principalTable: "Posts",
                        principalColumn: "UlidId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ContentSimilarities",
                columns: table => new
                {
                    UlidId = table.Column<string>(type: "varchar(26)", unicode: false, maxLength: 26, nullable: false),
                    ContentId1 = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ContentId2 = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SimilarityScore = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Algorithm = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LastUpdated = table.Column<DateTime>(type: "datetime2", nullable: false),
                    NeedsUpdate = table.Column<bool>(type: "bit", nullable: false),
                    UpdatePriority = table.Column<int>(type: "int", nullable: false),
                    Content1UlidId = table.Column<string>(type: "varchar(26)", nullable: false),
                    Content2UlidId = table.Column<string>(type: "varchar(26)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ContentSimilarities", x => x.UlidId);
                    table.ForeignKey(
                        name: "FK_ContentSimilarities_Posts_Content1UlidId",
                        column: x => x.Content1UlidId,
                        principalTable: "Posts",
                        principalColumn: "UlidId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ContentSimilarities_Posts_Content2UlidId",
                        column: x => x.Content2UlidId,
                        principalTable: "Posts",
                        principalColumn: "UlidId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ContentVersions",
                columns: table => new
                {
                    UlidId = table.Column<string>(type: "varchar(26)", unicode: false, maxLength: 26, nullable: false),
                    ContentId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ContentType = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    VersionNumber = table.Column<int>(type: "int", nullable: false),
                    PostId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Content = table.Column<string>(type: "ntext", nullable: false),
                    EditorId = table.Column<string>(type: "varchar(26)", unicode: false, maxLength: 26, nullable: false),
                    EditReason = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    MeritScore = table.Column<decimal>(type: "decimal(5,2)", precision: 5, scale: 2, nullable: false),
                    MeritScoreComponents = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsModerationEdit = table.Column<bool>(type: "bit", nullable: false),
                    ModeratorNotes = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    EditType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PostUlidId = table.Column<string>(type: "varchar(26)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ContentVersions", x => x.UlidId);
                    table.ForeignKey(
                        name: "FK_ContentVersions_AspNetUsers_EditorId",
                        column: x => x.EditorId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ContentVersions_Posts_PostUlidId",
                        column: x => x.PostUlidId,
                        principalTable: "Posts",
                        principalColumn: "UlidId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MeritScore",
                columns: table => new
                {
                    UlidId = table.Column<string>(type: "varchar(26)", unicode: false, maxLength: 26, nullable: false),
                    ContentId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ContentType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ScoreTypeId = table.Column<string>(type: "varchar(26)", unicode: false, maxLength: 26, nullable: false),
                    Score = table.Column<decimal>(type: "decimal(5,2)", precision: 5, scale: 2, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MeritScore", x => x.UlidId);
                    table.ForeignKey(
                        name: "FK_MeritScore_MeritScoreTypes_ScoreTypeId",
                        column: x => x.ScoreTypeId,
                        principalTable: "MeritScoreTypes",
                        principalColumn: "UlidId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_MeritScore_Posts_ContentId",
                        column: x => x.ContentId,
                        principalTable: "Posts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MeritScoreHistory",
                columns: table => new
                {
                    UlidId = table.Column<string>(type: "varchar(26)", unicode: false, maxLength: 26, nullable: false),
                    ContentId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ContentType = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Score = table.Column<decimal>(type: "decimal(5,2)", precision: 5, scale: 2, nullable: false),
                    Components = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ModelVersion = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Explanations = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Context = table.Column<string>(type: "ntext", nullable: false),
                    EvaluatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsRecalculation = table.Column<bool>(type: "bit", nullable: false),
                    RecalculationReason = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    Timestamp = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UserId = table.Column<string>(type: "varchar(26)", unicode: false, maxLength: 26, nullable: false),
                    Reason = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MeritScoreHistory", x => x.UlidId);
                    table.ForeignKey(
                        name: "FK_MeritScoreHistory_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_MeritScoreHistory_Posts_ContentId",
                        column: x => x.ContentId,
                        principalTable: "Posts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Note",
                columns: table => new
                {
                    UlidId = table.Column<string>(type: "varchar(26)", unicode: false, maxLength: 26, nullable: false),
                    PostId = table.Column<string>(type: "varchar(26)", unicode: false, maxLength: 26, nullable: false),
                    Type = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Content = table.Column<string>(type: "ntext", nullable: false),
                    RelatedSourceIds = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Confidence = table.Column<decimal>(type: "decimal(5,2)", precision: 5, scale: 2, nullable: false),
                    IsApplied = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Note", x => x.UlidId);
                    table.ForeignKey(
                        name: "FK_Note_Posts_PostId",
                        column: x => x.PostId,
                        principalTable: "Posts",
                        principalColumn: "UlidId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PostEngagement",
                columns: table => new
                {
                    UlidId = table.Column<string>(type: "varchar(26)", unicode: false, maxLength: 26, nullable: false),
                    Views = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    UniqueViews = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    Likes = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    Comments = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    Forks = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    Shares = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    AverageTimeSpentSeconds = table.Column<decimal>(type: "decimal(10,2)", precision: 10, scale: 2, nullable: false, defaultValue: 0m),
                    BounceRate = table.Column<decimal>(type: "decimal(5,2)", precision: 5, scale: 2, nullable: false, defaultValue: 0m),
                    ContributorCount = table.Column<int>(type: "int", nullable: false),
                    CitationCount = table.Column<int>(type: "int", nullable: false),
                    ReferenceCount = table.Column<int>(type: "int", nullable: false),
                    ViewsByRegion = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ViewsByPlatform = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ViewTrend = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SourceInfluenceScores = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PeakEngagementTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    EngagementVelocity = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    SentimentScore = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    TopEngagementSources = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PostId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    PostUlidId = table.Column<string>(type: "varchar(26)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PostEngagement", x => x.UlidId);
                    table.ForeignKey(
                        name: "FK_PostEngagement_Posts_PostUlidId",
                        column: x => x.PostUlidId,
                        principalTable: "Posts",
                        principalColumn: "UlidId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PostRelations",
                columns: table => new
                {
                    ParentId = table.Column<string>(type: "varchar(26)", unicode: false, maxLength: 26, nullable: false),
                    ChildId = table.Column<string>(type: "varchar(26)", unicode: false, maxLength: 26, nullable: false),
                    RelationType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    OrderIndex = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    Role = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Context = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    RelevanceScore = table.Column<decimal>(type: "decimal(5,2)", precision: 5, scale: 2, nullable: false, defaultValue: 0.00m),
                    UlidId = table.Column<string>(type: "varchar(26)", unicode: false, maxLength: 26, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PostRelations", x => new { x.ParentId, x.ChildId });
                    table.ForeignKey(
                        name: "FK_PostRelations_Posts_ChildId",
                        column: x => x.ChildId,
                        principalTable: "Posts",
                        principalColumn: "UlidId");
                    table.ForeignKey(
                        name: "FK_PostRelations_Posts_ParentId",
                        column: x => x.ParentId,
                        principalTable: "Posts",
                        principalColumn: "UlidId");
                });

            migrationBuilder.CreateTable(
                name: "PostTag",
                columns: table => new
                {
                    PostsUlidId = table.Column<string>(type: "varchar(26)", nullable: false),
                    TagsUlidId = table.Column<string>(type: "varchar(26)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PostTag", x => new { x.PostsUlidId, x.TagsUlidId });
                    table.ForeignKey(
                        name: "FK_PostTag_Posts_PostsUlidId",
                        column: x => x.PostsUlidId,
                        principalTable: "Posts",
                        principalColumn: "UlidId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PostTag_Tags_TagsUlidId",
                        column: x => x.TagsUlidId,
                        principalTable: "Tags",
                        principalColumn: "UlidId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PostTags",
                columns: table => new
                {
                    PostId = table.Column<string>(type: "varchar(26)", unicode: false, maxLength: 26, nullable: false),
                    TagId = table.Column<string>(type: "varchar(26)", unicode: false, maxLength: 26, nullable: false),
                    UlidId = table.Column<string>(type: "varchar(26)", unicode: false, maxLength: 26, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PostTags", x => new { x.PostId, x.TagId });
                    table.ForeignKey(
                        name: "FK_PostTags_Posts_PostId",
                        column: x => x.PostId,
                        principalTable: "Posts",
                        principalColumn: "UlidId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PostTags_Tags_TagId",
                        column: x => x.TagId,
                        principalTable: "Tags",
                        principalColumn: "UlidId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Notifications",
                columns: table => new
                {
                    UlidId = table.Column<string>(type: "varchar(26)", unicode: false, maxLength: 26, nullable: false),
                    UserId = table.Column<string>(type: "varchar(26)", unicode: false, maxLength: 26, nullable: false),
                    Type = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Title = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Message = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    Link = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    IsRead = table.Column<bool>(type: "bit", nullable: false),
                    PostId = table.Column<string>(type: "varchar(26)", unicode: false, maxLength: 26, nullable: false),
                    CommentId = table.Column<string>(type: "varchar(26)", unicode: false, maxLength: 26, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Notifications", x => x.UlidId);
                    table.ForeignKey(
                        name: "FK_Notifications_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Notifications_Comments_CommentId",
                        column: x => x.CommentId,
                        principalTable: "Comments",
                        principalColumn: "UlidId",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_Notifications_Posts_PostId",
                        column: x => x.PostId,
                        principalTable: "Posts",
                        principalColumn: "UlidId",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "ContentDiff",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ContentVersionId = table.Column<string>(type: "varchar(26)", nullable: false),
                    DiffData = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TitleDiff = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    MeritScoreDiff = table.Column<decimal>(type: "decimal(5,2)", precision: 5, scale: 2, nullable: false),
                    ComponentDiffs = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ContentDiff", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ContentDiff_ContentVersions_ContentVersionId",
                        column: x => x.ContentVersionId,
                        principalTable: "ContentVersions",
                        principalColumn: "UlidId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "QuoteLocations",
                columns: table => new
                {
                    UlidId = table.Column<string>(type: "varchar(26)", unicode: false, maxLength: 26, nullable: false),
                    PostSourceId = table.Column<string>(type: "varchar(26)", unicode: false, maxLength: 26, nullable: false),
                    Content = table.Column<string>(type: "ntext", nullable: false),
                    StartPosition = table.Column<int>(type: "int", nullable: false),
                    EndPosition = table.Column<int>(type: "int", nullable: false),
                    Context = table.Column<string>(type: "ntext", nullable: false),
                    PostRelationParentId = table.Column<string>(type: "varchar(26)", unicode: false, maxLength: 26, nullable: false),
                    PostRelationChildId = table.Column<string>(type: "varchar(26)", unicode: false, maxLength: 26, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_QuoteLocations", x => x.UlidId);
                    table.ForeignKey(
                        name: "FK_QuoteLocations_PostRelations_PostRelationParentId_PostRelationChildId",
                        columns: x => new { x.PostRelationParentId, x.PostRelationChildId },
                        principalTable: "PostRelations",
                        principalColumns: new[] { "ParentId", "ChildId" });
                    table.ForeignKey(
                        name: "FK_QuoteLocations_Posts_PostSourceId",
                        column: x => x.PostSourceId,
                        principalTable: "Posts",
                        principalColumn: "UlidId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AdminActionLogs_Action",
                table: "AdminActionLogs",
                column: "Action");

            migrationBuilder.CreateIndex(
                name: "IX_AdminActionLogs_AdminUserId",
                table: "AdminActionLogs",
                column: "AdminUserId");

            migrationBuilder.CreateIndex(
                name: "IX_AdminActionLogs_Category",
                table: "AdminActionLogs",
                column: "Category");

            migrationBuilder.CreateIndex(
                name: "IX_AdminActionLogs_Timestamp",
                table: "AdminActionLogs",
                column: "Timestamp");

            migrationBuilder.CreateIndex(
                name: "IX_ApiUsageLogs_Endpoint",
                table: "ApiUsageLogs",
                column: "Endpoint");

            migrationBuilder.CreateIndex(
                name: "IX_ApiUsageLogs_StatusCode",
                table: "ApiUsageLogs",
                column: "StatusCode");

            migrationBuilder.CreateIndex(
                name: "IX_ApiUsageLogs_Timestamp",
                table: "ApiUsageLogs",
                column: "Timestamp");

            migrationBuilder.CreateIndex(
                name: "IX_ApiUsageLogs_UserId",
                table: "ApiUsageLogs",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetRoleClaims_RoleId",
                table: "AspNetRoleClaims",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "RoleNameIndex",
                table: "AspNetRoles",
                column: "NormalizedName",
                unique: true,
                filter: "[NormalizedName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserClaims_UserId",
                table: "AspNetUserClaims",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserLogins_UserId",
                table: "AspNetUserLogins",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserRoles_RoleId",
                table: "AspNetUserRoles",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "EmailIndex",
                table: "AspNetUsers",
                column: "NormalizedEmail");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_Email",
                table: "AspNetUsers",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_UserName",
                table: "AspNetUsers",
                column: "UserName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "UserNameIndex",
                table: "AspNetUsers",
                column: "NormalizedUserName",
                unique: true,
                filter: "[NormalizedUserName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_BlockedIpAddresses_BlockedAt",
                table: "BlockedIpAddresses",
                column: "BlockedAt");

            migrationBuilder.CreateIndex(
                name: "IX_BlockedIpAddresses_BlockedByUserId",
                table: "BlockedIpAddresses",
                column: "BlockedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_BlockedIpAddresses_ExpiresAt",
                table: "BlockedIpAddresses",
                column: "ExpiresAt");

            migrationBuilder.CreateIndex(
                name: "IX_BlockedIpAddresses_IpAddress",
                table: "BlockedIpAddresses",
                column: "IpAddress",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Comments_AuthorId",
                table: "Comments",
                column: "AuthorId");

            migrationBuilder.CreateIndex(
                name: "IX_Comments_ParentCommentId",
                table: "Comments",
                column: "ParentCommentId");

            migrationBuilder.CreateIndex(
                name: "IX_Comments_PostId",
                table: "Comments",
                column: "PostId");

            migrationBuilder.CreateIndex(
                name: "IX_ContentDiff_ContentVersionId",
                table: "ContentDiff",
                column: "ContentVersionId");

            migrationBuilder.CreateIndex(
                name: "IX_ContentDiff_CreatedAt",
                table: "ContentDiff",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_ContentModerationEvents_ActionId",
                table: "ContentModerationEvents",
                column: "ActionId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ContentModerationEvents_ContentId_ContentType",
                table: "ContentModerationEvents",
                columns: new[] { "ContentId", "ContentType" });

            migrationBuilder.CreateIndex(
                name: "IX_ContentModerationEvents_ModeratedAt",
                table: "ContentModerationEvents",
                column: "ModeratedAt");

            migrationBuilder.CreateIndex(
                name: "IX_ContentModerationEvents_ModeratorId",
                table: "ContentModerationEvents",
                column: "ModeratorId");

            migrationBuilder.CreateIndex(
                name: "IX_ContentReports_ContentId_ContentType",
                table: "ContentReports",
                columns: new[] { "ContentId", "ContentType" });

            migrationBuilder.CreateIndex(
                name: "IX_ContentReports_CreatedAt",
                table: "ContentReports",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_ContentReports_ModeratorId",
                table: "ContentReports",
                column: "ModeratorId");

            migrationBuilder.CreateIndex(
                name: "IX_ContentReports_ReporterId",
                table: "ContentReports",
                column: "ReporterId");

            migrationBuilder.CreateIndex(
                name: "IX_ContentReports_ResolvedAt",
                table: "ContentReports",
                column: "ResolvedAt");

            migrationBuilder.CreateIndex(
                name: "IX_ContentReports_Status",
                table: "ContentReports",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_ContentSimilarities_Content1UlidId",
                table: "ContentSimilarities",
                column: "Content1UlidId");

            migrationBuilder.CreateIndex(
                name: "IX_ContentSimilarities_Content2UlidId",
                table: "ContentSimilarities",
                column: "Content2UlidId");

            migrationBuilder.CreateIndex(
                name: "IX_ContentSimilarity_CalculatedAt",
                table: "ContentSimilarity",
                column: "CalculatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_ContentSimilarity_ContentId1_ContentId2",
                table: "ContentSimilarity",
                columns: new[] { "ContentId1", "ContentId2" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ContentSimilarity_SimilarityScore",
                table: "ContentSimilarity",
                column: "SimilarityScore");

            migrationBuilder.CreateIndex(
                name: "IX_ContentTopics_ContentId_ContentType",
                table: "ContentTopics",
                columns: new[] { "ContentId", "ContentType" });

            migrationBuilder.CreateIndex(
                name: "IX_ContentTopics_ExtractedAt",
                table: "ContentTopics",
                column: "ExtractedAt");

            migrationBuilder.CreateIndex(
                name: "IX_ContentTopics_Topic",
                table: "ContentTopics",
                column: "Topic");

            migrationBuilder.CreateIndex(
                name: "IX_ContentVersions_ContentId_ContentType_VersionNumber",
                table: "ContentVersions",
                columns: new[] { "ContentId", "ContentType", "VersionNumber" });

            migrationBuilder.CreateIndex(
                name: "IX_ContentVersions_CreatedAt",
                table: "ContentVersions",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_ContentVersions_EditorId",
                table: "ContentVersions",
                column: "EditorId");

            migrationBuilder.CreateIndex(
                name: "IX_ContentVersions_PostUlidId",
                table: "ContentVersions",
                column: "PostUlidId");

            migrationBuilder.CreateIndex(
                name: "IX_ExternalLogins_Email",
                table: "ExternalLogins",
                column: "Email");

            migrationBuilder.CreateIndex(
                name: "IX_ExternalLogins_LastLoginAt",
                table: "ExternalLogins",
                column: "LastLoginAt");

            migrationBuilder.CreateIndex(
                name: "IX_ExternalLogins_Provider_ProviderKey",
                table: "ExternalLogins",
                columns: new[] { "Provider", "ProviderKey" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ExternalLogins_UserId",
                table: "ExternalLogins",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_LoginAttempts_IpAddress",
                table: "LoginAttempts",
                column: "IpAddress");

            migrationBuilder.CreateIndex(
                name: "IX_LoginAttempts_IsSuspicious",
                table: "LoginAttempts",
                column: "IsSuspicious");

            migrationBuilder.CreateIndex(
                name: "IX_LoginAttempts_Success",
                table: "LoginAttempts",
                column: "Success");

            migrationBuilder.CreateIndex(
                name: "IX_LoginAttempts_Timestamp",
                table: "LoginAttempts",
                column: "Timestamp");

            migrationBuilder.CreateIndex(
                name: "IX_LoginAttempts_Username",
                table: "LoginAttempts",
                column: "Username");

            migrationBuilder.CreateIndex(
                name: "IX_MeritScore_ContentId_ContentType",
                table: "MeritScore",
                columns: new[] { "ContentId", "ContentType" });

            migrationBuilder.CreateIndex(
                name: "IX_MeritScore_Score",
                table: "MeritScore",
                column: "Score");

            migrationBuilder.CreateIndex(
                name: "IX_MeritScore_ScoreTypeId",
                table: "MeritScore",
                column: "ScoreTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_MeritScoreHistory_ContentId_ContentType",
                table: "MeritScoreHistory",
                columns: new[] { "ContentId", "ContentType" });

            migrationBuilder.CreateIndex(
                name: "IX_MeritScoreHistory_EvaluatedAt",
                table: "MeritScoreHistory",
                column: "EvaluatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_MeritScoreHistory_ModelVersion",
                table: "MeritScoreHistory",
                column: "ModelVersion");

            migrationBuilder.CreateIndex(
                name: "IX_MeritScoreHistory_UserId",
                table: "MeritScoreHistory",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_ModerationActions_ContentId_ContentType",
                table: "ModerationActions",
                columns: new[] { "ContentId", "ContentType" });

            migrationBuilder.CreateIndex(
                name: "IX_ModerationActions_CreatedAt",
                table: "ModerationActions",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_ModerationActions_ModeratorId",
                table: "ModerationActions",
                column: "ModeratorId");

            migrationBuilder.CreateIndex(
                name: "IX_ModerationActions_Outcome",
                table: "ModerationActions",
                column: "Outcome");

            migrationBuilder.CreateIndex(
                name: "IX_ModerationActions_ReviewedById",
                table: "ModerationActions",
                column: "ReviewedById");

            migrationBuilder.CreateIndex(
                name: "IX_ModerationActions_Severity",
                table: "ModerationActions",
                column: "Severity");

            migrationBuilder.CreateIndex(
                name: "IX_ModerationAppeals_AppealerId",
                table: "ModerationAppeals",
                column: "AppealerId");

            migrationBuilder.CreateIndex(
                name: "IX_ModerationAppeals_CreatedAt",
                table: "ModerationAppeals",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_ModerationAppeals_ModerationActionId",
                table: "ModerationAppeals",
                column: "ModerationActionId");

            migrationBuilder.CreateIndex(
                name: "IX_ModerationAppeals_ReviewerId",
                table: "ModerationAppeals",
                column: "ReviewerId");

            migrationBuilder.CreateIndex(
                name: "IX_ModerationAppeals_Status",
                table: "ModerationAppeals",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_ModerationEffects_EffectType",
                table: "ModerationEffects",
                column: "EffectType");

            migrationBuilder.CreateIndex(
                name: "IX_ModerationEffects_ExpiresAt",
                table: "ModerationEffects",
                column: "ExpiresAt");

            migrationBuilder.CreateIndex(
                name: "IX_ModerationEffects_IsReverted",
                table: "ModerationEffects",
                column: "IsReverted");

            migrationBuilder.CreateIndex(
                name: "IX_ModerationEffects_ModerationActionId",
                table: "ModerationEffects",
                column: "ModerationActionId");

            migrationBuilder.CreateIndex(
                name: "IX_Note_IsApplied",
                table: "Note",
                column: "IsApplied");

            migrationBuilder.CreateIndex(
                name: "IX_Note_PostId",
                table: "Note",
                column: "PostId");

            migrationBuilder.CreateIndex(
                name: "IX_Note_Type",
                table: "Note",
                column: "Type");

            migrationBuilder.CreateIndex(
                name: "IX_Notifications_CommentId",
                table: "Notifications",
                column: "CommentId");

            migrationBuilder.CreateIndex(
                name: "IX_Notifications_CreatedAt",
                table: "Notifications",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_Notifications_IsRead",
                table: "Notifications",
                column: "IsRead");

            migrationBuilder.CreateIndex(
                name: "IX_Notifications_PostId",
                table: "Notifications",
                column: "PostId");

            migrationBuilder.CreateIndex(
                name: "IX_Notifications_Type",
                table: "Notifications",
                column: "Type");

            migrationBuilder.CreateIndex(
                name: "IX_Notifications_UserId",
                table: "Notifications",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Notifications_UserId_CreatedAt",
                table: "Notifications",
                columns: new[] { "UserId", "CreatedAt" });

            migrationBuilder.CreateIndex(
                name: "IX_Notifications_UserId_IsRead",
                table: "Notifications",
                columns: new[] { "UserId", "IsRead" });

            migrationBuilder.CreateIndex(
                name: "IX_Notifications_UserId_Type",
                table: "Notifications",
                columns: new[] { "UserId", "Type" });

            migrationBuilder.CreateIndex(
                name: "IX_PostEngagement_EngagementVelocity",
                table: "PostEngagement",
                column: "EngagementVelocity");

            migrationBuilder.CreateIndex(
                name: "IX_PostEngagement_PostId",
                table: "PostEngagement",
                column: "PostId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PostEngagement_PostUlidId",
                table: "PostEngagement",
                column: "PostUlidId");

            migrationBuilder.CreateIndex(
                name: "IX_PostEngagement_Views",
                table: "PostEngagement",
                column: "Views");

            migrationBuilder.CreateIndex(
                name: "IX_PostRelations_ChildId_OrderIndex",
                table: "PostRelations",
                columns: new[] { "ChildId", "OrderIndex" },
                filter: "RelationType = 'remix'")
                .Annotation("SqlServer:Include", new[] { "ParentId", "Role" });

            migrationBuilder.CreateIndex(
                name: "IX_PostRelations_ChildId_RelationType",
                table: "PostRelations",
                columns: new[] { "ChildId", "RelationType" })
                .Annotation("SqlServer:Include", new[] { "Role", "OrderIndex" });

            migrationBuilder.CreateIndex(
                name: "IX_PostRelations_ChildId_RelationType_OrderIndex",
                table: "PostRelations",
                columns: new[] { "ChildId", "RelationType", "OrderIndex" },
                filter: "RelationType = 'remix'")
                .Annotation("SqlServer:Include", new[] { "Role", "RelevanceScore" });

            migrationBuilder.CreateIndex(
                name: "IX_PostRelations_ParentId_ChildId_RelationType",
                table: "PostRelations",
                columns: new[] { "ParentId", "ChildId", "RelationType" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PostRelations_ParentId_RelationType",
                table: "PostRelations",
                columns: new[] { "ParentId", "RelationType" })
                .Annotation("SqlServer:Include", new[] { "Role", "OrderIndex" });

            migrationBuilder.CreateIndex(
                name: "IX_PostRelations_RelationType_CreatedAt",
                table: "PostRelations",
                columns: new[] { "RelationType", "CreatedAt" })
                .Annotation("SqlServer:Include", new[] { "ParentId", "ChildId" });

            migrationBuilder.CreateIndex(
                name: "IX_PostRelations_RelationType_RelevanceScore",
                table: "PostRelations",
                columns: new[] { "RelationType", "RelevanceScore" },
                filter: "RelationType = 'remix'")
                .Annotation("SqlServer:Include", new[] { "Role" });

            migrationBuilder.CreateIndex(
                name: "IX_PostRelations_RelationType_Role_OrderIndex",
                table: "PostRelations",
                columns: new[] { "RelationType", "Role", "OrderIndex" },
                filter: "RelationType = 'remix'")
                .Annotation("SqlServer:Include", new[] { "ParentId", "ChildId" });

            migrationBuilder.CreateIndex(
                name: "IX_Posts_AuthorId",
                table: "Posts",
                column: "AuthorId");

            migrationBuilder.CreateIndex(
                name: "IX_Posts_AuthorId_IsDeleted",
                table: "Posts",
                columns: new[] { "AuthorId", "IsDeleted" });

            migrationBuilder.CreateIndex(
                name: "IX_Posts_CreatedAt",
                table: "Posts",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_Posts_IsDeleted",
                table: "Posts",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_Posts_IsDeleted_CreatedAt",
                table: "Posts",
                columns: new[] { "IsDeleted", "CreatedAt" });

            migrationBuilder.CreateIndex(
                name: "IX_Posts_ParentPostUlidId",
                table: "Posts",
                column: "ParentPostUlidId");

            migrationBuilder.CreateIndex(
                name: "IX_Posts_SubstackId",
                table: "Posts",
                column: "SubstackId");

            migrationBuilder.CreateIndex(
                name: "IX_PostTag_TagsUlidId",
                table: "PostTag",
                column: "TagsUlidId");

            migrationBuilder.CreateIndex(
                name: "IX_PostTags_TagId",
                table: "PostTags",
                column: "TagId");

            migrationBuilder.CreateIndex(
                name: "IX_QuoteLocations_PostRelationParentId_PostRelationChildId",
                table: "QuoteLocations",
                columns: new[] { "PostRelationParentId", "PostRelationChildId" });

            migrationBuilder.CreateIndex(
                name: "IX_QuoteLocations_PostSourceId",
                table: "QuoteLocations",
                column: "PostSourceId");

            migrationBuilder.CreateIndex(
                name: "IX_ReputationBadges_AwardedAt",
                table: "ReputationBadges",
                column: "AwardedAt");

            migrationBuilder.CreateIndex(
                name: "IX_ReputationBadges_Category",
                table: "ReputationBadges",
                column: "Category");

            migrationBuilder.CreateIndex(
                name: "IX_ReputationBadges_UserId_BadgeType_Level",
                table: "ReputationBadges",
                columns: new[] { "UserId", "BadgeType", "Level" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ReputationSnapshots_EndDate",
                table: "ReputationSnapshots",
                column: "EndDate");

            migrationBuilder.CreateIndex(
                name: "IX_ReputationSnapshots_Level",
                table: "ReputationSnapshots",
                column: "Level");

            migrationBuilder.CreateIndex(
                name: "IX_ReputationSnapshots_UserId_TimeFrame_StartDate",
                table: "ReputationSnapshots",
                columns: new[] { "UserId", "TimeFrame", "StartDate" });

            migrationBuilder.CreateIndex(
                name: "IX_SecurityAuditLogs_EventType",
                table: "SecurityAuditLogs",
                column: "EventType");

            migrationBuilder.CreateIndex(
                name: "IX_SecurityAuditLogs_IpAddress",
                table: "SecurityAuditLogs",
                column: "IpAddress");

            migrationBuilder.CreateIndex(
                name: "IX_SecurityAuditLogs_Level",
                table: "SecurityAuditLogs",
                column: "Level");

            migrationBuilder.CreateIndex(
                name: "IX_SecurityAuditLogs_Severity",
                table: "SecurityAuditLogs",
                column: "Severity");

            migrationBuilder.CreateIndex(
                name: "IX_SecurityAuditLogs_Timestamp",
                table: "SecurityAuditLogs",
                column: "Timestamp");

            migrationBuilder.CreateIndex(
                name: "IX_SecurityAuditLogs_UserId",
                table: "SecurityAuditLogs",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_SecurityEvents_CreatedAt",
                table: "SecurityEvents",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_SecurityEvents_EventType",
                table: "SecurityEvents",
                column: "EventType");

            migrationBuilder.CreateIndex(
                name: "IX_SecurityEvents_IpAddress",
                table: "SecurityEvents",
                column: "IpAddress");

            migrationBuilder.CreateIndex(
                name: "IX_SecurityEvents_IpAddress_EventType",
                table: "SecurityEvents",
                columns: new[] { "IpAddress", "EventType" });

            migrationBuilder.CreateIndex(
                name: "IX_SecurityEvents_IsResolved",
                table: "SecurityEvents",
                column: "IsResolved");

            migrationBuilder.CreateIndex(
                name: "IX_SecurityEvents_IsResolved_RequiresAction",
                table: "SecurityEvents",
                columns: new[] { "IsResolved", "RequiresAction" });

            migrationBuilder.CreateIndex(
                name: "IX_SecurityEvents_RequiresAction",
                table: "SecurityEvents",
                column: "RequiresAction");

            migrationBuilder.CreateIndex(
                name: "IX_SecurityEvents_Severity",
                table: "SecurityEvents",
                column: "Severity");

            migrationBuilder.CreateIndex(
                name: "IX_SecurityEvents_Severity_IsResolved",
                table: "SecurityEvents",
                columns: new[] { "Severity", "IsResolved" });

            migrationBuilder.CreateIndex(
                name: "IX_SecurityEvents_UserId",
                table: "SecurityEvents",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_SecurityEvents_UserId_EventType",
                table: "SecurityEvents",
                columns: new[] { "UserId", "EventType" });

            migrationBuilder.CreateIndex(
                name: "IX_SubstackFollowers_UserId",
                table: "SubstackFollowers",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Substacks_AuthorName",
                table: "Substacks",
                column: "AuthorName");

            migrationBuilder.CreateIndex(
                name: "IX_Substacks_AvgMeritScore",
                table: "Substacks",
                column: "AvgMeritScore");

            migrationBuilder.CreateIndex(
                name: "IX_Substacks_CreatedAt",
                table: "Substacks",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_Substacks_CustomDomain",
                table: "Substacks",
                column: "CustomDomain");

            migrationBuilder.CreateIndex(
                name: "IX_Substacks_EngagementRate",
                table: "Substacks",
                column: "EngagementRate");

            migrationBuilder.CreateIndex(
                name: "IX_Substacks_IsVerified",
                table: "Substacks",
                column: "IsVerified");

            migrationBuilder.CreateIndex(
                name: "IX_Substacks_IsVerified_AvgMeritScore",
                table: "Substacks",
                columns: new[] { "IsVerified", "AvgMeritScore" });

            migrationBuilder.CreateIndex(
                name: "IX_Substacks_IsVerified_EngagementRate",
                table: "Substacks",
                columns: new[] { "IsVerified", "EngagementRate" });

            migrationBuilder.CreateIndex(
                name: "IX_Substacks_LastPostDate",
                table: "Substacks",
                column: "LastPostDate");

            migrationBuilder.CreateIndex(
                name: "IX_Substacks_LastPostDate_EngagementRate",
                table: "Substacks",
                columns: new[] { "LastPostDate", "EngagementRate" });

            migrationBuilder.CreateIndex(
                name: "IX_Substacks_Subdomain",
                table: "Substacks",
                column: "Subdomain",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SubstackTopics_TopicsId",
                table: "SubstackTopics",
                column: "TopicsId");

            migrationBuilder.CreateIndex(
                name: "IX_SubstackUser_FollowersId",
                table: "SubstackUser",
                column: "FollowersId");

            migrationBuilder.CreateIndex(
                name: "IX_TagRelationships_ApprovedById",
                table: "TagRelationships",
                column: "ApprovedById");

            migrationBuilder.CreateIndex(
                name: "IX_TagRelationships_CreatorId",
                table: "TagRelationships",
                column: "CreatorId");

            migrationBuilder.CreateIndex(
                name: "IX_TagRelationships_IsApproved",
                table: "TagRelationships",
                column: "IsApproved");

            migrationBuilder.CreateIndex(
                name: "IX_TagRelationships_RelatedTagId",
                table: "TagRelationships",
                column: "RelatedTagId");

            migrationBuilder.CreateIndex(
                name: "IX_TagRelationships_SourceTagId_RelatedTagId_RelationType",
                table: "TagRelationships",
                columns: new[] { "SourceTagId", "RelatedTagId", "RelationType" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TagRelationships_Strength",
                table: "TagRelationships",
                column: "Strength");

            migrationBuilder.CreateIndex(
                name: "IX_Tags_Category",
                table: "Tags",
                column: "Category");

            migrationBuilder.CreateIndex(
                name: "IX_Tags_Name",
                table: "Tags",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Tags_ParentTagId",
                table: "Tags",
                column: "ParentTagId");

            migrationBuilder.CreateIndex(
                name: "IX_Tags_Slug",
                table: "Tags",
                column: "Slug",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Tags_Status",
                table: "Tags",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_Tags_UseCount",
                table: "Tags",
                column: "UseCount");

            migrationBuilder.CreateIndex(
                name: "IX_Tags_UserId",
                table: "Tags",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_TagSynonyms_ApprovedById",
                table: "TagSynonyms",
                column: "ApprovedById");

            migrationBuilder.CreateIndex(
                name: "IX_TagSynonyms_CreatedById",
                table: "TagSynonyms",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_TagSynonyms_IsApproved",
                table: "TagSynonyms",
                column: "IsApproved");

            migrationBuilder.CreateIndex(
                name: "IX_TagSynonyms_SourceTagId",
                table: "TagSynonyms",
                column: "SourceTagId");

            migrationBuilder.CreateIndex(
                name: "IX_TagSynonyms_TargetTagId",
                table: "TagSynonyms",
                column: "TargetTagId");

            migrationBuilder.CreateIndex(
                name: "IX_TagWikis_ApprovedById",
                table: "TagWikis",
                column: "ApprovedById");

            migrationBuilder.CreateIndex(
                name: "IX_TagWikis_EditorId",
                table: "TagWikis",
                column: "EditorId");

            migrationBuilder.CreateIndex(
                name: "IX_TagWikis_IsApproved",
                table: "TagWikis",
                column: "IsApproved");

            migrationBuilder.CreateIndex(
                name: "IX_TagWikis_TagId_VersionNumber",
                table: "TagWikis",
                columns: new[] { "TagId", "VersionNumber" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TrendingContents_ContentId_ContentType",
                table: "TrendingContents",
                columns: new[] { "ContentId", "ContentType" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TrendingContents_TrendingScore",
                table: "TrendingContents",
                column: "TrendingScore");

            migrationBuilder.CreateIndex(
                name: "IX_TrendingContents_WindowStart_WindowEnd",
                table: "TrendingContents",
                columns: new[] { "WindowStart", "WindowEnd" });

            migrationBuilder.CreateIndex(
                name: "IX_UserContentInteractions_ContentId_ContentType_InteractedAt",
                table: "UserContentInteractions",
                columns: new[] { "ContentId", "ContentType", "InteractedAt" });

            migrationBuilder.CreateIndex(
                name: "IX_UserContentInteractions_InteractionType",
                table: "UserContentInteractions",
                column: "InteractionType");

            migrationBuilder.CreateIndex(
                name: "IX_UserContentInteractions_UserId_InteractedAt",
                table: "UserContentInteractions",
                columns: new[] { "UserId", "InteractedAt" });

            migrationBuilder.CreateIndex(
                name: "IX_UserReputationMetrics_Level",
                table: "UserReputationMetrics",
                column: "Level");

            migrationBuilder.CreateIndex(
                name: "IX_UserReputationMetrics_OverallMeritScore",
                table: "UserReputationMetrics",
                column: "OverallMeritScore");

            migrationBuilder.CreateIndex(
                name: "IX_UserReputationMetrics_TotalContributions",
                table: "UserReputationMetrics",
                column: "TotalContributions");

            migrationBuilder.CreateIndex(
                name: "IX_UserReputationMetrics_UserId",
                table: "UserReputationMetrics",
                column: "UserId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_UserSessions_ExpiresAt",
                table: "UserSessions",
                column: "ExpiresAt");

            migrationBuilder.CreateIndex(
                name: "IX_UserSessions_LastActivityAt",
                table: "UserSessions",
                column: "LastActivityAt");

            migrationBuilder.CreateIndex(
                name: "IX_UserSessions_SessionId",
                table: "UserSessions",
                column: "SessionId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_UserSessions_UserId",
                table: "UserSessions",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_UserTopicPreferences_LastUpdated",
                table: "UserTopicPreferences",
                column: "LastUpdated");

            migrationBuilder.CreateIndex(
                name: "IX_UserTopicPreferences_UserId_Topic",
                table: "UserTopicPreferences",
                columns: new[] { "UserId", "Topic" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AdminActionLogs");

            migrationBuilder.DropTable(
                name: "ApiUsageLogs");

            migrationBuilder.DropTable(
                name: "AspNetRoleClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserLogins");

            migrationBuilder.DropTable(
                name: "AspNetUserRoles");

            migrationBuilder.DropTable(
                name: "AspNetUserTokens");

            migrationBuilder.DropTable(
                name: "BlockedIpAddresses");

            migrationBuilder.DropTable(
                name: "ContentDiff");

            migrationBuilder.DropTable(
                name: "ContentModerationEvents");

            migrationBuilder.DropTable(
                name: "ContentReports");

            migrationBuilder.DropTable(
                name: "ContentSimilarities");

            migrationBuilder.DropTable(
                name: "ContentSimilarity");

            migrationBuilder.DropTable(
                name: "ExternalLogins");

            migrationBuilder.DropTable(
                name: "LoginAttempts");

            migrationBuilder.DropTable(
                name: "MeritScore");

            migrationBuilder.DropTable(
                name: "MeritScoreHistory");

            migrationBuilder.DropTable(
                name: "ModerationAppeals");

            migrationBuilder.DropTable(
                name: "ModerationEffects");

            migrationBuilder.DropTable(
                name: "Note");

            migrationBuilder.DropTable(
                name: "Notifications");

            migrationBuilder.DropTable(
                name: "PostEngagement");

            migrationBuilder.DropTable(
                name: "PostTag");

            migrationBuilder.DropTable(
                name: "PostTags");

            migrationBuilder.DropTable(
                name: "QuoteLocations");

            migrationBuilder.DropTable(
                name: "ReputationBadges");

            migrationBuilder.DropTable(
                name: "ReputationSnapshots");

            migrationBuilder.DropTable(
                name: "SecurityAuditLogs");

            migrationBuilder.DropTable(
                name: "SecurityEvents");

            migrationBuilder.DropTable(
                name: "SubstackFollowers");

            migrationBuilder.DropTable(
                name: "SubstackTopics");

            migrationBuilder.DropTable(
                name: "SubstackUser");

            migrationBuilder.DropTable(
                name: "TagRelationships");

            migrationBuilder.DropTable(
                name: "TagSynonyms");

            migrationBuilder.DropTable(
                name: "TagWikis");

            migrationBuilder.DropTable(
                name: "TrendingContents");

            migrationBuilder.DropTable(
                name: "UserContentInteractions");

            migrationBuilder.DropTable(
                name: "UserReputationMetrics");

            migrationBuilder.DropTable(
                name: "UserSessions");

            migrationBuilder.DropTable(
                name: "UserTopicPreferences");

            migrationBuilder.DropTable(
                name: "AspNetRoles");

            migrationBuilder.DropTable(
                name: "ContentVersions");

            migrationBuilder.DropTable(
                name: "MeritScoreTypes");

            migrationBuilder.DropTable(
                name: "ModerationActions");

            migrationBuilder.DropTable(
                name: "Comments");

            migrationBuilder.DropTable(
                name: "PostRelations");

            migrationBuilder.DropTable(
                name: "ContentTopics");

            migrationBuilder.DropTable(
                name: "Tags");

            migrationBuilder.DropTable(
                name: "Posts");

            migrationBuilder.DropTable(
                name: "AspNetUsers");

            migrationBuilder.DropTable(
                name: "Substacks");
        }
    }
}
