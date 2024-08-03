namespace TransactionManager.Services.Interfaces;

public interface ITransactionService
{
    Task UpsertAsync(IFormFile file, CancellationToken cancellationToken);
}