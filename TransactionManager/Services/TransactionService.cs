using System.Net;
using TransactionManager.DataAccess.Interfaces;
using TransactionManager.Dto;
using TransactionManager.Entities;
using TransactionManager.Exceptions;
using TransactionManager.Extensions;
using TransactionManager.Helpers;
using TransactionManager.Helpers.Parser;
using TransactionManager.Helpers.Parser.Mappers;
using TransactionManager.Services.Interfaces;

namespace TransactionManager.Services;

/// <summary>
/// Provides business logic for handling transactions, including importing, exporting,
/// and retrieving transactions in different time zones.
/// </summary>
public class TransactionService : ITransactionService
{
    private readonly ITransactionDataAccess _transactionDataAccess;
    private readonly HttpContext _httpContext;

    /// <summary>
    /// Initializes a new instance of the <see cref="TransactionService"/> class.
    /// </summary>
    /// <param name="transactionDataAccess">The data access layer for transactions.</param>
    /// <param name="accessor">The HTTP context accessor used to retrieve the current HTTP context.</param>
    /// <exception cref="NullReferenceException">
    /// Thrown if the <paramref name="accessor"/> does not provide a valid HTTP context.
    /// </exception>
    public TransactionService(ITransactionDataAccess transactionDataAccess, IHttpContextAccessor accessor)
    {
        _transactionDataAccess = transactionDataAccess;
        _httpContext = accessor.HttpContext ??
                       throw new NullReferenceException("HttpContextAccessor doesn't have context");
    }

    /// <summary>
    /// Inserts or updates transactions based on the provided CSV file.
    /// </summary>
    /// <param name="file">The CSV file containing transaction data.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    /// <exception cref="HttpException">
    /// Thrown if the CSV file is invalid or the data cannot be processed.
    /// </exception>
    public async Task UpsertAsync(IFormFile file, CancellationToken cancellationToken = default)
    {
        var transactions = await ParseFromCsvToEntityList(file);
        
        await _transactionDataAccess.UpsertTransactionsAsync(transactions, cancellationToken);
    }

    /// <summary>
    /// Exports transactions to an Excel file based on the specified date range.
    /// </summary>
    /// <remarks>
    /// This method supports three modes of operation:
    /// 1. If no date range is provided, it exports all existing transactions to an Excel file.
    /// 2. If a date range is provided in the user's timezone, it limits transactions to those within the specified range.
    ///    The `User-Timezone` header must be specified in IANA format (e.g., "America/New_York").
    /// 3. If a date range is provided in Zooly Time format, it searches transactions based on UTC time.
    /// </remarks>
    /// <param name="exportTransactionDto">
    /// The date range for exporting transactions. If null, all transactions are exported.
    /// </param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>
    /// A <see cref="Task{TResult}"/> representing the asynchronous operation, 
    /// with the result being a byte array containing the Excel file data.
    /// </returns>
    /// <exception cref="HttpException">
    /// Thrown if no transactions are found within the specified date range.
    /// </exception>
    public async Task<byte[]> ExportTransactionsAsync(TransactionDateRangeDto? exportTransactionDto,
        CancellationToken cancellationToken = default)
    {
        List<Transaction> transactions;

        if (exportTransactionDto is null)
            transactions = await _transactionDataAccess.GetAllTransactionsAsync(cancellationToken);
        else
            transactions = await GetTransactionsByDateAsync(exportTransactionDto.StartDate,
                exportTransactionDto.EndDate, cancellationToken);

        if (transactions.Count == 0)
            throw new HttpException(HttpStatusCode.NotFound, "No transaction was found");
        
        var excelStream = ExcelConverter.ConvertToExcel(transactions);
        
        return excelStream;
    }

