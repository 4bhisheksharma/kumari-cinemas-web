using Oracle.ManagedDataAccess.Client;
using System.Data;

namespace KumariCinemas.Web.Services;

/// <summary>
/// Base service providing common database helper methods.
/// All entity services inherit from this to avoid repeating
/// connection/command/parameter boilerplate.
/// </summary>
public abstract class BaseService
{
    protected readonly DatabaseConnHelper DbHelper;

    protected BaseService(DatabaseConnHelper dbHelper)
    {
        DbHelper = dbHelper;
    }

    /// <summary>Opens a connection and returns it. Caller must dispose.</summary>
    protected OracleConnection OpenConnection()
    {
        var conn = DbHelper.GetConnection();
        conn.Open();
        return conn;
    }

    /// <summary>Executes a query and maps each row via the provided mapper function.</summary>
    protected List<T> Query<T>(string sql, Func<OracleDataReader, T> mapper, params OracleParameter[] parameters)
    {
        var results = new List<T>();
        using var conn = OpenConnection();
        using var cmd = new OracleCommand(sql, conn);
        if (parameters.Length > 0)
            cmd.Parameters.AddRange(parameters);
        using var reader = cmd.ExecuteReader();
        while (reader.Read())
            results.Add(mapper(reader));
        return results;
    }

    /// <summary>Executes a non-query (INSERT / UPDATE / DELETE) and returns rows affected.</summary>
    protected int Execute(string sql, params OracleParameter[] parameters)
    {
        using var conn = OpenConnection();
        using var cmd = new OracleCommand(sql, conn);
        if (parameters.Length > 0)
            cmd.Parameters.AddRange(parameters);
        return cmd.ExecuteNonQuery();
    }

    /// <summary>Executes a scalar query and returns the result.</summary>
    protected object? Scalar(string sql, params OracleParameter[] parameters)
    {
        using var conn = OpenConnection();
        using var cmd = new OracleCommand(sql, conn);
        if (parameters.Length > 0)
            cmd.Parameters.AddRange(parameters);
        return cmd.ExecuteScalar();
    }

    /// <summary>Shorthand to create an OracleParameter, handling nulls.</summary>
    protected static OracleParameter Param(string name, object? value)
        => new(name, value ?? DBNull.Value);
}
