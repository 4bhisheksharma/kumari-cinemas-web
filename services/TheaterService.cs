using Oracle.ManagedDataAccess.Client;

namespace KumariCinemas.Web.Services;

public class TheaterService
{
    private readonly DatabaseConnHelper _dbHelper;

    public TheaterService(DatabaseConnHelper dbHelper)
    {
        _dbHelper = dbHelper;
    }

    public List<Theater> GetAllTheaters()
    {
        var theaters = new List<Theater>();

        using var conn = _dbHelper.GetConnection();
        conn.Open();
        using var cmd = new OracleCommand(
            "SELECT TheatreID, TheatreName, TheatreCity, TheatreAddress, TheatreContactNumber FROM Theatre ORDER BY TheatreID", conn);
        using var reader = cmd.ExecuteReader();

        while (reader.Read())
        {
            theaters.Add(new Theater
            {
                TheatreID = reader.GetInt32(0),
                TheatreName = reader.GetString(1),
                TheatreCity = reader.IsDBNull(2) ? string.Empty : reader.GetString(2),
                TheatreAddress = reader.IsDBNull(3) ? string.Empty : reader.GetString(3),
                TheatreContactNumber = reader.IsDBNull(4) ? string.Empty : reader.GetString(4)
            });
        }

        return theaters;
    }

    public void CreateTheater(Theater theater)
    {
        using var conn = _dbHelper.GetConnection();
        conn.Open();
        using var cmd = new OracleCommand(
            @"INSERT INTO Theatre (TheatreID, TheatreName, TheatreCity, TheatreAddress, TheatreContactNumber) 
              VALUES ((SELECT NVL(MAX(TheatreID),0)+1 FROM Theatre), :name, :city, :address, :contact)", conn);
        cmd.Parameters.Add(new OracleParameter("name", theater.TheatreName));
        cmd.Parameters.Add(new OracleParameter("city", theater.TheatreCity));
        cmd.Parameters.Add(new OracleParameter("address", theater.TheatreAddress));
        cmd.Parameters.Add(new OracleParameter("contact", theater.TheatreContactNumber));
        cmd.ExecuteNonQuery();
    }

    public void UpdateTheater(Theater theater)
    {
        using var conn = _dbHelper.GetConnection();
        conn.Open();
        using var cmd = new OracleCommand(
            @"UPDATE Theatre SET TheatreName = :name, TheatreCity = :city, TheatreAddress = :address, 
              TheatreContactNumber = :contact WHERE TheatreID = :id", conn);
        cmd.Parameters.Add(new OracleParameter("name", theater.TheatreName));
        cmd.Parameters.Add(new OracleParameter("city", theater.TheatreCity));
        cmd.Parameters.Add(new OracleParameter("address", theater.TheatreAddress));
        cmd.Parameters.Add(new OracleParameter("contact", theater.TheatreContactNumber));
        cmd.Parameters.Add(new OracleParameter("id", theater.TheatreID));
        cmd.ExecuteNonQuery();
    }

    public void DeleteTheater(int theatreId)
    {
        using var conn = _dbHelper.GetConnection();
        conn.Open();
        using var cmd = new OracleCommand("DELETE FROM Theatre WHERE TheatreID = :id", conn);
        cmd.Parameters.Add(new OracleParameter("id", theatreId));
        cmd.ExecuteNonQuery();
    }
}
