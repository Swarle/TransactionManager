using Microsoft.AspNetCore.Mvc;
using TransactionManager.Services.Interfaces;

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
}