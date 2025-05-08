using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

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
        var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();

        // Must match the connection string in MauiProgram.cs
        optionsBuilder.UseSqlServer(
                        "Server=tcp:fyreworkspmserver.database.windows.net,1433;" +
                        "Initial Catalog=FyreWorksPM;Persist Security Info=False;" +
                        "User ID=ccagle;Password=NakedD!sc0753;" +
                        "MultipleActiveResultSets=False;Encrypt=True;" +
                        "TrustServerCertificate=False;Connection Timeout=30;"
                        );


        return new ApplicationDbContext(optionsBuilder.Options);
    }
}
