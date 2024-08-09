namespace TransactionManager.Entities.ModifiedEntities;

/// <summary>
/// Represents a transaction entity with additional information for the user's timezone.
/// This class is used to hold the results returned from queries that include user-specific timezone conversions.
/// </summary>
public class UserTimezoneTransaction
{
    public string TransactionId { get; set; } = null!;
    public string Name { get; set; } = null!;
    public string Email { get; set; } = null!;
    public decimal Amount { get; set; }
    public DateTime TransactionDateUtc { get; set; }
    public DateTime TransactionDateInUserTimezone { get; set; }
    public string TransactionTimezone { get; set; } = null!;
    public double Latitude { get; set; }
    public double Longitude { get; set; }
}