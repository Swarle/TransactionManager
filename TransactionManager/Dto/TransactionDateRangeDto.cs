namespace TransactionManager.Dto;

/// <summary>
/// Represents a Data Transfer Object (DTO) for defining a range of transaction dates.
/// </summary>
public class TransactionDateRangeDto
{
    public DateTime StartDate { get; set; }
    
    public DateTime EndDate { get; set; }
}