    /// <summary>
    /// Retrieves a list of transactions within a specified date range in the user's timezone.
    /// </summary>
    /// <remarks>
    /// This method retrieves transactions that fall within the specified date range in the timezone of the current user.
    /// Only dates with an "Unspecified" kind are accepted. Dates with "UTC" or "Local" kind are not valid.
    /// Requires a `User-Timezone` value in IANA format in the header of the request.
    /// </remarks>
    /// <param name="transactionDateRangeDto">
    /// The date range in the user's timezone for which to retrieve transactions.
    /// </param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>
    /// A <see cref="Task{TResult}"/> representing the asynchronous operation,
    /// with the result being a list of <see cref="UserTimezoneTransactionDto"/> objects.
    /// </returns>
    /// <exception cref="HttpException">
    /// Thrown if the time kind of the provided dates is UTC with <see cref="HttpStatusCode"/> <see cref="HttpStatusCode.BadRequest"/>
    /// or if no transactions are found with <see cref="HttpStatusCode"/> <see cref="HttpStatusCode.NotFound"/>.
    /// </exception>
    public async Task<List<UserTimezoneTransactionDto>> GetTransactionsForUserTimezoneAsync(
        TransactionDateRangeDto transactionDateRangeDto,
        CancellationToken cancellationToken = default)
    {
        if (transactionDateRangeDto.StartDate.Kind == DateTimeKind.Utc ||
            transactionDateRangeDto.EndDate.Kind == DateTimeKind.Utc)
            throw new HttpException(HttpStatusCode.BadRequest, "Time must be unspecified, not UTC");
        
        var userTimezoneId = _httpContext.Request.GetTimezoneFromHeader();

        var (startDateUtc, endDateUtc) =
            ConvertDateRangeToUtc(
                transactionDateRangeDto.StartDate,
                transactionDateRangeDto.EndDate,
                userTimezoneId);
        
        var transactions =
            await _transactionDataAccess.GetAllTransactionsForUserTimezoneAsync(
                startDateUtc,
                endDateUtc,
                userTimezoneId,
                cancellationToken);

        if (transactions.Count == 0)
            throw new HttpException(HttpStatusCode.NotFound, "No transactions found for this date range");

        var transactionsDto =
            TransactionMapper.MapTransactionsToDto(transactions);

        return transactionsDto;
    }

    /// <summary>
    /// Retrieves a list of transactions within a specified date range in the client's timezone.
    /// </summary>
    /// <remarks>
    /// Only dates with an "Unspecified" kind are accepted. Dates with "UTC" or "Local" kind are not valid.
    /// </remarks>
    /// <param name="transactionDateRangeDto">
    /// The date range in the client's timezone for which to retrieve transactions.
    /// </param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>
    /// A <see cref="Task{TResult}"/> representing the asynchronous operation,
    /// with the result being a list of <see cref="ClientTimezoneTransactionDto"/> objects.
    /// </returns>
    /// <exception cref="HttpException">
    /// Thrown if the time kind of the provided dates is UTC with <see cref="HttpStatusCode"/> <see cref="HttpStatusCode.BadRequest"/>
    /// or if no transactions are found with <see cref="HttpStatusCode"/> <see cref="HttpStatusCode.NotFound"/>.
    /// </exception>
    public async Task<List<ClientTimezoneTransactionDto>> GetTransactionsForClientTimezoneAsync(
        TransactionDateRangeDto transactionDateRangeDto,
        CancellationToken cancellationToken = default)
    {
        if (transactionDateRangeDto.StartDate.Kind == DateTimeKind.Utc ||
            transactionDateRangeDto.EndDate.Kind == DateTimeKind.Utc)
            throw new HttpException(HttpStatusCode.BadRequest, "Time must be unspecified, not UTC");
        
        var transactions =
            await _transactionDataAccess.GetAllTransactionsForClientTimezoneAsync(
                transactionDateRangeDto.StartDate,
                transactionDateRangeDto.EndDate,
                cancellationToken);
        
        if (transactions.Count == 0)
            throw new HttpException(HttpStatusCode.NotFound, "No transactions found for this date range");

        var transactionsDto =
            TransactionMapper.MapTransactionsToDto(transactions);

        return transactionsDto;
    }

