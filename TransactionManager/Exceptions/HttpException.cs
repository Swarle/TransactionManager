using System.Net;
using TransactionManager.StaticConstants;

namespace TransactionManager.Exceptions;

/// <summary>
/// Represents an error that occurred while processing the request
/// (e.g BadRequest, NotFound, Unauthorized)
/// </summary>
/// <remarks>
/// Accepts an error code, an errored string, or a dictionary <see cref="Dictionary{TKey, TValue}"/>
/// also takes an object type
/// </remarks>
public class HttpException : Exception
{
    public HttpException(HttpStatusCode statusCode)
    {
        StatusCode = statusCode;
    }
    public HttpException(HttpStatusCode statusCode, Dictionary<string, string> errorMessages,
        object? result = null)
    {
        StatusCode = statusCode;
        ErrorMessages = errorMessages;
        Result = result;
    }
    public HttpException(HttpStatusCode statusCode, string errorMessage,
        object? result = null)
    {
        StatusCode = statusCode;
        ErrorMessages.Add(SD.DefaultErrorKey, errorMessage);
        Result = result;
    }
    
    public HttpStatusCode StatusCode { get; set; }
    public Dictionary<string, string> ErrorMessages { get; set; } = [];
    public object? Result { get; set; }
}