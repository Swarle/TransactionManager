using TransactionManager.Entities;

namespace TransactionManager.DataAccess.Interfaces;

public interface ITransactionDataAccess
{
    Task UpsertTransactionsAsync(List<Transaction> transactions, CancellationToken cancellationToken);
}