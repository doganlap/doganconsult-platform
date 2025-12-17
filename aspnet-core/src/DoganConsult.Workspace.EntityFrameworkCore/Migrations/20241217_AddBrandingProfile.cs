using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DoganConsult.Workspace.EntityFrameworkCore.Migrations
{
    /// <inheritdoc />
    public partial class AddBrandingProfile : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AppBrandingProfiles",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: true),
                    AppDisplayName = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    ProductName = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: true),
                    LogoUrl = table.Column<string>(type: "character varying(512)", maxLength: 512, nullable: true),
                    FaviconUrl = table.Column<string>(type: "character varying(512)", maxLength: 512, nullable: true),
                    PrimaryColor = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    AccentColor = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    DefaultLanguage = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    IsRtl = table.Column<bool>(type: "boolean", nullable: false),
                    HomeRoute = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    CreationTime = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    CreatorId = table.Column<Guid>(type: "uuid", nullable: true),
                    LastModificationTime = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    LastModifierId = table.Column<Guid>(type: "uuid", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    DeleterId = table.Column<Guid>(type: "uuid", nullable: true),
                    DeletionTime = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AppBrandingProfiles", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AppBrandingProfiles_TenantId",
                table: "AppBrandingProfiles",
                column: "TenantId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AppBrandingProfiles");
        }
    }
}
