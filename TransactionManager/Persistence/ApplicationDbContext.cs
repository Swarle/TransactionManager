using System.Reflection;
using Microsoft.EntityFrameworkCore;
using TransactionManager.Entities;

namespace TransactionManager.Persistence;

/// <summary>
/// Represents the application's database context that manages entity objects during runtime.
/// </summary>
public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions options) : base(options)
    {
    }

    public DbSet<Transaction> Transactions { get; set; }


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Apply all configurations from the assembly that contains the current context. This includes entity configurations.
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetAssembly(typeof(ApplicationDbContext))!);
    }
}