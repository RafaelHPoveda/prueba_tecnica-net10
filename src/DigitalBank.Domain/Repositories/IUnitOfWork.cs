namespace DigitalBank.Domain.Repositories;

// Garantiza la atomicidad de operaciones multi-entidad (por ejemplo: descontar saldo
// de BankAccount y registrar el registro histórico en TransactionBank bajo la misma transacción).
public interface IUnitOfWork : IDisposable
{
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    Task BeginTransactionAsync(CancellationToken cancellationToken = default);
    Task CommitTransactionAsync(CancellationToken cancellationToken = default);
    Task RollbackTransactionAsync(CancellationToken cancellationToken = default);
}
