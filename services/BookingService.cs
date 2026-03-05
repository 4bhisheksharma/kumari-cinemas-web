using Oracle.ManagedDataAccess.Client;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace KumariCinemas.Web.Services;

public class BookingService : BaseService
{
    public BookingService(DatabaseConnHelper dbHelper) : base(dbHelper) { }

    public List<Booking> GetAllBookings() => Query(
        @"SELECT b.BookingID, b.UserID, b.ShowID, b.BookingTime, b.ReservationDate, b.ConfirmationNumber,
                 u.UserName, m.Title, h.HallName, t.TheatreName, s.ShowDate, s.ShowTime
          FROM Booking b
          LEFT JOIN UserTable u ON b.UserID  = u.UserID
          LEFT JOIN Show s      ON b.ShowID  = s.ShowID
          LEFT JOIN Movie m     ON s.MovieID = m.MovieID
          LEFT JOIN Hall h      ON s.HallID  = h.HallID
          LEFT JOIN Theatre t   ON h.TheatreID = t.TheatreID
          ORDER BY b.BookingID DESC",
        r => new Booking
        {
            BookingID = r.GetInt32(0),
            UserID = r.IsDBNull(1) ? 0 : r.GetInt32(1),
            ShowID = r.IsDBNull(2) ? 0 : r.GetInt32(2),
            BookingTime = r.IsDBNull(3) ? null : r.GetDateTime(3),
            ReservationDate = r.IsDBNull(4) ? null : r.GetDateTime(4),
            ConfirmationNumber = r.IsDBNull(5) ? string.Empty : r.GetString(5),
            UserName = r.IsDBNull(6) ? string.Empty : r.GetString(6),
            MovieTitle = r.IsDBNull(7) ? string.Empty : r.GetString(7),
            HallName = r.IsDBNull(8) ? string.Empty : r.GetString(8),
            TheatreName = r.IsDBNull(9) ? string.Empty : r.GetString(9),
            ShowDate = r.IsDBNull(10) ? null : r.GetDateTime(10),
            ShowTime = r.IsDBNull(11) ? null : r.GetDateTime(11)
        });

    public List<SelectListItem> GetUserOptions() => Query(
        "SELECT UserID, UserName || ' (' || ContactNumber || ')' FROM UserTable ORDER BY UserName",
        r => new SelectListItem
        {
            Value = r.GetInt32(0).ToString(),
            Text = r.IsDBNull(1) ? $"User #{r.GetInt32(0)}" : r.GetString(1)
        });

    public List<SelectListItem> GetShowOptions() => Query(
        @"SELECT s.ShowID, m.Title || ' | ' || TO_CHAR(s.ShowDate,'YYYY-MM-DD') || ' ' || TO_CHAR(s.ShowTime,'HH24:MI') || ' (' || h.HallName || ')'
          FROM Show s
          LEFT JOIN Movie m ON s.MovieID = m.MovieID
          LEFT JOIN Hall h  ON s.HallID  = h.HallID
          ORDER BY s.ShowDate DESC, s.ShowTime",
        r => new SelectListItem
        {
            Value = r.GetInt32(0).ToString(),
            Text = r.IsDBNull(1) ? $"Show #{r.GetInt32(0)}" : r.GetString(1)
        });

    public void CreateBooking(Booking b) => Execute(
        @"INSERT INTO Booking (BookingID, UserID, ShowID, BookingTime, ReservationDate, ConfirmationNumber)
          VALUES ((SELECT NVL(MAX(BookingID),0)+1 FROM Booking), :userID, :showID, :bookingTime, :reservationDate, :confirmationNumber)",
        Param("userID", b.UserID), Param("showID", b.ShowID),
        Param("bookingTime", b.BookingTime), Param("reservationDate", b.ReservationDate),
        Param("confirmationNumber", b.ConfirmationNumber));

    public void UpdateBooking(Booking b) => Execute(
        @"UPDATE Booking SET UserID=:userID, ShowID=:showID, BookingTime=:bookingTime,
          ReservationDate=:reservationDate, ConfirmationNumber=:confirmationNumber WHERE BookingID=:id",
        Param("userID", b.UserID), Param("showID", b.ShowID),
        Param("bookingTime", b.BookingTime), Param("reservationDate", b.ReservationDate),
        Param("confirmationNumber", b.ConfirmationNumber), Param("id", b.BookingID));

    public void DeleteBooking(int bookingId) => Execute(
        "DELETE FROM Booking WHERE BookingID = :id",
        Param("id", bookingId));
}
