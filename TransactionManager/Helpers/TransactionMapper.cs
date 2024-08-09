using TransactionManager.Dto;
using TransactionManager.Entities;
using TransactionManager.Entities.ModifiedEntities;

namespace TransactionManager.Helpers;

/// <summary>
/// Static class that provides mapping methods for converting transaction entities to their corresponding DTOs.
/// </summary>
public static class TransactionMapper
{
    /// <summary>
    /// Maps a <see cref="Transaction"/> entity to a <see cref="TransactionDto"/>.
    /// </summary>
    /// <param name="transaction">The transaction entity to map.</param>
    /// <returns>A <see cref="TransactionDto"/> containing the mapped data.</returns>
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

    /// <summary>
    /// Maps a <see cref="UserTimezoneTransaction"/> entity to a <see cref="UserTimezoneTransactionDto"/>.
    /// </summary>
    /// <param name="transaction">The user timezone transaction entity to map.</param>
    /// <returns>A <see cref="UserTimezoneTransactionDto"/> containing the mapped data.</returns>
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

    /// <summary>
    /// Maps a <see cref="ClientTimezoneTransaction"/> entity to a <see cref="ClientTimezoneTransactionDto"/>.
    /// </summary>
    /// <param name="transaction">The client timezone transaction entity to map.</param>
    /// <returns>A <see cref="ClientTimezoneTransactionDto"/> containing the mapped data.</returns>
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

    /// <summary>
    /// Maps a list of <see cref="Transaction"/> entities to a list of <see cref="TransactionDto"/>s.
    /// </summary>
    /// <param name="transactions">The list of transaction entities to map.</param>
    /// <returns>A list of <see cref="TransactionDto"/>s containing the mapped data.</returns>
    public static List<TransactionDto> MapTransactionsToDto(List<Transaction> transactions)
    {
        var transactionsDto = transactions
            .Select(MapTransactionToDto)
            .ToList();

        return transactionsDto;
    }
    
    /// <summary>
    /// Maps a list of <see cref="UserTimezoneTransaction"/> entities to a list of <see cref="UserTimezoneTransactionDto"/>s.
    /// </summary>
    /// <param name="transactions">The list of user timezone transaction entities to map.</param>
    /// <returns>A list of <see cref="UserTimezoneTransactionDto"/>s containing the mapped data.</returns>
    public static List<UserTimezoneTransactionDto> MapTransactionsToDto(List<UserTimezoneTransaction> transactions)
    {
        var transactionsDto = transactions
            .Select(MapTransactionToDto)
            .ToList();

        return transactionsDto;
    }
    
    /// <summary>
    /// Maps a list of <see cref="ClientTimezoneTransaction"/> entities to a list of <see cref="ClientTimezoneTransactionDto"/>s.
    /// </summary>
    /// <param name="transactions">The list of client timezone transaction entities to map.</param>
    /// <returns>A list of <see cref="ClientTimezoneTransactionDto"/>s containing the mapped data.</returns>
    public static List<ClientTimezoneTransactionDto> MapTransactionsToDto(List<ClientTimezoneTransaction> transactions)
    {
        var transactionsDto = transactions
            .Select(MapTransactionToDto)
            .ToList();

        return transactionsDto;
    }
}
