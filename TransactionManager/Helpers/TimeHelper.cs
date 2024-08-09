using TimeZoneConverter;

namespace TransactionManager.Helpers;

/// <summary>
/// Provides helper methods for working with time zones and date/time conversions.
/// </summary>
public static class TimeHelper
{
    /// <summary>
    /// Converts the given date string to UTC time based on the specified time zone ID.
    /// </summary>
    /// <param name="date">The date string to be converted. It should be in a format that <see cref="DateTime.Parse(string)"/> can parse.</param>
    /// <param name="timezoneId">The time zone ID to use for conversion. Should be a valid IANA time zone identifier.</param>
    /// <returns>The UTC <see cref="DateTime"/>  representation of the provided date string.</returns>
    /// <exception cref="ArgumentException">Thrown when the date string is not in a valid format or the time zone ID is invalid.</exception>
    public static DateTime ConvertToUtc(string date, string timezoneId)
    {
        var dateTime = DateTime.Parse(date);
        var timezoneInfo = TZConvert.GetTimeZoneInfo(timezoneId);

        return TimeZoneInfo.ConvertTimeToUtc(dateTime, timezoneInfo);
    }
    
    /// <summary>
    /// Converts the given DateTime to UTC time based on the specified time zone ID.
    /// </summary>
    /// <param name="date">The DateTime to be converted to UTC.</param>
    /// <param name="timezoneId">The time zone ID to use for conversion. Should be a valid IANA time zone identifier.</param>
    /// <returns>The UTC <see cref="DateTime"/>  representation of the provided <see cref="DateTime"/> .</returns>
    /// <exception cref="ArgumentException">Thrown when the time zone ID is invalid.</exception>
    public static DateTime ConvertToUtc(DateTime date, string timezoneId)
    {
        var timezoneInfo = TZConvert.GetTimeZoneInfo(timezoneId);

        return TimeZoneInfo.ConvertTimeToUtc(date, timezoneInfo);
    }

    /// <summary>
    /// Converts the given UTC DateTime to local time based on the specified time zone ID.
    /// </summary>
    /// <param name="date">The UTC DateTime to be converted to local time.</param>
    /// <param name="timezoneId">The time zone ID to use for conversion. Should be a valid IANA time zone identifier.</param>
    /// <returns>The local <see cref="DateTime"/> representation of the provided UTC <see cref="DateTime"/> .</returns>
    /// <exception cref="ArgumentException">Thrown when the time zone ID is invalid.</exception>
    public static DateTime ConvertToLocal(DateTime date, string timezoneId)
    {
        var timezoneInfo = TZConvert.GetTimeZoneInfo(timezoneId);

        return TimeZoneInfo.ConvertTimeFromUtc(date, timezoneInfo);
    }

    /// <summary>
    /// Determines whether the specified time zone ID is a valid IANA time zone identifier.
    /// </summary>
    /// <param name="timezoneId">The time zone ID to check.</param>
    /// <returns><c>true</c> if the time zone ID is a valid IANA time zone identifier; otherwise, <c>false</c>.</returns>
    public static bool IsTimezoneIANA(string timezoneId)
    {
        var ianaTimezoneNames = TZConvert.KnownIanaTimeZoneNames;

        return ianaTimezoneNames.Contains(timezoneId);
    }
}
