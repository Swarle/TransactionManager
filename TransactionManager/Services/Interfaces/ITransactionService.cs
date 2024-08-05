using TransactionManager.Dto;

namespace TransactionManager.Services.Interfaces;

public interface ITransactionService
{
    Task UpsertAsync(IFormFile file, CancellationToken cancellationToken);
    Task<byte[]> ExportTransactionsAsync(ExportTransactionDto? exportTransactionDto, CancellationToken cancellationToken);
}