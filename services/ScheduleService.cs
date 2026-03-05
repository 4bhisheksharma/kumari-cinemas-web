using Oracle.ManagedDataAccess.Client;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace KumariCinemas.Web.Services;

public class ScheduleService : BaseService
{
    public ScheduleService(DatabaseConnHelper dbHelper) : base(dbHelper) { }

    public List<Schedule> GetAllSchedules() => Query(
        @"SELECT s.ShowID, s.ShowDate, s.ShowTime, s.StartTime, s.EndTime, s.MovieID, s.HallID,
                 m.Title, h.HallName, t.TheatreName
          FROM Show s
          LEFT JOIN Movie m ON s.MovieID = m.MovieID
          LEFT JOIN Hall h ON s.HallID = h.HallID
          LEFT JOIN Theatre t ON h.TheatreID = t.TheatreID
          ORDER BY s.ShowDate DESC, s.ShowTime",
        r => new Schedule
        {
            ShowID = r.GetInt32(0),
            ShowDate = r.GetDateTime(1),
            ShowTime = r.GetDateTime(2),
            StartTime = r.IsDBNull(3) ? null : r.GetDateTime(3),
            EndTime = r.IsDBNull(4) ? null : r.GetDateTime(4),
            MovieID = r.GetInt32(5),
            HallID = r.GetInt32(6),
            MovieTitle = r.IsDBNull(7) ? string.Empty : r.GetString(7),
            HallName = r.IsDBNull(8) ? string.Empty : r.GetString(8),
            TheatreName = r.IsDBNull(9) ? string.Empty : r.GetString(9)
        });

    public List<SelectListItem> GetMovieOptions() => Query(
        "SELECT MovieID, Title FROM Movie ORDER BY Title",
        r => new SelectListItem { Value = r.GetInt32(0).ToString(), Text = r.GetString(1) });

    public List<SelectListItem> GetHallOptions() => Query(
        @"SELECT h.HallID, h.HallName || ' (' || t.TheatreName || ')'
          FROM Hall h LEFT JOIN Theatre t ON h.TheatreID = t.TheatreID
          ORDER BY t.TheatreName, h.HallName",
        r => new SelectListItem
        {
            Value = r.GetInt32(0).ToString(),
            Text = r.IsDBNull(1) ? $"Hall #{r.GetInt32(0)}" : r.GetString(1)
        });

    public void CreateSchedule(Schedule s) => Execute(
        @"INSERT INTO Show (ShowID, ShowDate, ShowTime, StartTime, EndTime, MovieID, HallID)
          VALUES ((SELECT NVL(MAX(ShowID),0)+1 FROM Show), :showDate, :showTime, :startTime, :endTime, :movieID, :hallID)",
        Param("showDate", s.ShowDate), Param("showTime", s.ShowTime),
        Param("startTime", s.StartTime), Param("endTime", s.EndTime),
        Param("movieID", s.MovieID), Param("hallID", s.HallID));

    public void UpdateSchedule(Schedule s) => Execute(
        @"UPDATE Show SET ShowDate=:showDate, ShowTime=:showTime, StartTime=:startTime,
          EndTime=:endTime, MovieID=:movieID, HallID=:hallID WHERE ShowID=:id",
        Param("showDate", s.ShowDate), Param("showTime", s.ShowTime),
        Param("startTime", s.StartTime), Param("endTime", s.EndTime),
        Param("movieID", s.MovieID), Param("hallID", s.HallID),
        Param("id", s.ShowID));

    public void DeleteSchedule(int showId) => Execute(
        "DELETE FROM Show WHERE ShowID = :id",
        Param("id", showId));
}
