using Dapper;
using TransactionManager.DataAccess.Interfaces;
using TransactionManager.Entities;
using TransactionManager.Persistence;

namespace TransactionManager.DataAccess;

public class TransactionDataAccess : ITransactionDataAccess
{
    private readonly SqlConnectionFactory _connectionFactory;
    private readonly ILogger<TransactionDataAccess> _logger;

    public TransactionDataAccess(SqlConnectionFactory connectionFactory, ILogger<TransactionDataAccess> logger)
    {
        _connectionFactory = connectionFactory;
        _logger = logger;
    }

    public async Task UpsertTransactionsAsync(List<Transaction> transactions, CancellationToken cancellationToken)
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
                                    ""Longitude"" = EXCLUDED.""Longitude"";";
        
        _logger.LogInformation("Executing SQL: {Request} with number of entities {NumberOfEntities}", request, transactions.Count);

        var affectedRowsCount = await connection.ExecuteAsync(request, transactions);
        
        _logger.LogInformation("The request was successfully executed, number of affected rows {AffectedRowsCount}", affectedRowsCount);
    }
}