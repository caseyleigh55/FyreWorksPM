using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Json;

using System.IO;


namespace FyreWorksPM.DataAccess.Data;

/// <summary>
/// Used by EF Core CLI tools at design-time to create a DbContext instance.
/// Required for tasks like adding migrations and updating the database.
/// </summary>
public class ApplicationDbContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
{
    /// <summary>
    /// Creates a configured instance of the ApplicationDbContext using a hardcoded connection string.
    /// This should match the connection string used in MauiProgram.cs.
    /// </summary>
    /// <param name="args">CLI arguments passed by EF tools (unused here).</param>
    public ApplicationDbContext CreateDbContext(string[] args)
    {
        // Try current dir, fallback to API project dir
        string basePath = Directory.GetCurrentDirectory();
        if (!File.Exists(Path.Combine(basePath, "appsettings.json")))
        {
            basePath = Path.Combine(basePath, "../FyreWorksPM.Api");
        }

        var config = new ConfigurationBuilder()
            .SetBasePath(basePath)
            .AddJsonFile("appsettings.json", optional: false)
            .Build();

        var connectionString = config.GetConnectionString("DefaultConnection");

        var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
        optionsBuilder.UseSqlServer(connectionString);

        return new ApplicationDbContext(optionsBuilder.Options);
    }
}
