using System.Linq.Expressions;

namespace DigitalBank.Domain.Repositories;

// Contrato genérico para desacoplar el acceso a datos.
// Provee las operaciones fundamentales (CRUD) requeridas por la capa de aplicación.
public interface IBaseRepository<T> where T : class
{
    Task<T?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<T>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<T>> FindAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default);
    Task AddAsync(T entity, CancellationToken cancellationToken = default);
    void Update(T entity);
    void Delete(T entity);
}
