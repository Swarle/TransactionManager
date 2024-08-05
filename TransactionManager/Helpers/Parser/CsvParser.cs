using System.ComponentModel;
using System.Reflection;
using TransactionManager.Helpers.Parser.Interfaces;

namespace TransactionManager.Helpers.Parser;

public static class CsvParser
{
    public static async Task<List<TEntity>> ParseCsvToEntities<TEntity>(IFormFile file, IEntityCsvMapper<TEntity> mapper)
        where TEntity : new()
    {
        if (file.Length == 0)
            throw new InvalidDataException("File is empty of not provided");

        var extension = Path.GetExtension(file.FileName);
        if (extension != ".csv")
            throw new InvalidDataException("File is not a CSV");
        
        var result = new List<TEntity>();

        using var stream = new StreamReader(file.OpenReadStream());
        
        var header = (await stream.ReadLineAsync())!.Split(',');
        
        while (!stream.EndOfStream)
        {
            var line = await stream.ReadLineAsync() ?? 
                       throw new InvalidOperationException("Unable to read line");
            
            var values = ParseCsvLine(line);

            var csvData = new Dictionary<string, string>();
            
            for (int i = 0; i < header.Length; i++)
            {
                csvData[header[i]] = values[i];
            }

            var obj = mapper.MapFromLineToEntity(csvData);
            
            result.Add(obj);
        }

        return result;
    }
    
    private static string[] ParseCsvLine(string line)
    {
        var result = new List<string>();
        var current = string.Empty;
        var inQuotes = false;

        for (int i = 0; i < line.Length; i++)
        {
            var c = line[i];

            if (c == '"' && (i == 0 || line[i - 1] != '\\'))
            {
                inQuotes = !inQuotes;
            }
            else if (c == ',' && !inQuotes)
            {
                result.Add(current);
                current = string.Empty;
            }
            else
            {
                current += c;
            }
        }

        result.Add(current);

        return result.ToArray();
    }
}