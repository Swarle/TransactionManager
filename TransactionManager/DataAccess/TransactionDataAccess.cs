using System.Text;
using Dapper;
using TransactionManager.DataAccess.Interfaces;
using TransactionManager.Entities;
using TransactionManager.Entities.ModifiedEntities;
using TransactionManager.Persistence;

namespace TransactionManager.DataAccess;

/// <summary>
/// Implementation of the <see cref="ITransactionDataAccess"/> interface
/// that manages transactions
/// </summary>
public class TransactionDataAccess : ITransactionDataAccess
{
    private readonly SqlConnectionFactory _connectionFactory;
    private readonly ILogger<TransactionDataAccess> _logger;

    public TransactionDataAccess(SqlConnectionFactory connectionFactory, ILogger<TransactionDataAccess> logger)
    {
        _connectionFactory = connectionFactory;
        _logger = logger;
    }
    
    /// <summary>
    /// Asynchronously upsert transactions to the database.
    /// </summary>
    /// <param name="transactions">An list of <see cref="Transaction"/> objects.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>An <see cref="Task"/> that represent asynchronous operation</returns>
    public async Task UpsertTransactionsAsync(List<Transaction> transactions, CancellationToken cancellationToken = default)
    {
        await using var connection = _connectionFactory.CreateConnection();
        await connection.OpenAsync(cancellationToken);

        const string request = @"INSERT INTO ""Transactions""
                            (""TransactionId"", ""Name"", ""Email"", ""Amount"", ""TransactionDateUtc"", ""TransactionTimezone"", ""Latitude"", ""Longitude"")
                            VALUES (@TransactionId, @Name, @Email, @Amount, @TransactionDateUtc, @TransactionTimezone, @Latitude, @Longitude)
                                ON CONFLICT(""TransactionId"") 
                                DO UPDATE SET
                                    ""Name"" = EXCLUDED.""Name"",
                                    ""Email"" = EXCLUDED.""Email"",
                                    ""Amount"" = EXCLUDED.""Amount"",
                                    ""TransactionDateUtc"" = EXCLUDED.""TransactionDateUtc"",
                                    ""TransactionTimezone"" = EXCLUDED.""TransactionTimezone"",
                                    ""Latitude"" = EXCLUDED.""Latitude"",
                                    ""Longitude"" = EXCLUDED.""Longitude""";
        
        _logger.LogInformation("Executing SQL: {Request} with number of entities {NumberOfEntities}", request, transactions.Count);

        var affectedRowsCount = await connection.ExecuteAsync(request, transactions);
        
        _logger.LogInformation("The request was successfully executed, number of affected rows {AffectedRowsCount}", affectedRowsCount);
    }
    
    /// <summary>
    /// An asynchronous operation that receives all transactions from the database.
    /// </summary>
    /// <param name="cancellationToken">An cancellation token.</param>
    /// <returns>A list of <see cref="Transaction"/></returns>
    public async Task<List<Transaction>> GetAllTransactionsAsync(CancellationToken cancellationToken = default)
    {
        await using var connection = _connectionFactory.CreateConnection();
        await connection.OpenAsync(cancellationToken);

        const string request = @"SELECT ""TransactionId"", ""Name"", ""Email"",
                                    ""Amount"", ""TransactionDateUtc"",""TransactionTimezone"", ""Latitude"", ""Longitude""
	                                FROM ""Transactions""";
        
        _logger.LogInformation("Executing SQL: {Request}", request);

        var transactions = await connection.QueryAsync<Transaction>(request);

        return transactions.ToList();
    }
    

    
    /// <summary>
    /// An asynchronous operation that retrieves all
    /// transactions from the database between two dates.
    /// </summary>
    /// <remarks>
    /// <paramref name="startDate"/> and <paramref name="endDate"/>
    /// should have UTC Kind.
    /// The method searches in UTC format.
    /// </remarks>
    /// <param name="startDate">The start date of the range.</param>
    /// <param name="endDate">The end date of the range.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A list of <see cref="Transaction"/> objects.</returns>
    public async Task<List<Transaction>> GetAllTransactionsAsync(
        DateTime startDate,
        DateTime endDate,
        CancellationToken cancellationToken = default)
    {
        await using var connection = _connectionFactory.CreateConnection();
        await connection.OpenAsync(cancellationToken);

        const string request = @"SELECT ""TransactionId"", ""Name"", ""Email"",
                                    ""Amount"", ""TransactionDateUtc"",""TransactionTimezone"", ""Latitude"", ""Longitude""
	                                FROM ""Transactions""
                                    WHERE (""TransactionDateUtc"" AT TIME ZONE 'UTC') BETWEEN @StartDate AND @EndDate";

        var parameters = new { StartDate = startDate, EndDate = endDate};
        
        _logger.LogInformation("Executing SQL: {Request}", request);

        var transactions = await connection.QueryAsync<Transaction>(request, parameters);

        return transactions.ToList();
    }
    
