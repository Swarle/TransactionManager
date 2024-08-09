using TransactionManager.Dto;

namespace TransactionManager.Services.Interfaces;

/// <summary>
/// Defines the contract for a service that handles business logic related to transactions.
/// </summary>
public interface ITransactionService
{
    /// <summary>
    /// Inserts or updates transactions based on the data provided in the <see cref="IFormFile"/> file.
    /// </summary>
    /// <param name="file">The <see cref="IFormFile"/> file containing transaction data.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    Task UpsertAsync(IFormFile file, CancellationToken cancellationToken = default);

    /// <summary>
    /// Exports transactions to an Excel file based on the specified date range.
    /// </summary>
    /// <param name="exportTransactionDto">
    /// The date range for exporting transactions. If null, all transactions are exported.
    /// </param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>
    /// A <see cref="Task{TResult}"/> representing the asynchronous operation, 
    /// with the result being a byte array containing the Excel file data.
    /// </returns>
    Task<byte[]> ExportTransactionsAsync(TransactionDateRangeDto? exportTransactionDto,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves a list of transactions within a specified date range in the user's timezone.
    /// </summary>
    /// <param name="transactionDateRangeDto">
    /// The date range in the user's timezone for which to retrieve transactions.
    /// </param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>
    /// A <see cref="Task{TResult}"/> representing the asynchronous operation,
    /// with the result being a list of <see cref="UserTimezoneTransactionDto"/> objects.
    /// </returns>
    Task<List<UserTimezoneTransactionDto>> GetTransactionsForUserTimezoneAsync(
        TransactionDateRangeDto transactionDateRangeDto,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves a list of transactions within a specified date range in the transaction timezone.
    /// </summary>
    /// <param name="transactionDateRangeDto">
    /// The date range in the transaction timezone for which to retrieve transactions.
    /// </param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>
    /// A <see cref="Task{TResult}"/> representing the asynchronous operation,
    /// with the result being a list of <see cref="ClientTimezoneTransactionDto"/> objects.
    /// </returns>
    Task<List<ClientTimezoneTransactionDto>> GetTransactionsForClientTimezoneAsync(
        TransactionDateRangeDto transactionDateRangeDto,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves a list of transactions that occurred on a specific date in the transaction timezone.
    /// </summary>
    /// <param name="transactionByDateDto">
    /// The date for which to retrieve transactions, specified in the transaction timezone.
    /// </param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>
    /// A <see cref="Task{TResult}"/> representing the asynchronous operation,
    /// with the result being a list of <see cref="ClientTimezoneTransactionDto"/> objects.
    /// </returns>
    Task<List<ClientTimezoneTransactionDto>> GetTransactionsForClientTimezoneAsync(
        TransactionByDateDto transactionByDateDto,
        CancellationToken cancellationToken = default);
}