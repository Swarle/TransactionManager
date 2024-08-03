using Dapper;
using TransactionManager.DataAccess.Interfaces;
using TransactionManager.Entities;
using TransactionManager.Persistence;

namespace TransactionManager.DataAccess;

public class TransactionDataAccess : ITransactionDataAccess
{
    private readonly SqlConnectionFactory _connectionFactory;

    public TransactionDataAccess(SqlConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task UpsertTransactionsAsync(List<Transaction> transactions, CancellationToken cancellationToken)
    {
        await using var connection = _connectionFactory.CreateConnection();
        await connection.OpenAsync(cancellationToken);

        const string request = @"INSERT INTO TRANSACTION
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


        await connection.ExecuteAsync(request, transactions);
    }
}