namespace TransactionManager.Dto;

public class TransactionByDateDto
{
    public int Year { get; set; }
    public int? Month { get; set; } = null;
    public int? Day { get; set; } = null;
}