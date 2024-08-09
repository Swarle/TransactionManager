using TransactionManager.Entities;
using TransactionManager.Entities.ModifiedEntities;

namespace TransactionManager.DataAccess.Interfaces;

/// <summary>
/// Describes methods of managing transactions in the database
/// </summary>
public interface ITransactionDataAccess
{
    /// <summary>
    /// Asynchronously upsert transactions to the database.
    /// </summary>
    /// <param name="transactions">An list of <see cref="Transaction"/> objects.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>An <see cref="Task"/> that represent asynchronous operation</returns>
    Task UpsertTransactionsAsync(List<Transaction> transactions, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// An asynchronous operation that receives all transactions from the database.
    /// </summary>
    /// <param name="cancellationToken">An cancellation token.</param>
    /// <returns>A list of <see cref="Transaction"/></returns>
    Task<List<Transaction>> GetAllTransactionsAsync(CancellationToken cancellationToken = default);
    
    /// <summary>
    /// An asynchronous operation that retrieves all
    /// transactions from the database between two dates.
    /// </summary>
    /// <param name="startDate">The start date of the range.</param>
    /// <param name="endDate">The end date of the range.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A list of <see cref="Transaction"/> objects.</returns>
    Task<List<Transaction>> GetAllTransactionsAsync(
        DateTime startDate,
        DateTime endDate,
        CancellationToken cancellationToken = default);
    
    /// <summary>
    /// An asynchronous operation that retrieves all transactions from the database
    /// between two dates in the specified timezone.
    /// (timezone specified in <paramref name="timezoneId" /> parameter)
    /// </summary>
    /// <param name="startDate">The start date of the range.</param>
    /// <param name="endDate">The end date of the range.</param>
    /// <param name="timezoneId">The ID of the timezone.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A list of <see cref="UserTimezoneTransaction"/> objects.</returns>
    Task<List<UserTimezoneTransaction>> GetAllTransactionsForUserTimezoneAsync(
        DateTime startDate,
        DateTime endDate,
        string timezoneId,
        CancellationToken cancellationToken = default);
    
    /// <summary>
    /// An asynchronous operation that retrieves
    /// all transactions from the database between two dates
    /// </summary>
    /// <param name="startDate">The start date of the range.</param>
    /// <param name="endDate">The end date of the range.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A list of <see cref="ClientTimezoneTransaction"/> objects.</returns>
    Task<List<ClientTimezoneTransaction>> GetAllTransactionsForClientTimezoneAsync(
        DateTime startDate,
        DateTime endDate,
        CancellationToken cancellationToken = default);
    
    /// <summary>
    /// An asynchronous operation that returns a list of transactions
    /// that occurred on a specific date.
    /// </summary>
    /// <param name="year">The year in which the transaction took place.</param>
    /// <param name="month">The month in which the transaction took place (optional).</param>
    /// <param name="day">The day in which the transaction took place (optional).</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A list of <see cref="ClientTimezoneTransaction"/> objects</returns>
    Task<List<ClientTimezoneTransaction>> GetAllTransactionsForClientTimezoneAsync(
        int year,
        int? month = null,
        int? day = null,
        CancellationToken cancellationToken = default);
}