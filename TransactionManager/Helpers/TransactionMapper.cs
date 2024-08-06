using TransactionManager.Dto;
using TransactionManager.Entities;

namespace TransactionManager.Helpers;

public static class TransactionMapper
{
    public static TransactionDto MapTransactionToTransactionDto(Transaction transaction) =>
        new TransactionDto
        {
            TransactionId = transaction.TransactionId,
            Name = transaction.Name,
            Email = transaction.Email,
            Amount = transaction.Amount,
            TransactionDateUtc = transaction.TransactionDateUtc,
            TransactionTimezone = transaction.TransactionTimezone,
            Latitude = transaction.Latitude,
            Longitude = transaction.Longitude
        };

    public static List<TransactionDto> MapTransactionsToTransactionsDto(List<Transaction> transactions)
    {
        var transactionsDto = transactions
            .Select(MapTransactionToTransactionDto)
            .ToList();

        return transactionsDto;
    }
}