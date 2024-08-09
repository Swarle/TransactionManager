namespace TransactionManager.Entities.ModifiedEntities;

/// <summary>
/// Represents a transaction entity with additional information for the client's timezone.
/// This class is used to hold the results returned from queries that include client-specific timezone conversions.
/// </summary>
public class ClientTimezoneTransaction
{
    public string TransactionId { get; set; } = null!;
    public string Name { get; set; } = null!;
    public string Email { get; set; } = null!;
    public decimal Amount { get; set; }
    public DateTime TransactionDateUtc { get; set; }
    public DateTime TransactionDateInClientTimezone { get; set; }
    public string TransactionTimezone { get; set; } = null!;
    public double Latitude { get; set; }
    public double Longitude { get; set; }
}