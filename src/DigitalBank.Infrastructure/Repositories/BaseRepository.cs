using System.Linq.Expressions;
using DigitalBank.Domain.Repositories;
using DigitalBank.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace DigitalBank.Infrastructure.Repositories;

// Implementacion base genberica de repositorio para todas las entidades
public class BaseRepository<T> : IBaseRepository<T> where T : class
{
    protected readonly RepositoryContext RepositoryContext;
    protected readonly DbSet<T> DbSet;

    public BaseRepository(RepositoryContext repositoryContext)
    {
        RepositoryContext = repositoryContext;
        DbSet = repositoryContext.Set<T>();
    }

    public virtual async Task<T?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await DbSet.FindAsync([id], cancellationToken);
    }

    public virtual async Task<IReadOnlyList<T>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await DbSet.AsNoTracking().ToListAsync(cancellationToken);
    }

    public virtual async Task<IReadOnlyList<T>> FindAsync(
        Expression<Func<T, bool>> predicate,
        CancellationToken cancellationToken = default)
    {
        return await DbSet.AsNoTracking().Where(predicate).ToListAsync(cancellationToken);
    }

    public virtual async Task AddAsync(T entity, CancellationToken cancellationToken = default)
    {
        await DbSet.AddAsync(entity, cancellationToken);
    }

    public virtual void Update(T entity)
    {
        DbSet.Update(entity);
    }

    public virtual void Delete(T entity)
    {
        DbSet.Remove(entity);
    }
}
