namespace TransactionManager.Dto;

/// <summary>
/// Represents a Data Transfer Object (DTO) for a transaction
/// </summary>
public class TransactionDto
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