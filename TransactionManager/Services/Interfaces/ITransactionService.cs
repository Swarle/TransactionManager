using TransactionManager.Dto;

namespace TransactionManager.Services.Interfaces;

public interface ITransactionService
{
    Task UpsertAsync(IFormFile file, CancellationToken cancellationToken = default);
    Task<byte[]> ExportTransactionsAsync(TransactionDateRangeDto? exportTransactionDto, CancellationToken cancellationToken = default);

    Task<List<UserTimezoneTransactionDto>> GetTransactionsForUserTimezoneAsync(
        TransactionDateRangeDto transactionDateRangeDto,
        CancellationToken cancellationToken = default);

    Task<List<ClientTimezoneTransactionDto>> GetTransactionsForClientTimezoneAsync(
        TransactionDateRangeDto transactionDateRangeDto,
        CancellationToken cancellationToken = default);
    Task<List<ClientTimezoneTransactionDto>> GetTransactionsForClientTimezoneAsync(
        TransactionByDateDto transactionByDateDto,
        CancellationToken cancellationToken = default);
}