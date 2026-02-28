using Argus.Argus.Application.Interfaces;
using Argus.Argus.Domain.Entities;
using Argus.Argus.Infrastructure.Converters;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore.Storage;
using System.Reflection;

namespace Argus.Argus.Infrastructure;

public class ArgusDbContext : DbContext, IArgusDbContext
{
    private IDbContextTransaction? _currentTransaction;

    public ArgusDbContext(DbContextOptions<ArgusDbContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(
            Assembly.GetAssembly(typeof(ArgusDbContext)) ?? Assembly.GetExecutingAssembly());
    }

    protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
    {
        base.ConfigureConventions(configurationBuilder);

        configurationBuilder.Properties<DateOnly>().HaveConversion<DateOnlyConverter>();
        configurationBuilder.Properties<TimeOnly>().HaveConversion<TimeOnlyConverter>();
        configurationBuilder.Properties<string>().HaveMaxLength(256);
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        UpdateTimestamps();
        return await base.SaveChangesAsync(cancellationToken);
    }

    public override int SaveChanges()
    {
        UpdateTimestamps();
        return base.SaveChanges();
    }

    public async Task BeginTransactionAsync(CancellationToken cancellationToken = default)
    {
        _currentTransaction ??= await Database.BeginTransactionAsync(cancellationToken);
    }

    public void BeginTransaction()
    {
        _currentTransaction ??= Database.BeginTransaction();
    }

    public async Task CommitTransactionAsync(CancellationToken cancellationToken = default)
    {
        if (_currentTransaction is null)
            throw new InvalidOperationException("No active transaction to commit.");

        try
        {
            await SaveChangesAsync(cancellationToken);
            await _currentTransaction.CommitAsync(cancellationToken);
        }
        catch
        {
            await RollbackTransactionAsync(cancellationToken);
            throw;
        }
        finally
        {
            await _currentTransaction.DisposeAsync();
            _currentTransaction = null;
        }
    }

    public void CommitTransaction()
    {
        if (_currentTransaction is null)
            throw new InvalidOperationException("No active transaction to commit.");

        try
        {
            SaveChanges();
            _currentTransaction.Commit();
        }
        catch
        {
            RollbackTransaction();
            throw;
        }
        finally
        {
            _currentTransaction.Dispose();
            _currentTransaction = null;
        }
    }

    public async Task RollbackTransactionAsync(CancellationToken cancellationToken = default)
    {
        if (_currentTransaction is null) return;

        await _currentTransaction.RollbackAsync(cancellationToken);
        await _currentTransaction.DisposeAsync();
        _currentTransaction = null;
    }

    public void RollbackTransaction()
    {
        if (_currentTransaction is null) return;

        _currentTransaction.Rollback();
        _currentTransaction.Dispose();
        _currentTransaction = null;
    }

    private const string CreatedFieldName = nameof(EntityBase.CreatedAt);
    private const string UpdatedFieldName = nameof(EntityBase.UpdatedAt);

    private void UpdateTimestamps()
    {
        foreach (var entry in ChangeTracker.Entries()
                     .Where(entry => entry.State is EntityState.Added or EntityState.Modified))
        {
            if (entry.Properties.Any(p => p.Metadata.Name == CreatedFieldName) && entry.State == EntityState.Added)
                entry.Property(CreatedFieldName).CurrentValue = DateTime.UtcNow;

            if (entry.Properties.Any(p => p.Metadata.Name == UpdatedFieldName) &&
                entry.State is EntityState.Added or EntityState.Modified)
                entry.Property(UpdatedFieldName).CurrentValue = DateTime.UtcNow;
        }
    }

    public class ArgusDbContextFactory : IDesignTimeDbContextFactory<ArgusDbContext>
    {
        public ArgusDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<ArgusDbContext>();
            optionsBuilder.UseNpgsql();

            return new ArgusDbContext(optionsBuilder.Options);
        }
    }
}
