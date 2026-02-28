using Microsoft.EntityFrameworkCore;

namespace Argus.Argus.Application.Interfaces;

public interface IArgusDbContext : IAsyncDisposable
{
    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    public int SaveChanges();

    public Task BeginTransactionAsync(CancellationToken cancellationToken = default);
    public void BeginTransaction();

    public Task RollbackTransactionAsync(CancellationToken cancellationToken = default);
    public void RollbackTransaction();

    public Task CommitTransactionAsync(CancellationToken cancellationToken = default);
    public void CommitTransaction();
}
