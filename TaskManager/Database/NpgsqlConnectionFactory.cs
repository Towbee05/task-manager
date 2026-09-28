using Npgsql;
using System.Data;

namespace TaskManager.Database;

public interface IConnectionFactory
{
    IDbConnection CreateConnection();
}


public class NpgsqlConnectionFactory : IConnectionFactory
{
    private readonly string _connectionString;
    public NpgsqlConnectionFactory(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("Default")
        ?? throw new InvalidOperationException("Connection string 'Default' is not yet configured");
    }

    public IDbConnection CreateConnection()
    {
        return new NpgsqlConnection(_connectionString);
    }
}