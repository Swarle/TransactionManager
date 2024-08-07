using TransactionManager.Entities;
using TransactionManager.Entities.ModifiedEntities;

namespace TransactionManager.DataAccess.Interfaces;

public interface ITransactionDataAccess
{
    Task UpsertTransactionsAsync(List<Transaction> transactions, CancellationToken cancellationToken = default);
    Task<List<Transaction>> GetAllTransactionsAsync(CancellationToken cancellationToken = default);
    Task<List<Transaction>> GetAllTransactionsAsync(
        DateTime startDate,
        DateTime endDate,
        string timezoneId,
        CancellationToken cancellationToken = default);
    
    Task<List<Transaction>> GetAllTransactionsAsync(
        DateTime startDate,
        DateTime endDate,
        CancellationToken cancellationToken = default);
    
    Task<List<UserTimezoneTransaction>> GetAllTransactionsForUserTimezoneAsync(
        DateTime startDate,
        DateTime endDate,
        string timezoneId,
        CancellationToken cancellationToken = default);
    
    Task<List<ClientTimezoneTransaction>> GetAllTransactionsForClientTimezoneAsync(
        DateTime startDate,
        DateTime endDate,
        CancellationToken cancellationToken = default);
    
    Task<List<ClientTimezoneTransaction>> GetAllTransactionsForClientTimezoneAsync(
        int year,
        int? month = null,
        int? day = null,
        CancellationToken cancellationToken = default);
}