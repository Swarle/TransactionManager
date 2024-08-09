using System.Globalization;
using GeoTimeZone;
using TransactionManager.Entities;
using TransactionManager.Helpers.Parser.Interfaces;

namespace TransactionManager.Helpers.Parser.Mappers;

/// <summary>
/// Implements <see cref="IEntityCsvMapper{TEntity}"/> to map CSV data to a <see cref="Transaction"/> entity.
/// </summary>
/// <remarks>
/// This class provides the implementation for mapping a dictionary of CSV data to a <see cref="Transaction"/> entity.
/// It parses specific fields from the CSV data, converts them to the appropriate types, and performs additional
/// processing such as extracting location information and converting transaction dates to UTC.
/// </remarks>
public class TransactionCsvMapper : IEntityCsvMapper<Transaction>
{
    private static readonly string[] RequiredColumns =
    [
        "transaction_id", "name", "email", "amount", "transaction_date", "client_location"
    ];
    
    /// <summary>
    /// Maps a dictionary of CSV data to a <see cref="Transaction"/> entity.
    /// </summary>
    /// <param name="csvData">A dictionary where the keys are column names from the CSV file, and the values are the corresponding cell values.</param>
    /// <returns>A <see cref="Transaction"/> instance populated with data from the CSV line.</returns>
    /// <exception cref="FormatException">
    /// Thrown if the data in the CSV does not match the expected format, such as invalid decimal or date formats.
    /// </exception>
    public Transaction MapFromLineToEntity(Dictionary<string, string> csvData)
    {
        var (latitude, longitude) = SeparateLocation(csvData["client_location"]);

        var timezoneId = GetTimezoneFromLocation(latitude, longitude);

        return new Transaction
        {
            TransactionId = csvData["transaction_id"],
            Name = csvData["name"],
            Email = csvData["email"],
            Amount = decimal.Parse(csvData["amount"].Replace("$", ""), CultureInfo.InvariantCulture),
            TransactionDateUtc = TimeHelper.ConvertToUtc(csvData["transaction_date"], timezoneId),
            TransactionTimezone = timezoneId,
            Latitude = latitude,
            Longitude = longitude
        };
    }
    
    /// <summary>
    /// Validates the structure of the CSV headers by checking if they match the required columns.
    /// </summary>
    /// <param name="headers">An array of <see cref="string"/> representing the CSV headers.</param>
    /// <returns><c>true</c> if the headers match the required columns; otherwise, <c>false</c>.</returns>
    public bool ValidateCsvStructure(string[] headers)
    {
        return RequiredColumns.SequenceEqual(headers);
    }

    /// <summary>
    /// Extracts latitude and longitude from a location string.
    /// </summary>
    /// <param name="location">A string containing latitude and longitude separated by a comma.</param>
    /// <returns>A tuple containing the latitude and longitude as <see cref="double"/>.</returns>
    /// <exception cref="FormatException">
    /// Thrown if the location string cannot be parsed into two valid doubles.
    /// </exception>
    private static (double latitude, double longitude) SeparateLocation(string location)
    {
        var locationSplit = location.Split(',');

        var latitude = double.Parse(locationSplit[0], CultureInfo.InvariantCulture);
        var longitude = double.Parse(locationSplit[1], CultureInfo.InvariantCulture);

        return (latitude, longitude);
    }

    /// <summary>
    /// Retrieves the time zone ID based on latitude and longitude.
    /// </summary>
    /// <param name="latitude">The latitude of the location.</param>
    /// <param name="longitude">The longitude of the location.</param>
    /// <returns>The time zone ID as a string.</returns>
    private static string GetTimezoneFromLocation(double latitude, double longitude)
    {
        return TimeZoneLookup.GetTimeZone(latitude, longitude).Result;
    }
}
