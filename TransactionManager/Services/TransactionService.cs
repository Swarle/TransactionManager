using TransactionManager.DataAccess.Interfaces;
using TransactionManager.Entities;
using TransactionManager.Helpers;
using TransactionManager.Helpers.Parser;
using TransactionManager.Helpers.Parser.Mappers;
using TransactionManager.Services.Interfaces;

namespace TransactionManager.Services;

public class TransactionService : ITransactionService
{
    private readonly ITransactionDataAccess _transactionDataAccess;

    public TransactionService(ITransactionDataAccess transactionDataAccess)
    {
        _transactionDataAccess = transactionDataAccess;
    }

    public async Task UpsertAsync(IFormFile file, CancellationToken cancellationToken)
    {
        var transactionCsvMapper = new TransactionCsvMapper();
        var transactions = await CsvParser.ParseCsvToEntities(file,transactionCsvMapper);

        Console.ReadLine();
    }
}