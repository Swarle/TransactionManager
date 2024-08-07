namespace TransactionManager.Dto;

public class UserTimezoneTransactionDto : TransactionDto
{
    public DateTime TransactionDateInUserTimezone { get; set; }
}