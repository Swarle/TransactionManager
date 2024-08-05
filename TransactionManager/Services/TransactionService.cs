using System.Net;
using TransactionManager.DataAccess.Interfaces;
using TransactionManager.Entities;
using TransactionManager.Exceptions;
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
        var transactions = await ParseFromCsvToEntityList(file);
        
        await _transactionDataAccess.UpsertTransactionsAsync(transactions, cancellationToken);
    }
    
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