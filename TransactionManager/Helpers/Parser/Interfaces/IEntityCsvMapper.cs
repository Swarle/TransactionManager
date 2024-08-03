namespace TransactionManager.Helpers.Parser.Interfaces;

public interface IEntityCsvMapper<out TEntity>
{
    TEntity MapFromLineToEntity(Dictionary<string, string> csvData);
}