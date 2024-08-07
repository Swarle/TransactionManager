using Microsoft.AspNetCore.Mvc;
using SharpGrip.FluentValidation.AutoValidation.Mvc.Attributes;
using TransactionManager.Dto;
using TransactionManager.Services.Interfaces;
using TransactionManager.StaticConstants;

namespace TransactionManager.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TransactionController : ControllerBase
{
    private readonly ITransactionService _transactionService;

    public TransactionController(ITransactionService transactionService)
    {
        _transactionService = transactionService;
    }

    [HttpPost("upsert")]
    public async Task<ActionResult> UpsertTransactionsAsync(IFormFile file,
        CancellationToken cancellationToken = default)
    {
        await _transactionService.UpsertAsync(file, cancellationToken);

        return Ok();
    }

    [HttpPost("export/excel")]
    public async Task<ActionResult<byte[]>> ExportTransactionsAsync([FromBody] TransactionDateRangeDto? transactionDateRangeDto,
        CancellationToken cancellationToken)
    {
        var stream = await _transactionService.ExportTransactionsAsync(transactionDateRangeDto, cancellationToken);

        return File(stream, SD.ExcelContentType, $"transactions.xlsx");
    }

    [HttpPost("get-all/for-user-timezone")]
    public async Task<ActionResult<List<TransactionDto>>> GetTransactionsForUserTimezoneAsync(
        [FromBody] TransactionDateRangeDto transactionDateRangeDto,
        CancellationToken cancellationToken)
    {
        var transactions =
            await _transactionService.GetTransactionsForUserTimezoneAsync(transactionDateRangeDto, cancellationToken);

        return Ok(transactions);
    }

    [HttpPost("get-all/for-client-timezone")]
    public async Task<ActionResult<List<ClientTimezoneTransactionDto>>> GetTransactionsForClientTimezoneAsync(
        [FromBody] TransactionDateRangeDto transactionDateRangeDto,
        CancellationToken cancellationToken)
    {
        var transactions =
            await _transactionService.GetTransactionsForClientTimezoneAsync(transactionDateRangeDto, cancellationToken);

        return Ok(transactions);
    }

    [HttpPost("get-all/by-date")]
    public async Task<ActionResult<List<ClientTimezoneTransactionDto>>> GetTransactionsForClientTimezoneAsync(
        [FromBody] TransactionByDateDto transactionByDateDto,
        CancellationToken cancellationToken)
    {
        var transaction =
            await _transactionService.GetTransactionsForClientTimezoneAsync(transactionByDateDto, cancellationToken);

        return Ok(transaction);
    }
}