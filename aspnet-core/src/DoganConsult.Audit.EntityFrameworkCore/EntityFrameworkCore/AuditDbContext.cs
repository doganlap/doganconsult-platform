using Microsoft.EntityFrameworkCore;
using Volo.Abp.AuditLogging.EntityFrameworkCore;
using Volo.Abp.BackgroundJobs.EntityFrameworkCore;
using Volo.Abp.Data;
using Volo.Abp.DependencyInjection;
using Volo.Abp.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore.Modeling;
using Volo.Abp.FeatureManagement.EntityFrameworkCore;
using Volo.Abp.Identity;
using Volo.Abp.Identity.EntityFrameworkCore;
using Volo.Abp.OpenIddict.EntityFrameworkCore;
using Volo.Abp.PermissionManagement.EntityFrameworkCore;
using Volo.Abp.SettingManagement.EntityFrameworkCore;
using Volo.Abp.TenantManagement;
using Volo.Abp.TenantManagement.EntityFrameworkCore;
using DoganConsult.Audit.AuditLogs;
using DoganConsult.Audit.Approvals;

namespace DoganConsult.Audit.EntityFrameworkCore;

[ReplaceDbContext(typeof(IIdentityDbContext))]
[ReplaceDbContext(typeof(ITenantManagementDbContext))]
[ConnectionStringName("Default")]
public class AuditDbContext :
    AbpDbContext<AuditDbContext>,
    IIdentityDbContext,
    ITenantManagementDbContext
{
    /* Add DbSet properties for your Aggregate Roots / Entities here. */
    public DbSet<AuditLog> AuditLogs { get; set; }
    public DbSet<ApprovalRequest> ApprovalRequests { get; set; }
    public DbSet<ApprovalHistory> ApprovalHistories { get; set; }

    #region Entities from the modules

    /* Notice: We only implemented IIdentityDbContext and ITenantManagementDbContext
     * and replaced them for this DbContext. This allows you to perform JOIN
     * queries for the entities of these modules over the repositories easily. You
     * typically don't need that for other modules. But, if you need, you can
     * implement the DbContext interface of the needed module and use ReplaceDbContext
     * attribute just like IIdentityDbContext and ITenantManagementDbContext.
     *
     * More info: Replacing a DbContext of a module ensures that the related module
     * uses this DbContext on runtime. Otherwise, it will use its own DbContext class.
     */

    //Identity
    public DbSet<IdentityUser> Users { get; set; }
    public DbSet<IdentityRole> Roles { get; set; }
    public DbSet<IdentityClaimType> ClaimTypes { get; set; }
    public DbSet<OrganizationUnit> OrganizationUnits { get; set; }
    public DbSet<IdentitySecurityLog> SecurityLogs { get; set; }
    public DbSet<IdentityLinkUser> LinkUsers { get; set; }
    public DbSet<IdentityUserDelegation> UserDelegations { get; set; }
    public DbSet<IdentitySession> Sessions { get; set; }
    // Tenant Management
    public DbSet<Tenant> Tenants { get; set; }
    public DbSet<TenantConnectionString> TenantConnectionStrings { get; set; }

    #endregion

    public AuditDbContext(DbContextOptions<AuditDbContext> options)
        : base(options)
    {

    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        /* Include modules to your migration db context */

        builder.ConfigurePermissionManagement();
        builder.ConfigureSettingManagement();
        builder.ConfigureBackgroundJobs();
        builder.ConfigureAuditLogging();
        builder.ConfigureIdentity();
        builder.ConfigureOpenIddict();
        builder.ConfigureFeatureManagement();
        builder.ConfigureTenantManagement();

        /* Configure your own tables/entities inside here */

        builder.Entity<AuditLog>(b =>
        {
            b.ToTable(AuditConsts.DbTablePrefix + "AuditLogs", AuditConsts.DbSchema);
            // ABP auto-configures FullAuditedAggregateRoot properties
            
            b.Property(x => x.Action).IsRequired().HasMaxLength(200);
            b.Property(x => x.EntityType).IsRequired().HasMaxLength(200);
            b.Property(x => x.EntityId).HasMaxLength(200);
            b.Property(x => x.UserId).HasMaxLength(200);
            b.Property(x => x.UserName).HasMaxLength(200);
            b.Property(x => x.Description).HasMaxLength(2000);
            b.Property(x => x.Status).HasMaxLength(50);
            b.Property(x => x.Changes).HasMaxLength(2000);
            b.Property(x => x.IpAddress).HasMaxLength(200);
            b.Property(x => x.UserAgent).HasMaxLength(500);

            b.HasIndex(x => x.EntityType);
            b.HasIndex(x => x.EntityId);
            b.HasIndex(x => x.UserId);
            b.HasIndex(x => x.CreationTime);
        });

        // ApprovalRequest configuration
        builder.Entity<ApprovalRequest>(b =>
        {
            b.ToTable(AuditConsts.DbTablePrefix + "ApprovalRequests", AuditConsts.DbSchema);
            b.ConfigureByConvention();

            b.Property(x => x.RequestNumber).IsRequired().HasMaxLength(50);
            b.Property(x => x.EntityName).IsRequired().HasMaxLength(256);
            b.Property(x => x.RequesterName).IsRequired().HasMaxLength(256);
            b.Property(x => x.RequesterEmail).HasMaxLength(256);
            b.Property(x => x.AssignedApproverName).HasMaxLength(256);
            b.Property(x => x.ApprovedByName).HasMaxLength(256);
            b.Property(x => x.RequestReason).HasMaxLength(2000);
            b.Property(x => x.ApprovalComments).HasMaxLength(2000);
            b.Property(x => x.RequestedAction).IsRequired().HasMaxLength(64);

            b.HasIndex(x => x.RequestNumber).IsUnique();
            b.HasIndex(x => x.Status);
            b.HasIndex(x => x.EntityType);
            b.HasIndex(x => x.EntityId);
            b.HasIndex(x => x.RequesterId);
            b.HasIndex(x => x.AssignedApproverId);
            b.HasIndex(x => new { x.EntityType, x.EntityId });
        });

        // ApprovalHistory configuration
        builder.Entity<ApprovalHistory>(b =>
        {
            b.ToTable(AuditConsts.DbTablePrefix + "ApprovalHistories", AuditConsts.DbSchema);
            b.ConfigureByConvention();

            b.Property(x => x.Action).IsRequired().HasMaxLength(64);
            b.Property(x => x.ActorName).IsRequired().HasMaxLength(256);
            b.Property(x => x.Comments).HasMaxLength(2000);
            b.Property(x => x.IpAddress).HasMaxLength(64);
            b.Property(x => x.UserAgent).HasMaxLength(512);

            b.HasIndex(x => x.ApprovalRequestId);
            b.HasIndex(x => x.ActorId);
            b.HasIndex(x => x.CreationTime);
        });
    }
}
