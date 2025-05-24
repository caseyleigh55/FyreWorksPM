using Microsoft.EntityFrameworkCore;
using FyreWorksPM.DataAccess.Data.Models;
using FyreWorksPM.DataAccess.Models;

namespace FyreWorksPM.DataAccess.Data;

/// <summary>
/// Entity Framework Core database context for the FyreWorksPM application.
/// Defines entity sets and connection behavior.
/// </summary>
public class ApplicationDbContext : DbContext
{
    /// <summary>
    /// Constructor that passes options to the base DbContext.
    /// Used for dependency injection.
    /// </summary>
    /// <param name="options">The database context options.</param>
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<BidModel>().ToTable("BidInfo");
        modelBuilder.Entity<SiteInfoModel>().ToTable("BidSiteInfo");
        modelBuilder.Entity<BidTaskModel>().ToTable("BidTasks");

        // BidModel to BidTasks: One-to-many
        modelBuilder.Entity<BidModel>()
            .HasMany(b => b.BidModelTasks)
            .WithOne(t => t.BidTaskModelBid)
            .HasForeignKey(t => t.BidTaskModelBidId)
            .OnDelete(DeleteBehavior.Cascade);

        // BidTaskModel to TaskModel: Many-to-one
        modelBuilder.Entity<BidTaskModel>()
            .HasOne(t => t.BidTaskModelTask)
            .WithMany() // if TaskModel doesn't need to know its BidTasks
            .HasForeignKey(t => t.BidTaskModelTaskModelId)
            .OnDelete(DeleteBehavior.Restrict); // Or Cascade if you want them deleted with the task template

        base.OnModelCreating(modelBuilder);
    }

    #region DbSets

    /// <summary>
    /// Table for registered users.
    /// </summary>
    public DbSet<UserModel> Users { get; set; } = default!;

    /// <summary>
    /// Table for individual items.
    /// </summary>
    public DbSet<ItemModel> Items { get; set; } = default!;

    /// <summary>
    /// Table for item type categories.
    /// </summary>
    public DbSet<ItemTypeModel> ItemTypes { get; set; } = default!;

    /// <summary>
    /// Table for Client categories.
    /// </summary>
    public DbSet<ClientModel> Clients { get; set; } = default!;

    /// <summary>
    /// Table for Bid categories.
    /// </summary>
    public DbSet<BidModel> BidInfo { get; set; } = default!;

    /// <summary>
    /// Table for Bid categories.
    /// </summary>
    public DbSet<BidModel> BidSiteInfo { get; set; } = default!;

    /// <summary>
    /// Table for Bidtask categories.
    /// </summary>
    public DbSet<BidTaskModel> BidTasks { get; set; } = default!;

    /// <summary>
    /// Table for taskModel categories.
    /// </summary>
    public DbSet<TaskModel> TaskTemplates { get; set; } = default!;

    /// <summary>
    /// Table for BidComponentLineItems categories.
    /// </summary>
    public DbSet<BidComponentLineItemModel> BidComponents { get; set; } = default!;




    #endregion    
}
