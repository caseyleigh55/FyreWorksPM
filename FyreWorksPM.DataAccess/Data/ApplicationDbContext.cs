using Microsoft.EntityFrameworkCore;
using FyreWorksPM.DataAccess.Data.Models;

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


    #endregion    
}
