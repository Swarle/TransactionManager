using Npgsql;

namespace TransactionManager.Persistence;

/// <summary>
/// A factory class for creating instances of <see cref="NpgsqlConnection"/> using the specified connection string.
/// </summary>
public class SqlConnectionFactory
{
    private readonly string _connectionString;

    /// <summary>
    /// Initializes a new instance of the <see cref="SqlConnectionFactory"/> class with the specified connection string.
    /// </summary>
    /// <param name="connectionString">The connection string used to connect to the database.</param>
    public SqlConnectionFactory(string connectionString)
    {
        _connectionString = connectionString;
    }

    /// <summary>
    /// Creates and returns a new instance of <see cref="NpgsqlConnection"/> using the stored connection string.
    /// </summary>
    /// <returns>A new <see cref="NpgsqlConnection"/> instance.</returns>
    public NpgsqlConnection CreateConnection()
    {
        return new NpgsqlConnection(_connectionString);
    }
}