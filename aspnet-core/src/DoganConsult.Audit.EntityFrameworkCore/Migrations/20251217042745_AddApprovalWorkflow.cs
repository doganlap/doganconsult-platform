using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DoganConsult.Audit.Migrations
{
    /// <inheritdoc />
    public partial class AddApprovalWorkflow : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AppApprovalHistories",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ApprovalRequestId = table.Column<Guid>(type: "uuid", nullable: false),
                    Action = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    PreviousStatus = table.Column<int>(type: "integer", nullable: true),
                    NewStatus = table.Column<int>(type: "integer", nullable: false),
                    ActorId = table.Column<Guid>(type: "uuid", nullable: false),
                    ActorName = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    Comments = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    IpAddress = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true),
                    UserAgent = table.Column<string>(type: "character varying(512)", maxLength: 512, nullable: true),
                    AdditionalData = table.Column<string>(type: "text", nullable: true),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: true),
                    CreationTime = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    CreatorId = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AppApprovalHistories", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AppApprovalRequests",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    RequestNumber = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    EntityType = table.Column<int>(type: "integer", nullable: false),
                    EntityId = table.Column<Guid>(type: "uuid", nullable: false),
                    EntityName = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    Priority = table.Column<int>(type: "integer", nullable: false),
                    RequesterId = table.Column<Guid>(type: "uuid", nullable: false),
                    RequesterName = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    RequesterEmail = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    AssignedApproverId = table.Column<Guid>(type: "uuid", nullable: true),
                    AssignedApproverName = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    ApprovedById = table.Column<Guid>(type: "uuid", nullable: true),
                    ApprovedByName = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    ApprovedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    RequestReason = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    ApprovalComments = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    ExpiresAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    RequestedAction = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    EntitySnapshotBefore = table.Column<string>(type: "text", nullable: true),
                    EntitySnapshotAfter = table.Column<string>(type: "text", nullable: true),
                    OrganizationId = table.Column<Guid>(type: "uuid", nullable: true),
                    WorkflowStep = table.Column<int>(type: "integer", nullable: false),
                    TotalWorkflowSteps = table.Column<int>(type: "integer", nullable: false),
                    Metadata = table.Column<string>(type: "text", nullable: true),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: true),
                    ExtraProperties = table.Column<string>(type: "text", nullable: false),
                    ConcurrencyStamp = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: false),
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
                    table.PrimaryKey("PK_AppApprovalRequests", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AppApprovalHistories_ActorId",
                table: "AppApprovalHistories",
                column: "ActorId");

            migrationBuilder.CreateIndex(
                name: "IX_AppApprovalHistories_ApprovalRequestId",
                table: "AppApprovalHistories",
                column: "ApprovalRequestId");

            migrationBuilder.CreateIndex(
                name: "IX_AppApprovalHistories_CreationTime",
                table: "AppApprovalHistories",
                column: "CreationTime");

            migrationBuilder.CreateIndex(
                name: "IX_AppApprovalRequests_AssignedApproverId",
                table: "AppApprovalRequests",
                column: "AssignedApproverId");

            migrationBuilder.CreateIndex(
                name: "IX_AppApprovalRequests_EntityId",
                table: "AppApprovalRequests",
                column: "EntityId");

            migrationBuilder.CreateIndex(
                name: "IX_AppApprovalRequests_EntityType",
                table: "AppApprovalRequests",
                column: "EntityType");

            migrationBuilder.CreateIndex(
                name: "IX_AppApprovalRequests_EntityType_EntityId",
                table: "AppApprovalRequests",
                columns: new[] { "EntityType", "EntityId" });

            migrationBuilder.CreateIndex(
                name: "IX_AppApprovalRequests_RequesterId",
                table: "AppApprovalRequests",
                column: "RequesterId");

            migrationBuilder.CreateIndex(
                name: "IX_AppApprovalRequests_RequestNumber",
                table: "AppApprovalRequests",
                column: "RequestNumber",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AppApprovalRequests_Status",
                table: "AppApprovalRequests",
                column: "Status");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AppApprovalHistories");

            migrationBuilder.DropTable(
                name: "AppApprovalRequests");
        }
    }
}
