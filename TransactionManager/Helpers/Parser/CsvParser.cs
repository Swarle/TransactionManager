using TransactionManager.Helpers.Parser.Interfaces;

namespace TransactionManager.Helpers.Parser;

/// <summary>
/// A static CSV parser that parses from a CSV file into objects
/// </summary>
public static class CsvParser
{
    /// <summary>
    /// The method that parses from a CSV file in <typeparamref name="TEntity"/>
    /// </summary>
    /// <param name="file">An <see cref="IFormFile"/> that represents CSV file.</param>
    /// <param name="mapper">
    /// A mapper object that maps a string from a CSV file to a <typeparamref name="TEntity"/> object
    /// </param>
    /// <typeparam name="TEntity">The object type to which the CSV file is mapped</typeparam>
    /// <returns>A list of <typeparamref name="TEntity"/> objects.</returns>
    /// <exception cref="InvalidDataException">
    /// Throws an exception if the file is empty, not in CSV format,
    /// or if the CSV structure does not match the expected structure.
    /// </exception>
    /// <exception cref="InvalidOperationException">
    /// Throws an exception if the line cannot be read
    /// </exception>
    public static async Task<List<TEntity>> ParseCsvToEntities<TEntity>(IFormFile file, IEntityCsvMapper<TEntity> mapper)
        where TEntity : new()
    {
        if (file.Length == 0)
            throw new InvalidDataException("File is empty of not provided");

        var extension = Path.GetExtension(file.FileName);
        
        if (extension != ".csv")
            throw new InvalidDataException("File is not a CSV");

        using var stream = new StreamReader(file.OpenReadStream());
        
        var headers = (await stream.ReadLineAsync())!.Split(',');

        if (!mapper.ValidateCsvStructure(headers))
            throw new InvalidDataException(
                "The provided CSV file does not match the specified structure in the mapper");
        
        var result = new List<TEntity>();
        
        while (!stream.EndOfStream)
        {
            var line = await stream.ReadLineAsync() ?? 
                       throw new InvalidOperationException("Unable to read line");
            
            var values = ParseCsvLine(line);

            var csvData = new Dictionary<string, string>();
            
            for (int i = 0; i < headers.Length; i++)
            {
                csvData[headers[i]] = values[i];
            }

            var obj = mapper.MapFromLineToEntity(csvData);
            
            result.Add(obj);
        }

        return result;
    }
    
    
    /// <summary>
    /// Parses a single line of a CSV file into an array of strings, taking into account quoted values.
    /// </summary>
    /// <param name="line">The CSV line to parse.</param>
    /// <returns>
    /// An array of strings, where each element represents a field from the CSV line.
    /// </returns>
    /// <remarks>
    /// This method correctly handles fields enclosed in double quotes, allowing commas within quoted fields.
    /// It also ensures that escaped quotes within a field are treated as literal quotes.
    /// </remarks>
    private static string[] ParseCsvLine(string line)
    {
        var result = new List<string>();
        var current = string.Empty;
        var inQuotes = false;

        for (int i = 0; i < line.Length; i++)
        {
            var c = line[i];

            // Toggle the inQuotes flag if an unescaped double quote is encountered
            if (c == '"' && (i == 0 || line[i - 1] != '\\'))
            {
                inQuotes = !inQuotes;
            }
            // If a comma is found and we are not inside quotes, finalize the current field
            else if (c == ',' && !inQuotes)
            {
                result.Add(current);
                current = string.Empty;
            }
            // Otherwise, add the character to the current field
            else
            {
                current += c;
            }
        }
        
        result.Add(current);

        return result.ToArray();
    }

}