    /// <summary>
    /// Retrieves a list of transactions that occurred on a specific date in the client's timezone.
    /// </summary>
    /// <param name="transactionByDateDto">
    /// The date for which to retrieve transactions, specified in the client's timezone.
    /// </param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>
    /// A <see cref="Task{TResult}"/> representing the asynchronous operation,
    /// with the result being a list of <see cref="ClientTimezoneTransactionDto"/> objects.
    /// </returns>
    /// <exception cref="HttpException">
    /// Thrown if no transactions are found with <see cref="HttpStatusCode"/> <see cref="HttpStatusCode.NotFound"/>.
    /// </exception>
    public async Task<List<ClientTimezoneTransactionDto>> GetTransactionsForClientTimezoneAsync(TransactionByDateDto transactionByDateDto,
        CancellationToken cancellationToken = default)
    {
        var transactions =
            await _transactionDataAccess.GetAllTransactionsForClientTimezoneAsync(
                transactionByDateDto.Year,
                transactionByDateDto.Month,
                transactionByDateDto.Day,
                cancellationToken);
        
        if(transactions.Count == 0)
            throw new HttpException(HttpStatusCode.NotFound, "No transactions found for this date");

        var transactionsDto =
            TransactionMapper.MapTransactionsToDto(transactions);

        return transactionsDto;
    }

    /// <summary>
    /// Gets a list of transactions based on the specified date range,
    /// Kind is taken into account.
    /// </summary>
    /// <remarks>
    /// If kind is UTC, the search will be based on UTC,
    /// if Kind is Unspecified, then the search will be based on the user's local time zone
    /// </remarks>
    /// <param name="startDate">The start date of the range.</param>
    /// <param name="endDate">The end date of the range.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>
    /// A <see cref="Task{TResult}"/> representing the asynchronous operation,
    /// with the result being a list of <see cref="Transaction"/> objects.
    /// </returns>
    private async Task<List<Transaction>> GetTransactionsByDateAsync(DateTime startDate, DateTime endDate,
        CancellationToken cancellationToken = default)
    {
        if (startDate.Kind == DateTimeKind.Unspecified &&
            endDate.Kind == DateTimeKind.Unspecified)
        {
            var userTimezoneId = _httpContext.Request.GetTimezoneFromHeader();
            
            (startDate, endDate) =
                ConvertDateRangeToUtc(
                    startDate,
                    endDate,
                    userTimezoneId);
        }
        
        var transactions = await _transactionDataAccess.GetAllTransactionsAsync(startDate, endDate, cancellationToken);
        
        return transactions;
    }
    
    /// <summary>
    /// Converts a range of dates from a specified timezone to UTC timezone.
    /// </summary>
    /// <param name="firstDate">The first date in the specified timezone.</param>
    /// <param name="secondDate">The second date in the specified timezone.</param>
    /// <param name="timezoneId">The timezone ID of the input dates, specified in IANA format.</param>
    /// <returns>A tuple containing the converted first and second dates in UTC timezone.</returns>
    private static (DateTime firstDate, DateTime secondDate) ConvertDateRangeToUtc(
        DateTime firstDate,
        DateTime secondDate,
        string timezoneId)
    {
        var firstDateUtc = TimeHelper.ConvertToUtc(firstDate, timezoneId);
        var secondDateUtc = TimeHelper.ConvertToUtc(secondDate, timezoneId);

        return (firstDateUtc, secondDateUtc);
    }

    /// <summary>
    /// Parses the provided CSV file into a list of <see cref="Transaction"/> entities.
    /// </summary>
    /// <param name="file">The CSV file containing transaction data.</param>
    /// <returns>
    /// A <see cref="Task{TResult}"/> representing the asynchronous operation,
    /// with the result being a list of <see cref="Transaction"/> objects.
    /// </returns>
    /// <exception cref="HttpException">
    /// Thrown if the CSV file is invalid or the data cannot be processed.
    /// </exception>
    private static async Task<List<Transaction>> ParseFromCsvToEntityList(IFormFile file)
    {
        try
        {
            var transactionCsvMapper = new TransactionCsvMapper();
            var transactions = await CsvParser.ParseCsvToEntities(file, transactionCsvMapper);

            return transactions;
        }
        catch (InvalidDataException ex)
        {
            throw new HttpException(HttpStatusCode.BadRequest, ex.Message);
        }
    }
}
