using Oracle.ManagedDataAccess.Client;
using System.Data;

public class DatabaseConnHelper(IConfiguration configuration)
{
    private readonly string _connectionString = configuration.GetConnectionString("OracleConnection");

    public OracleConnection GetConnection()
    {
        return new OracleConnection(_connectionString);
    }
}