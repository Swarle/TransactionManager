using Microsoft.AspNetCore.Mvc;
using TransactionManager.Dto;
using TransactionManager.Services.Interfaces;
using TransactionManager.StaticConstants;

namespace TransactionManager.Controllers;

/// <summary>
/// A controller for managing transactions
/// </summary>
/// <remarks>
/// Controller methods include:
/// loading transactions in a csv file,
/// exporting transactions in .xlxs format,
/// receiving transactions for a specific date or in a date range
/// </remarks>
[ApiController]
[Route("api/[controller]")]
public class TransactionController : ControllerBase
{
    private readonly ITransactionService _transactionService;

    public TransactionController(ITransactionService transactionService)
    {
        _transactionService = transactionService;
    }
    
    /// <summary>
    /// Accepts a CSV file and processes transactions       
    /// </summary>
    /// <remarks>
    /// The method accepts a CSV type file,
    /// parses it and retrieves information about transactions from it,
    /// then it stores transactions in the database  
    /// </remarks>
    /// <param name="file">An CSV file with transactions</param>
    /// <param name="cancellationToken">The cancelation token.</param>
    /// <returns>
    /// A <see cref="ActionResult"/> that produces empty Status200OK result
    /// </returns>
    /// <response code="200">CSV file was successfuly uploaded.</response>
    /// <response code="200">Problems with parsing CSV file.</response>
    [HttpPost("upsert")]
    [Consumes("multipart/form-data")]
    [Produces("application/json")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult> UpsertTransactionsAsync(IFormFile file,
        CancellationToken cancellationToken = default)
    {
        await _transactionService.UpsertAsync(file, cancellationToken);

        return Ok();
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
    /// <param name="transactionDateRangeDto">The date range for the transactions to export. Optional. If not provided, exports all transactions.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>
    /// An Excel file containing the transactions.
    /// The file name will be "transactions.xlsx".
    /// </returns>
    /// <response code="200">
    /// Excel file generated successfully.
    /// The content type will be `application/vnd.openxmlformats-officedocument.spreadsheetml.sheet`
    /// </response>
    /// <response code="400">Invalid date range or timezone provided.</response>
    /// <response code="404">No transactions found for the specified criteria.</response>
    /// <response code="500">Internal server error.</response>
    [HttpPost("export/excel")]
    [Consumes("application/json")]
    [Produces("application/json")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<byte[]>> ExportTransactionsAsync([FromBody] TransactionDateRangeDto? transactionDateRangeDto,
        CancellationToken cancellationToken)
    {
        var stream = await _transactionService.ExportTransactionsAsync(transactionDateRangeDto, cancellationToken);

        return File(stream, SD.ExcelContentType, $"transactions.xlsx");
    }
    
    /// <summary>
    /// Retrieves transactions within the specified date range in the user's timezone.
    /// </summary>
    /// <remarks>
    /// This method retrieves transactions that fall within the specified date range in the timezone of the current user.
    /// Only dates with an "Unspecified" kind are accepted. Dates with an offset or Zooly Time are not valid.
    /// Requires a `User-Timezone` value in IANA format in the header.
    /// </remarks>
    /// <param name="transactionDateRangeDto">The date range for retrieving transactions. Must have an "Unspecified" kind.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>
    /// A list of <see cref="UserTimezoneTransactionDto"/>> that fall within the specified date range in the user's timezone.
    /// </returns>
    /// <response code="200">Successfully retrieved transactions within the specified date range.</response>
    /// <response code="400">Invalid date range or timezone provided.</response>
    /// <response code="404">No transactions found for the specified date range.</response>
    /// <response code="500">Internal server error.</response>
    [HttpPost("get-all/for-user-timezone")]
    [Consumes("application/json")]
    [Produces("application/json")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<UserTimezoneTransactionDto>))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<List<UserTimezoneTransactionDto>>> GetTransactionsForUserTimezoneAsync(
        [FromBody] TransactionDateRangeDto transactionDateRangeDto,
        CancellationToken cancellationToken)
    {
        var transactions =
            await _transactionService.GetTransactionsForUserTimezoneAsync(transactionDateRangeDto, cancellationToken);

        return Ok(transactions);
    }

    /// <summary>
    /// Retrieves transactions within the specified date range in the timezone of the transactions.
    /// </summary>
    /// <remarks>
    /// This method retrieves transactions that occurred within the specified date range, using the timezones of the transactions themselves.
    /// Only dates with an "Unspecified" kind are accepted. Dates with an offset or Zooly Time are not valid.
    /// </remarks>
    /// <param name="transactionDateRangeDto">The date range for retrieving transactions. Must have an "Unspecified" kind.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>
    /// A list of <see cref="ClientTimezoneTransactionDto"/>> that fall within the specified date range, based on the transaction timezones.
    /// </returns>
    /// <response code="200">Successfully retrieved transactions within the specified date range.</response>
    /// <response code="400">Invalid date range provided (e.g., dates with an offset).</response>
    /// <response code="404">No transactions found for the specified date range.</response>
    /// <response code="500">Internal server error.</response>
    [HttpPost("get-all/for-client-timezone")]
    [Consumes("application/json")]
    [Produces("application/json")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<ClientTimezoneTransactionDto>))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<List<ClientTimezoneTransactionDto>>> GetTransactionsForClientTimezoneAsync(
        [FromBody] TransactionDateRangeDto transactionDateRangeDto,
        CancellationToken cancellationToken)
    {
        var transactions =
            await _transactionService.GetTransactionsForClientTimezoneAsync(transactionDateRangeDto, cancellationToken);

        return Ok(transactions);
    }

    /// <summary>
    /// Retrieves transactions that occurred on a specific date, based on the timezone of the transactions.
    /// </summary>
    /// <remarks>
    /// This method retrieves transactions that occurred on a specific year, month, and day, using the timezones of the transactions themselves.
    /// </remarks>
    /// <param name="transactionByDateDto">
    /// The date details for retrieving transactions.
    /// The month and day fields are optional.
    /// If either field is omitted, the method will consider all transactions within the specified year (and month, if provided).
    /// </param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>
    /// A list of <see cref="ClientTimezoneTransactionDto"/> that occurred on the specified date, based on the transaction timezones.
    /// </returns>
    /// <response code="200">Successfully retrieved transactions for the specified date.</response>
    /// <response code="400">Invalid date details provided (e.g., month is null while day is specified).</response>
    /// <response code="404">No transactions found for the specified date.</response>
    /// <response code="500">Internal server error.</response>
    [HttpPost("get-all/for-client-timezone-by-date")]
    [Consumes("application/json")]
    [Produces("application/json")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<ClientTimezoneTransactionDto>))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<List<ClientTimezoneTransactionDto>>> GetTransactionsForClientTimezoneAsync(
        [FromBody] TransactionByDateDto transactionByDateDto,
        CancellationToken cancellationToken)
    {
        var transaction =
            await _transactionService.GetTransactionsForClientTimezoneAsync(transactionByDateDto, cancellationToken);

        return Ok(transaction);
    }
}