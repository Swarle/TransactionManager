using TimeZoneConverter;

namespace TransactionManager.Helpers;

public static class TimeHelper
{
    public static DateTime ConvertToUtc(string date, string timezoneId)
    {
        var dateTime = DateTime.Parse(date);
        
        var timezoneInfo = TZConvert.GetTimeZoneInfo(timezoneId);

        return TimeZoneInfo.ConvertTimeToUtc(dateTime, timezoneInfo);
    }

    public static DateTime ConvertToLocal(DateTime date, string timezoneId)
    {
        var timezoneInfo = TZConvert.GetTimeZoneInfo(timezoneId);

        return TimeZoneInfo.ConvertTimeFromUtc(date, timezoneInfo);
    }

    public static bool IsTimezoneIANA(string timezoneId)
    {
        var ianaTimezoneNames = TZConvert.KnownIanaTimeZoneNames;

        return ianaTimezoneNames.Contains(timezoneId);
    }
}