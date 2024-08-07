namespace TransactionManager.Dto;

public class ClientTimezoneTransactionDto : TransactionDto
{
    public DateTime TransactionDateInClientTimezone { get; set; }
}