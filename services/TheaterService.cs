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
            "SELECT TheaterID, ScreenName, HallName, TheaterType, SeatRows, SeatColumns, TotalSeats, Status FROM TheaterTable ORDER BY TheaterID", conn);
        using var reader = cmd.ExecuteReader();

        while (reader.Read())
        {
            theaters.Add(new Theater
            {
                TheaterID = reader.GetInt32(0),
                ScreenName = reader.GetString(1),
                HallName = reader.IsDBNull(2) ? string.Empty : reader.GetString(2),
                TheaterType = reader.IsDBNull(3) ? "Standard" : reader.GetString(3),
                SeatRows = reader.IsDBNull(4) ? 0 : reader.GetInt32(4),
                SeatColumns = reader.IsDBNull(5) ? 0 : reader.GetInt32(5),
                TotalSeats = reader.IsDBNull(6) ? 0 : reader.GetInt32(6),
                Status = reader.IsDBNull(7) ? "Active" : reader.GetString(7)
            });
        }

        return theaters;
    }

    public void CreateTheater(Theater theater)
    {
        using var conn = _dbHelper.GetConnection();
        conn.Open();
        using var cmd = new OracleCommand(
            "INSERT INTO TheaterTable (ScreenName, HallName, TheaterType, SeatRows, SeatColumns, TotalSeats, Status) VALUES (:screen, :hall, :type, :rows, :cols, :total, :status)", conn);
        cmd.Parameters.Add(new OracleParameter("screen", theater.ScreenName));
        cmd.Parameters.Add(new OracleParameter("hall", theater.HallName));
        cmd.Parameters.Add(new OracleParameter("type", theater.TheaterType));
        cmd.Parameters.Add(new OracleParameter("rows", theater.SeatRows));
        cmd.Parameters.Add(new OracleParameter("cols", theater.SeatColumns));
        cmd.Parameters.Add(new OracleParameter("total", theater.TotalSeats));
        cmd.Parameters.Add(new OracleParameter("status", theater.Status));
        cmd.ExecuteNonQuery();
    }

    public void UpdateTheater(Theater theater)
    {
        using var conn = _dbHelper.GetConnection();
        conn.Open();
        using var cmd = new OracleCommand(
            "UPDATE TheaterTable SET ScreenName = :screen, HallName = :hall, TheaterType = :type, SeatRows = :rows, SeatColumns = :cols, TotalSeats = :total, Status = :status WHERE TheaterID = :id", conn);
        cmd.Parameters.Add(new OracleParameter("screen", theater.ScreenName));
        cmd.Parameters.Add(new OracleParameter("hall", theater.HallName));
        cmd.Parameters.Add(new OracleParameter("type", theater.TheaterType));
        cmd.Parameters.Add(new OracleParameter("rows", theater.SeatRows));
        cmd.Parameters.Add(new OracleParameter("cols", theater.SeatColumns));
        cmd.Parameters.Add(new OracleParameter("total", theater.TotalSeats));
        cmd.Parameters.Add(new OracleParameter("status", theater.Status));
        cmd.Parameters.Add(new OracleParameter("id", theater.TheaterID));
        cmd.ExecuteNonQuery();
    }

    public void DeleteTheater(int theaterId)
    {
        using var conn = _dbHelper.GetConnection();
        conn.Open();
        using var cmd = new OracleCommand("DELETE FROM TheaterTable WHERE TheaterID = :id", conn);
        cmd.Parameters.Add(new OracleParameter("id", theaterId));
        cmd.ExecuteNonQuery();
    }
}
