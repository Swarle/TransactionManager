namespace TransactionManager.Entities;

/// <summary>
/// Represents a transaction entity in the system.
/// This class is mapped to a database table and contains details about individual transactions.
/// </summary>
public class Transaction
{
    public string TransactionId { get; set; } = null!;
    public string Name { get; set; } = null!;
    public string Email { get; set; } = null!;
    public decimal Amount { get; set; }
    public DateTime TransactionDateUtc { get; set; }
    public string TransactionTimezone { get; set; } = null!;
    public double Latitude { get; set; }
    public double Longitude { get; set; }
}