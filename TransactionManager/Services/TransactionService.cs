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

    public async Task UpsertAsync(IFormFile file, CancellationToken cancellationToken = default)
    {
        var transactions = await ParseFromCsvToEntityList(file);
        
        await _transactionDataAccess.UpsertTransactionsAsync(transactions, cancellationToken);
    }

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

    public async Task<List<UserTimezoneTransactionDto>> GetTransactionsForUserTimezoneAsync(
        TransactionDateRangeDto transactionDateRangeDto,
        CancellationToken cancellationToken = default)
    {
        if (transactionDateRangeDto.StartDate.Kind == DateTimeKind.Utc ||
            transactionDateRangeDto.EndDate.Kind == DateTimeKind.Utc)
            throw new HttpException(HttpStatusCode.BadRequest, "Time must be local, not UTC");
        
        var userTimezoneId = _httpContext.Request.GetTimezoneFromHeader();

        var startDateUtc = TimeHelper.ConvertToUtc(transactionDateRangeDto.StartDate, userTimezoneId);
        var endDateUtc = TimeHelper.ConvertToUtc(transactionDateRangeDto.EndDate, userTimezoneId);
        
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

    public async Task<List<ClientTimezoneTransactionDto>> GetTransactionsForClientTimezoneAsync(
        TransactionDateRangeDto transactionDateRangeDto,
        CancellationToken cancellationToken = default)
    {
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

    private async Task<List<Transaction>> GetTransactionsByDateAsync(DateTime startDate, DateTime endDate,
        CancellationToken cancellationToken = default)
    {
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