    /// <summary>
    /// An asynchronous operation that retrieves all transactions from the database
    /// between two dates in the specified timezone.
    /// (timezone specified in <paramref name="timezoneId" /> parameter)
    /// </summary>
    /// <remarks>
    /// <paramref name="startDate"/> and <paramref name="endDate"/>
    /// should have UTC Kind.
    /// The method searches in UTC format.
    /// </remarks>
    /// <param name="startDate">The start date of the range.</param>
    /// <param name="endDate">The end date of the range.</param>
    /// <param name="timezoneId">The ID of the timezone.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A list of <see cref="UserTimezoneTransaction"/> objects.</returns>
    public async Task<List<UserTimezoneTransaction>> GetAllTransactionsForUserTimezoneAsync(
        DateTime startDate,
        DateTime endDate,
        string timezoneId,
        CancellationToken cancellationToken = default)
    {
        await using var connection = _connectionFactory.CreateConnection();
        await connection.OpenAsync(cancellationToken);

        const string request = @"SELECT ""TransactionId"", ""Name"", ""Email"",
                                    ""Amount"", ""TransactionDateUtc"",
                                    (""TransactionDateUtc"" AT TIME ZONE 'UTC' AT TIME ZONE @TimeZoneId) as TransactionDateInUserTimezone,
                                    ""TransactionTimezone"", ""Latitude"", ""Longitude""
	                                FROM ""Transactions""
                                    WHERE (""TransactionDateUtc"" AT TIME ZONE 'UTC')
                                        BETWEEN @StartDate AND @EndDate";

        var parameters = new { StartDate = startDate, EndDate = endDate, TimeZoneId = timezoneId };
        
        _logger.LogInformation("Executing SQL: {Request}", request);

        var transactions =
            await connection.QueryAsync<UserTimezoneTransaction>(request, parameters);

        return transactions.ToList();
    }
    
    /// <summary>
    /// An asynchronous operation that retrieves
    /// all transactions from the database between two dates
    /// </summary>
    /// <remarks>
    /// The method searches for transactions relative
    /// to the local time when the transaction occurred
    /// </remarks>
    /// <param name="startDate">The start date of the range.</param>
    /// <param name="endDate">The end date of the range.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A list of <see cref="ClientTimezoneTransaction"/> objects.</returns>
    public async Task<List<ClientTimezoneTransaction>> GetAllTransactionsForClientTimezoneAsync(
        DateTime startDate,
        DateTime endDate,
        CancellationToken cancellationToken = default)
    {
        await using var connection = _connectionFactory.CreateConnection();
        await connection.OpenAsync(cancellationToken);

        const string request = @"SELECT ""TransactionId"", ""Name"", ""Email"",
                                    ""Amount"", ""TransactionDateUtc"",
                                    (""TransactionDateUtc"" AT TIME ZONE 'UTC' AT TIME ZONE ""TransactionTimezone"") as TransactionDateInClientTimezone,
                                    ""TransactionTimezone"", ""Latitude"", ""Longitude""
	                                FROM ""Transactions""
                                    WHERE (""TransactionDateUtc"" AT TIME ZONE 'UTC' AT TIME ZONE ""TransactionTimezone"") 
                                        BETWEEN @StartDate AND @EndDate";

        var parameters = new { StartDate = startDate, EndDate = endDate};
        
        _logger.LogInformation("Executing SQL: {Request}", request);

        var transactions =
            await connection.QueryAsync<ClientTimezoneTransaction>(request, parameters);

        return transactions.ToList();
    }
    
    /// <summary>
    /// An asynchronous operation that returns a list of transactions
    /// that occurred on a specific date.
    /// </summary>
    /// <remarks>
    /// The method searches for transactions relative
    /// to the local time when the transaction occurred
    /// </remarks>
    /// <param name="year">The year in which the transaction took place.</param>
    /// <param name="month">The month in which the transaction took place (optional).</param>
    /// <param name="day">The day in which the transaction took place (optional).</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A list of <see cref="ClientTimezoneTransaction"/> objects</returns>
    public async Task<List<ClientTimezoneTransaction>> GetAllTransactionsForClientTimezoneAsync(
        int year,
        int? month = null,
        int? day = null,
        CancellationToken cancellationToken = default)
    {
        await using var connection = _connectionFactory.CreateConnection();
        await connection.OpenAsync(cancellationToken);

        var request = new StringBuilder(@"SELECT ""TransactionId"", ""Name"", ""Email"",
                                    ""Amount"", ""TransactionDateUtc"",
                                    (""TransactionDateUtc"" AT TIME ZONE 'UTC' AT TIME ZONE ""TransactionTimezone"") as TransactionDateInClientTimezone,
                                    ""TransactionTimezone"", ""Latitude"", ""Longitude""
	                                FROM ""Transactions""
                                    WHERE DATE_PART('year', ""TransactionDateUtc"" AT TIME ZONE 'UTC' AT TIME ZONE ""TransactionTimezone"") = @Year");

        var parameters = new DynamicParameters();
        parameters.Add("Year", year);

        if (month is not null)
        {
            request.Append(
                @" AND DATE_PART('month', ""TransactionDateUtc"" AT TIME ZONE 'UTC' AT TIME ZONE ""TransactionTimezone"") = @Month");
            parameters.Add("Month", month);
        }

        if (day is not null)
        {
            request.Append(
                @" AND DATE_PART('day', ""TransactionDateUtc"" AT TIME ZONE 'UTC' AT TIME ZONE ""TransactionTimezone"") = @Day");
            parameters.Add("Day", day);
        }
        
        _logger.LogInformation("Executing SQL: {Request}", request.ToString());

        var transactions =
            await connection.QueryAsync<ClientTimezoneTransaction>(request.ToString(), parameters);

        return transactions.ToList();
    }
}