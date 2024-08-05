using System.Data;
using System.Net;
using System.Reflection;
using ClosedXML.Excel;
using DocumentFormat.OpenXml.Office2010.CustomUI;
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

public class TransactionService : ITransactionService
{
    private readonly ITransactionDataAccess _transactionDataAccess;
    private readonly HttpContext _httpContext;

    public TransactionService(ITransactionDataAccess transactionDataAccess, IHttpContextAccessor accessor)
    {
        _transactionDataAccess = transactionDataAccess;
        _httpContext = accessor.HttpContext ??
                       throw new NullReferenceException("HttpContextAccessor does`t have context");
    }

    public async Task UpsertAsync(IFormFile file, CancellationToken cancellationToken)
    {
        var transactions = await ParseFromCsvToEntityList(file);
        
        await _transactionDataAccess.UpsertTransactionsAsync(transactions, cancellationToken);
    }

    public async Task<byte[]> ExportTransactionsAsync(ExportTransactionDto? exportTransactionDto, CancellationToken cancellationToken)
    {
        List<Transaction> transactions;

        if (exportTransactionDto is null)
            transactions = await _transactionDataAccess.GetAllTransactionsAsync(cancellationToken);
        else
            transactions = await GetTransactionsByDateAsync(exportTransactionDto.StartDate,
                exportTransactionDto.EndDate, cancellationToken);

        if (transactions.Count == 0)
            throw new HttpException(HttpStatusCode.NotFound);
        
        var excelStream = ExcelConverter.ConvertToExcel(transactions);
        
        return excelStream;
    }

    private async Task<List<Transaction>> GetTransactionsByDateAsync(DateTime startDate, DateTime endDate,
        CancellationToken cancellationToken)
    {
        if (startDate > endDate)
            throw new HttpException(HttpStatusCode.BadRequest, "Start Date should be less then End Date");
        
        if (startDate.Kind != endDate.Kind)
            throw new HttpException(HttpStatusCode.BadRequest, "Start Date and End Date must have the same kind");

        List<Transaction> transactions;
        
        if (startDate.Kind == DateTimeKind.Utc && endDate.Kind == DateTimeKind.Utc)
        {
            transactions = await _transactionDataAccess.GetAllTransactionsAsync(startDate, endDate, cancellationToken);
        }
        else
        {
            var userTimezoneId = _httpContext.Request.GetTimezoneFromHeader();

            transactions = await _transactionDataAccess.GetAllTransactionsAsync(startDate,
                endDate, userTimezoneId, cancellationToken);
        }

        return transactions;
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