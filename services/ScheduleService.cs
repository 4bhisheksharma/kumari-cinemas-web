using Oracle.ManagedDataAccess.Client;
using Microsoft.AspNetCore.Mvc.Rendering;

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
        using var cmd = new OracleCommand(@"
            SELECT s.ShowID, s.ShowDate, s.ShowTime, s.StartTime, s.EndTime, s.MovieID, s.HallID,
                   m.Title, h.HallName, t.TheatreName
            FROM Show s
            LEFT JOIN Movie m ON s.MovieID = m.MovieID
            LEFT JOIN Hall h ON s.HallID = h.HallID
            LEFT JOIN Theatre t ON h.TheatreID = t.TheatreID
            ORDER BY s.ShowDate DESC, s.ShowTime", conn);
        using var reader = cmd.ExecuteReader();

        while (reader.Read())
        {
            schedules.Add(new Schedule
            {
                ShowID = reader.GetInt32(0),
                ShowDate = reader.GetDateTime(1),
                ShowTime = reader.GetDateTime(2),
                StartTime = reader.IsDBNull(3) ? null : reader.GetDateTime(3),
                EndTime = reader.IsDBNull(4) ? null : reader.GetDateTime(4),
                MovieID = reader.GetInt32(5),
                HallID = reader.GetInt32(6),
                MovieTitle = reader.IsDBNull(7) ? string.Empty : reader.GetString(7),
                HallName = reader.IsDBNull(8) ? string.Empty : reader.GetString(8),
                TheatreName = reader.IsDBNull(9) ? string.Empty : reader.GetString(9)
            });
        }

        return schedules;
    }

    public List<SelectListItem> GetMovieOptions()
    {
        var items = new List<SelectListItem>();
        using var conn = _dbHelper.GetConnection();
        conn.Open();
        using var cmd = new OracleCommand("SELECT MovieID, Title FROM Movie ORDER BY Title", conn);
        using var reader = cmd.ExecuteReader();
        while (reader.Read())
        {
            items.Add(new SelectListItem
            {
                Value = reader.GetInt32(0).ToString(),
                Text = reader.GetString(1)
            });
        }
        return items;
    }

    public List<SelectListItem> GetHallOptions()
    {
        var items = new List<SelectListItem>();
        using var conn = _dbHelper.GetConnection();
        conn.Open();
        using var cmd = new OracleCommand(@"
            SELECT h.HallID, h.HallName || ' (' || t.TheatreName || ')'
            FROM Hall h
            LEFT JOIN Theatre t ON h.TheatreID = t.TheatreID
            ORDER BY t.TheatreName, h.HallName", conn);
        using var reader = cmd.ExecuteReader();
        while (reader.Read())
        {
            items.Add(new SelectListItem
            {
                Value = reader.GetInt32(0).ToString(),
                Text = reader.IsDBNull(1) ? $"Hall #{reader.GetInt32(0)}" : reader.GetString(1)
            });
        }
        return items;
    }

    public void CreateSchedule(Schedule schedule)
    {
        using var conn = _dbHelper.GetConnection();
        conn.Open();
        using var cmd = new OracleCommand(@"
            INSERT INTO Show (ShowID, ShowDate, ShowTime, StartTime, EndTime, MovieID, HallID) 
            VALUES ((SELECT NVL(MAX(ShowID),0)+1 FROM Show), :showDate, :showTime, :startTime, :endTime, :movieID, :hallID)", conn);
        cmd.Parameters.Add(new OracleParameter("showDate", schedule.ShowDate));
        cmd.Parameters.Add(new OracleParameter("showTime", schedule.ShowTime));
        cmd.Parameters.Add(new OracleParameter("startTime", (object?)schedule.StartTime ?? DBNull.Value));
        cmd.Parameters.Add(new OracleParameter("endTime", (object?)schedule.EndTime ?? DBNull.Value));
        cmd.Parameters.Add(new OracleParameter("movieID", schedule.MovieID));
        cmd.Parameters.Add(new OracleParameter("hallID", schedule.HallID));
        cmd.ExecuteNonQuery();
    }

    public void UpdateSchedule(Schedule schedule)
    {
        using var conn = _dbHelper.GetConnection();
        conn.Open();
        using var cmd = new OracleCommand(@"
            UPDATE Show SET ShowDate = :showDate, ShowTime = :showTime, StartTime = :startTime, 
            EndTime = :endTime, MovieID = :movieID, HallID = :hallID WHERE ShowID = :id", conn);
        cmd.Parameters.Add(new OracleParameter("showDate", schedule.ShowDate));
        cmd.Parameters.Add(new OracleParameter("showTime", schedule.ShowTime));
        cmd.Parameters.Add(new OracleParameter("startTime", (object?)schedule.StartTime ?? DBNull.Value));
        cmd.Parameters.Add(new OracleParameter("endTime", (object?)schedule.EndTime ?? DBNull.Value));
        cmd.Parameters.Add(new OracleParameter("movieID", schedule.MovieID));
        cmd.Parameters.Add(new OracleParameter("hallID", schedule.HallID));
        cmd.Parameters.Add(new OracleParameter("id", schedule.ShowID));
        cmd.ExecuteNonQuery();
    }

    public void DeleteSchedule(int showId)
    {
        using var conn = _dbHelper.GetConnection();
        conn.Open();
        using var cmd = new OracleCommand("DELETE FROM Show WHERE ShowID = :id", conn);
        cmd.Parameters.Add(new OracleParameter("id", showId));
        cmd.ExecuteNonQuery();
    }
}
