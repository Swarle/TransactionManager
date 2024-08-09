using System.Net;
using TransactionManager.Exceptions;
using TransactionManager.Helpers;
using TransactionManager.StaticConstants;

namespace TransactionManager.Extensions;
/// <summary>
/// Provides extension methods to <see cref="HttpRequest"/>
/// </summary>
public static class HttpRequestExtensions
{
    /// <summary>
    /// Extracts the user's timezone from the request header.
    /// The header key is defined in the <see cref="SD"/> class 
    /// under the <see cref="SD.UserTimezoneHeaderKey"/> property.
    /// </summary>
    /// <param name="request">The HTTP request containing the header.</param>
    /// <returns>
    /// A string representing the user's timezone extracted from the header.
    /// </returns>
    /// <exception cref="HttpException">
    /// Thrown with a status code of <see cref="HttpStatusCode.BadRequest"/> 
    /// if the request does not contain the header or if the timezone is not in IANA format.
    /// </exception>
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