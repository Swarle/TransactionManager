using TransactionManager.Dto;

namespace TransactionManager.Services.Interfaces;

public interface ITransactionService
{
    Task UpsertAsync(IFormFile file, CancellationToken cancellationToken);
    Task<byte[]> ExportTransactionsAsync(TransactionDateRangeDto? exportTransactionDto, CancellationToken cancellationToken);

    Task<List<TransactionDto>> GetTransactionsByUserTimezoneAsync(TransactionDateRangeDto transactionDateRangeDto,
        CancellationToken cancellationToken);
}