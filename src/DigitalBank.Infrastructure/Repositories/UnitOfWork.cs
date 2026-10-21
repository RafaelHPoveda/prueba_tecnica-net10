using DigitalBank.Domain.Repositories;
using DigitalBank.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore.Storage;

namespace DigitalBank.Infrastructure.Repositories;

public class UnitOfWork : IUnitOfWork
{
    private readonly RepositoryContext _repositoryContext;
    private IDbContextTransaction? _transaction;

    public UnitOfWork(RepositoryContext repositoryContext)
    {
        _repositoryContext = repositoryContext;
    }

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await _repositoryContext.SaveChangesAsync(cancellationToken);
    }

    public async Task BeginTransactionAsync(CancellationToken cancellationToken = default)
    {
        _transaction = await _repositoryContext.Database.BeginTransactionAsync(cancellationToken);
    }

    public async Task CommitTransactionAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            await _repositoryContext.SaveChangesAsync(cancellationToken);
            if (_transaction != null)
            {
                await _transaction.CommitAsync(cancellationToken);
            }
        }
        finally
        {
            if (_transaction != null)
            {
                await _transaction.DisposeAsync();
                _transaction = null;
            }
        }
    }

    public async Task RollbackTransactionAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            if (_transaction != null)
            {
                await _transaction.RollbackAsync(cancellationToken);
            }
        }
        finally
        {
            if (_transaction != null)
            {
                await _transaction.DisposeAsync();
                _transaction = null;
            }
        }
    }

    public void Dispose()
    {
        _transaction?.Dispose();
        _repositoryContext.Dispose();
        GC.SuppressFinalize(this);
    }
}
