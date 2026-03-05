using Oracle.ManagedDataAccess.Client;
using Microsoft.AspNetCore.Mvc.Rendering;
using KumariCinemas.Web.Models;

namespace KumariCinemas.Web.Services;

/// <summary>
/// Service for complex report queries that span multiple tables.
/// </summary>
public class ReportService : BaseService
{
    public ReportService(DatabaseConnHelper dbHelper) : base(dbHelper) { }

    // ────────────────────────────────────────────────────
    //  User dropdown for the report form
    // ────────────────────────────────────────────────────
    public List<SelectListItem> GetUserOptions() => Query(
        "SELECT UserID, UserName || ' (' || ContactNumber || ')' FROM UserTable ORDER BY UserName",
        r => new SelectListItem
        {
            Value = r.GetInt32(0).ToString(),
            Text = r.IsDBNull(1) ? $"User #{r.GetInt32(0)}" : r.GetString(1)
        });

    // ────────────────────────────────────────────────────
    //  Complex query: User Ticket Report (last 6 months)
    // ────────────────────────────────────────────────────
    //
    //  SQL QUERY (Oracle 21c):
    //  ---------------------------------------------------------
    //  SELECT u.UserID, u.UserName, u.ContactNumber, u.Email, u.Address,
    //         t.TicketID, t.TicketNumber, t.TicketStatus,
    //         b.ConfirmationNumber, b.BookingTime, b.ReservationDate,
    //         m.Title, m.Genre, m.Language, m.Duration,
    //         s.ShowDate, s.ShowTime,
    //         h.HallName, th.TheatreName,
    //         st.SeatRow, st.SeatNumber,
    //         NVL(p.Amount, 0) AS Amount
    //  FROM   UserTable u
    //         JOIN Booking b  ON b.UserID    = u.UserID
    //         JOIN Ticket  t  ON t.BookingID = b.BookingID
    //         JOIN Show    s  ON b.ShowID    = s.ShowID
    //         JOIN Movie   m  ON s.MovieID   = m.MovieID
    //         JOIN Hall    h  ON s.HallID    = h.HallID
    //         JOIN Theatre th ON h.TheatreID = th.TheatreID
    //         LEFT JOIN Seat    st ON t.SeatID  = st.SeatID
    //         LEFT JOIN Pricing p  ON t.PriceID = p.PriceID
    //  WHERE  u.UserID = :userId
    //    AND  b.ReservationDate >= ADD_MONTHS(SYSDATE, -6)
    //  ORDER BY b.ReservationDate DESC, t.TicketID DESC
    //  ---------------------------------------------------------

    private const string UserTicketSql = @"
        SELECT u.UserID, u.UserName, u.ContactNumber, u.Email, u.Address,
               t.TicketID, t.TicketNumber, t.TicketStatus,
               b.ConfirmationNumber, b.BookingTime, b.ReservationDate,
               m.Title, m.Genre, m.Language, m.Duration,
               s.ShowDate, s.ShowTime,
               h.HallName, th.TheatreName,
               st.SeatRow, st.SeatNumber,
               NVL(p.Amount, 0)
        FROM   UserTable u
               JOIN Booking b  ON b.UserID    = u.UserID
               JOIN Ticket  t  ON t.BookingID = b.BookingID
               JOIN Show    s  ON b.ShowID    = s.ShowID
               JOIN Movie   m  ON s.MovieID   = m.MovieID
               JOIN Hall    h  ON s.HallID    = h.HallID
               JOIN Theatre th ON h.TheatreID = th.TheatreID
               LEFT JOIN Seat    st ON t.SeatID  = st.SeatID
               LEFT JOIN Pricing p  ON t.PriceID = p.PriceID
        WHERE  u.UserID = :userId
          AND  b.ReservationDate >= ADD_MONTHS(SYSDATE, -6)
        ORDER BY b.ReservationDate DESC, t.TicketID DESC";

    public UserTicketReport? GetUserTicketReport(int userId)
    {
        UserTicketReport? report = null;

        using var conn = OpenConnection();
        using var cmd = new OracleCommand(UserTicketSql, conn);
        cmd.Parameters.Add(Param("userId", userId));
        using var r = cmd.ExecuteReader();

        while (r.Read())
        {
            // First row initialises the report header (user details)
            report ??= new UserTicketReport
            {
                UserID = r.GetInt32(0),
                UserName = r.GetString(1),
                ContactNumber = r.GetString(2),
                Email = r.IsDBNull(3) ? string.Empty : r.GetString(3),
                Address = r.IsDBNull(4) ? null : r.GetString(4)
            };

            var amount = r.GetDecimal(21);

            report.Tickets.Add(new UserTicketRow
            {
                TicketID = r.GetInt32(5),
                TicketNumber = r.IsDBNull(6) ? string.Empty : r.GetString(6),
                TicketStatus = r.IsDBNull(7) ? string.Empty : r.GetString(7),
                ConfirmationNumber = r.IsDBNull(8) ? string.Empty : r.GetString(8),
                BookingTime = r.IsDBNull(9) ? DateTime.MinValue : r.GetDateTime(9),
                ReservationDate = r.IsDBNull(10) ? DateTime.MinValue : r.GetDateTime(10),
                MovieTitle = r.GetString(11),
                Genre = r.IsDBNull(12) ? string.Empty : r.GetString(12),
                Language = r.IsDBNull(13) ? string.Empty : r.GetString(13),
                Duration = r.IsDBNull(14) ? 0 : r.GetInt32(14),
                ShowDate = r.GetDateTime(15),
                ShowTime = r.GetDateTime(16),
                HallName = r.GetString(17),
                TheatreName = r.GetString(18),
                SeatRow = r.IsDBNull(19) ? string.Empty : r.GetString(19),
                SeatNumber = r.IsDBNull(20) ? 0 : Convert.ToInt32(r.GetValue(20)),
                Amount = amount
            });

            report.TotalSpent += amount;
        }

        if (report != null)
            report.TotalTickets = report.Tickets.Count;

        return report;
    }

