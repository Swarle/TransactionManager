using System.Net;
using TransactionManager.Exceptions;
using TransactionManager.Helpers;
using TransactionManager.StaticConstants;

namespace TransactionManager.Extensions;

public static class HttpRequestExtensions
{
    public static string GetTimezoneFromHeader(this HttpRequest request)
    {
        var userTimezone = request.Headers[SD.UserTimezoneHeaderKey].ToString();

        if (string.IsNullOrWhiteSpace(userTimezone))
            throw new HttpException(HttpStatusCode.BadRequest, $"Does not contain a header {SD.UserTimezoneHeaderKey}");

        if (!TimeHelper.IsTimezoneIANA(userTimezone))
            throw new HttpException(HttpStatusCode.BadRequest, "Timezone must be in IANA format");

        return userTimezone;
    }
}