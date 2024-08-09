namespace TransactionManager.Dto;

/// <summary>
/// Represents a Data Transfer Object (DTO) for a transaction, including
/// information about the transaction date in the client's timezone.
/// Inherits from <see cref="TransactionDto"/>.
/// </summary>
public class ClientTimezoneTransactionDto : TransactionDto
{
    public DateTime TransactionDateInClientTimezone { get; set; }
}