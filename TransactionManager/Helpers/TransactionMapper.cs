using TransactionManager.Dto;
using TransactionManager.Entities;
using TransactionManager.Entities.ModifiedEntities;

namespace TransactionManager.Helpers;

public static class TransactionMapper
{
    public static TransactionDto MapTransactionToDto(Transaction transaction) =>
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

    public static UserTimezoneTransactionDto MapTransactionToDto(
        UserTimezoneTransaction transaction) =>
        new UserTimezoneTransactionDto
        {
            TransactionId = transaction.TransactionId,
            Name = transaction.Name,
            Email = transaction.Email,
            Amount = transaction.Amount,
            TransactionDateUtc = transaction.TransactionDateUtc,
            TransactionDateInUserTimezone = transaction.TransactionDateInUserTimezone,
            TransactionTimezone = transaction.TransactionTimezone,
            Latitude = transaction.Latitude,
            Longitude = transaction.Longitude
        };

    public static ClientTimezoneTransactionDto MapTransactionToDto(
        ClientTimezoneTransaction transaction) =>
        new ClientTimezoneTransactionDto
        {
            TransactionId = transaction.TransactionId,
            Name = transaction.Name,
            Email = transaction.Email,
            Amount = transaction.Amount,
            TransactionDateUtc = transaction.TransactionDateUtc,
            TransactionDateInClientTimezone = transaction.TransactionDateInClientTimezone,
            TransactionTimezone = transaction.TransactionTimezone,
            Latitude = transaction.Latitude,
            Longitude = transaction.Longitude
        };

    public static List<TransactionDto> MapTransactionsToDto(List<Transaction> transactions)
    {
        var transactionsDto = transactions
            .Select(MapTransactionToDto)
            .ToList();

        return transactionsDto;
    }
    
    public static List<UserTimezoneTransactionDto> MapTransactionsToDto(List<UserTimezoneTransaction> transactions)
    {
        var transactionsDto = transactions
            .Select(MapTransactionToDto)
            .ToList();

        return transactionsDto;
    }
    
    public static List<ClientTimezoneTransactionDto> MapTransactionsToDto(List<ClientTimezoneTransaction> transactions)
    {
        var transactionsDto = transactions
            .Select(MapTransactionToDto)
            .ToList();

        return transactionsDto;
    }
}