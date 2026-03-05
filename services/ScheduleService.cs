using Oracle.ManagedDataAccess.Client;

namespace KumariCinemas.Web.Services;

public class ScheduleService
{
    private readonly DatabaseConnHelper _dbHelper;

    public ScheduleService(DatabaseConnHelper dbHelper)
    {
        _dbHelper = dbHelper;
    }

    public List<Schedule> GetAllSchedules()
    {
        var schedules = new List<Schedule>();

        using var conn = _dbHelper.GetConnection();
        conn.Open();
        using var cmd = new OracleCommand(
            "SELECT ScheduleID, MovieTitle, ScreenName, ShowDate, StartTime, EndTime, TicketPrice, Status FROM ScheduleTable ORDER BY ShowDate DESC, StartTime", conn);
        using var reader = cmd.ExecuteReader();

        while (reader.Read())
        {
            schedules.Add(new Schedule
            {
                ScheduleID = reader.GetInt32(0),
                MovieTitle = reader.IsDBNull(1) ? string.Empty : reader.GetString(1),
                ScreenName = reader.IsDBNull(2) ? string.Empty : reader.GetString(2),
                ShowDate = reader.IsDBNull(3) ? DateTime.MinValue : reader.GetDateTime(3),
                StartTime = reader.IsDBNull(4) ? string.Empty : reader.GetString(4),
                EndTime = reader.IsDBNull(5) ? string.Empty : reader.GetString(5),
                TicketPrice = reader.IsDBNull(6) ? 0 : reader.GetDecimal(6),
                Status = reader.IsDBNull(7) ? "Scheduled" : reader.GetString(7)
            });
        }

        return schedules;
    }

    public void CreateSchedule(Schedule schedule)
    {
        using var conn = _dbHelper.GetConnection();
        conn.Open();
        using var cmd = new OracleCommand(
            "INSERT INTO ScheduleTable (MovieTitle, ScreenName, ShowDate, StartTime, EndTime, TicketPrice, Status) VALUES (:movie, :screen, :showDate, :start, :endTime, :price, :status)", conn);
        cmd.Parameters.Add(new OracleParameter("movie", schedule.MovieTitle));
        cmd.Parameters.Add(new OracleParameter("screen", schedule.ScreenName));
        cmd.Parameters.Add(new OracleParameter("showDate", schedule.ShowDate));
        cmd.Parameters.Add(new OracleParameter("start", schedule.StartTime));
        cmd.Parameters.Add(new OracleParameter("endTime", schedule.EndTime));
        cmd.Parameters.Add(new OracleParameter("price", schedule.TicketPrice));
        cmd.Parameters.Add(new OracleParameter("status", schedule.Status));
        cmd.ExecuteNonQuery();
    }

    public void UpdateSchedule(Schedule schedule)
    {
        using var conn = _dbHelper.GetConnection();
        conn.Open();
        using var cmd = new OracleCommand(
            "UPDATE ScheduleTable SET MovieTitle = :movie, ScreenName = :screen, ShowDate = :showDate, StartTime = :start, EndTime = :endTime, TicketPrice = :price, Status = :status WHERE ScheduleID = :id", conn);
        cmd.Parameters.Add(new OracleParameter("movie", schedule.MovieTitle));
        cmd.Parameters.Add(new OracleParameter("screen", schedule.ScreenName));
        cmd.Parameters.Add(new OracleParameter("showDate", schedule.ShowDate));
        cmd.Parameters.Add(new OracleParameter("start", schedule.StartTime));
        cmd.Parameters.Add(new OracleParameter("endTime", schedule.EndTime));
        cmd.Parameters.Add(new OracleParameter("price", schedule.TicketPrice));
        cmd.Parameters.Add(new OracleParameter("status", schedule.Status));
        cmd.Parameters.Add(new OracleParameter("id", schedule.ScheduleID));
        cmd.ExecuteNonQuery();
    }

    public void DeleteSchedule(int scheduleId)
    {
        using var conn = _dbHelper.GetConnection();
        conn.Open();
        using var cmd = new OracleCommand("DELETE FROM ScheduleTable WHERE ScheduleID = :id", conn);
        cmd.Parameters.Add(new OracleParameter("id", scheduleId));
        cmd.ExecuteNonQuery();
    }
}
