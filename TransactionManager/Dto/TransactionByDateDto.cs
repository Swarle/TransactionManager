namespace TransactionManager.Dto;

/// <summary>
/// Represents a Data Transfer Object (DTO) for specifying a date or partial date for querying transactions.
/// </summary>
public class TransactionByDateDto
{
    public int Year { get; set; }
    public int? Month { get; set; } = null;
    public int? Day { get; set; } = null;
}