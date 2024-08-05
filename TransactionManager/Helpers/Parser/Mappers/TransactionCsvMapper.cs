using System.Globalization;
using GeoTimeZone;
using TimeZoneConverter;
using TransactionManager.Entities;
using TransactionManager.Helpers.Parser.Interfaces;

namespace TransactionManager.Helpers.Parser.Mappers;

public class TransactionCsvMapper : IEntityCsvMapper<Transaction>
{
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

    private static string GetTimezoneFromLocation(double latitude, double longitude)
    {
        return TimeZoneLookup.GetTimeZone(latitude, longitude).Result;
    }

    private static (double latitude, double longitude) SeparateLocation(string location)
    {
        var locationSplit = location.Split(',');

        var latitude = double.Parse(locationSplit[0], CultureInfo.InvariantCulture);
        var longitude = double.Parse(locationSplit[1], CultureInfo.InvariantCulture);

        return (latitude, longitude);
    }
}