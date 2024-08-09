namespace TransactionManager.Helpers.Parser.Interfaces;

/// <summary>
/// Defines a contract for mapping CSV data to an entity type.
/// </summary>
/// <typeparam name="TEntity">The type of the entity to be created from CSV data.</typeparam>
/// <remarks>
/// This interface is used to define how a line of CSV data should be mapped to an instance of an entity.
/// Implementations of this interface should handle the conversion from a dictionary of CSV column names and values
/// to an entity of type <typeparamref name="TEntity"/>.
/// </remarks>
public interface IEntityCsvMapper<out TEntity>
{
    /// <summary>
    /// Maps a dictionary of CSV data to an entity of type <typeparamref name="TEntity"/>.
    /// </summary>
    /// <param name="csvData">A dictionary where the keys are column names from the CSV file, and the values are the corresponding cell values.</param>
    /// <returns>An instance of <typeparamref name="TEntity"/> populated with data from the CSV line.</returns>
    TEntity MapFromLineToEntity(Dictionary<string, string> csvData);
    
    /// <summary>
    /// Validates the structure of the CSV headers.
    /// </summary>
    /// <param name="headers">A array of <see cref="string"/> representing the CSV headers.</param>
    /// <returns><c>true</c> if the structure is valid, otherwise <c>false</c>.</returns>
    bool ValidateCsvStructure(string[] headers);
}