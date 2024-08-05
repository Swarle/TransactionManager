using TransactionManager.Entities;

namespace TransactionManager.DataAccess.Interfaces;

public interface ITransactionDataAccess
{
    Task UpsertTransactionsAsync(List<Transaction> transactions, CancellationToken cancellationToken);
    Task<List<Transaction>> GetAllTransactionsAsync(CancellationToken cancellationToken);
    Task<List<Transaction>> GetAllTransactionsAsync(DateTime startDate, DateTime endDate, string timezoneId, CancellationToken cancellationToken);
    Task<List<Transaction>> GetAllTransactionsAsync(DateTime startDate, DateTime endDate, CancellationToken cancellationToken);
}