    // ────────────────────────────────────────────────────
    //  Theatre dropdown for the TheaterCityHall report
    // ────────────────────────────────────────────────────
    public List<SelectListItem> GetTheatreOptions() => Query(
        "SELECT TheatreID, TheatreName || ' (' || TheatreCity || ')' FROM Theatre ORDER BY TheatreName",
        r => new SelectListItem
        {
            Value = r.GetInt32(0).ToString(),
            Text = r.IsDBNull(1) ? $"Theatre #{r.GetInt32(0)}" : r.GetString(1)
        });

    // ────────────────────────────────────────────────────
    //  Complex query: TheaterCityHall Movie Report
    // ────────────────────────────────────────────────────
    //
    //  SQL QUERY (Oracle 21c):
    //  ---------------------------------------------------------
    //  SELECT th.TheatreID, th.TheatreName, th.TheatreCity, th.TheatreAddress, th.TheatreContactNumber,
    //         s.ShowID, s.ShowDate, s.ShowTime, s.StartTime, s.EndTime,
    //         m.Title, m.Genre, m.Language, m.Duration, m.ReleaseDate,
    //         h.HallName, h.HallCapacity, h.ExperienceType, h.ScreenSize
    //  FROM   Theatre th
    //         JOIN Hall h ON h.TheatreID = th.TheatreID
    //         JOIN Show s ON s.HallID    = h.HallID
    //         JOIN Movie m ON s.MovieID  = m.MovieID
    //  WHERE  th.TheatreID = :theatreId
    //  ORDER BY s.ShowDate DESC, s.ShowTime, h.HallName
    //  ---------------------------------------------------------

    private const string TheaterMovieSql = @"
        SELECT th.TheatreID, th.TheatreName, th.TheatreCity, th.TheatreAddress, th.TheatreContactNumber,
               s.ShowID, s.ShowDate, s.ShowTime, s.StartTime, s.EndTime,
               m.Title, m.Genre, m.Language, m.Duration, m.ReleaseDate,
               h.HallName, h.HallCapacity, h.ExperienceType, h.ScreenSize
        FROM   Theatre th
               JOIN Hall h ON h.TheatreID = th.TheatreID
               JOIN Show s ON s.HallID    = h.HallID
               JOIN Movie m ON s.MovieID  = m.MovieID
        WHERE  th.TheatreID = :theatreId
        ORDER BY s.ShowDate DESC, s.ShowTime, h.HallName";

    public TheaterMovieReport? GetTheaterMovieReport(int theatreId)
    {
        TheaterMovieReport? report = null;

        using var conn = OpenConnection();
        using var cmd = new OracleCommand(TheaterMovieSql, conn);
        cmd.Parameters.Add(Param("theatreId", theatreId));
        using var r = cmd.ExecuteReader();

        while (r.Read())
        {
            report ??= new TheaterMovieReport
            {
                TheatreID = r.GetInt32(0),
                TheatreName = r.GetString(1),
                TheatreCity = r.IsDBNull(2) ? string.Empty : r.GetString(2),
                TheatreAddress = r.IsDBNull(3) ? string.Empty : r.GetString(3),
                TheatreContactNumber = r.IsDBNull(4) ? string.Empty : r.GetString(4)
            };

            report.Shows.Add(new TheaterMovieRow
            {
                ShowID = r.GetInt32(5),
                ShowDate = r.GetDateTime(6),
                ShowTime = r.GetDateTime(7),
                StartTime = r.IsDBNull(8) ? null : r.GetDateTime(8),
                EndTime = r.IsDBNull(9) ? null : r.GetDateTime(9),
                MovieTitle = r.GetString(10),
                Genre = r.IsDBNull(11) ? string.Empty : r.GetString(11),
                Language = r.IsDBNull(12) ? string.Empty : r.GetString(12),
                Duration = r.IsDBNull(13) ? 0 : r.GetInt32(13),
                ReleaseDate = r.IsDBNull(14) ? null : r.GetDateTime(14),
                HallName = r.GetString(15),
                HallCapacity = r.IsDBNull(16) ? 0 : r.GetInt32(16),
                ExperienceType = r.IsDBNull(17) ? string.Empty : r.GetString(17),
                ScreenSize = r.IsDBNull(18) ? string.Empty : r.GetString(18)
            });
        }

        if (report != null)
        {
            report.TotalShows = report.Shows.Count;
            report.UniqueMovies = report.Shows.Select(s => s.MovieTitle).Distinct().Count();
        }

        return report;
    }
}
