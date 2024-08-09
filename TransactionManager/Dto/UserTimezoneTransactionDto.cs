namespace TransactionManager.Dto;

/// <summary>
/// Represents a Data Transfer Object (DTO) for a transaction, including
/// information about the transaction date in the user's timezone.
/// Inherits from <see cref="TransactionDto"/>.
/// </summary>
public class UserTimezoneTransactionDto : TransactionDto
{
    public DateTime TransactionDateInUserTimezone { get; set; }
}