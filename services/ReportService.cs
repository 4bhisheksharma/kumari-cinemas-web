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
}
