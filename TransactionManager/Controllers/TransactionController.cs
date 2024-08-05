using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using TransactionManager.Dto;
using TransactionManager.Entities;
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
    public async Task<ActionResult<byte[]>> ExportTransactionsAsync([FromBody] ExportTransactionDto? exportTransactionDto,
        CancellationToken cancellationToken = default)
    {
        var stream = await _transactionService.ExportTransactionsAsync(exportTransactionDto, cancellationToken);

        return File(stream, SD.ExcelContentType, $"transactions.xlsx");
    }
}