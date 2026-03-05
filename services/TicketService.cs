using Oracle.ManagedDataAccess.Client;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace KumariCinemas.Web.Services;

public class TicketService : BaseService
{
    public TicketService(DatabaseConnHelper dbHelper) : base(dbHelper) { }

    public List<Ticket> GetAllTickets() => Query(
        @"SELECT t.TicketID, t.BookingID, t.SeatID, t.PriceID, t.TicketNumber, t.TicketStatus,
                 m.Title, st.SeatRow, st.SeatNumber, NVL(p.Amount, 0),
                 h.HallName, s.ShowDate, u.UserName, b.ConfirmationNumber
          FROM Ticket t
          LEFT JOIN Booking b  ON t.BookingID = b.BookingID
          LEFT JOIN UserTable u ON b.UserID   = u.UserID
          LEFT JOIN Show s     ON b.ShowID    = s.ShowID
          LEFT JOIN Movie m    ON s.MovieID   = m.MovieID
          LEFT JOIN Hall h     ON s.HallID    = h.HallID
          LEFT JOIN Seat st    ON t.SeatID    = st.SeatID
          LEFT JOIN Pricing p  ON t.PriceID   = p.PriceID
          ORDER BY t.TicketID DESC",
        r => new Ticket
        {
            TicketID = r.GetInt32(0),
            BookingID = r.IsDBNull(1) ? 0 : r.GetInt32(1),
            SeatID = r.IsDBNull(2) ? 0 : r.GetInt32(2),
            PriceID = r.IsDBNull(3) ? 0 : r.GetInt32(3),
            TicketNumber = r.IsDBNull(4) ? string.Empty : r.GetString(4),
            TicketStatus = r.IsDBNull(5) ? "Booked" : r.GetString(5),
            MovieTitle = r.IsDBNull(6) ? string.Empty : r.GetString(6),
            SeatRow = r.IsDBNull(7) ? string.Empty : r.GetString(7),
            SeatNumber = r.IsDBNull(8) ? 0 : Convert.ToInt32(r.GetValue(8)),
            Amount = r.GetDecimal(9),
            HallName = r.IsDBNull(10) ? string.Empty : r.GetString(10),
            ShowDate = r.IsDBNull(11) ? DateTime.MinValue : r.GetDateTime(11),
            UserName = r.IsDBNull(12) ? string.Empty : r.GetString(12),
            ConfirmationNumber = r.IsDBNull(13) ? string.Empty : r.GetString(13)
        });

    public List<SelectListItem> GetBookingOptions() => Query(
        @"SELECT b.BookingID, b.ConfirmationNumber || ' - ' || u.UserName
          FROM Booking b LEFT JOIN UserTable u ON b.UserID = u.UserID
          ORDER BY b.BookingID DESC",
        r => new SelectListItem
        {
            Value = r.GetInt32(0).ToString(),
            Text = r.IsDBNull(1) ? $"Booking #{r.GetInt32(0)}" : r.GetString(1)
        });

    public List<SelectListItem> GetSeatOptions() => Query(
        @"SELECT s.SeatID, 'Row ' || s.SeatRow || ' Seat ' || s.SeatNumber || ' (' || h.HallName || ')'
          FROM Seat s LEFT JOIN Hall h ON s.HallID = h.HallID
          ORDER BY h.HallName, s.SeatRow, s.SeatNumber",
        r => new SelectListItem
        {
            Value = r.GetInt32(0).ToString(),
            Text = r.IsDBNull(1) ? $"Seat #{r.GetInt32(0)}" : r.GetString(1)
        });

    public List<SelectListItem> GetPricingOptions() => Query(
        @"SELECT p.PriceID, 'Rs. ' || p.Amount || ' (Show #' || p.ShowID || ')'
          FROM Pricing p ORDER BY p.PriceID",
        r => new SelectListItem
        {
            Value = r.GetInt32(0).ToString(),
            Text = r.IsDBNull(1) ? $"Price #{r.GetInt32(0)}" : r.GetString(1)
        });

    public void CreateTicket(Ticket t) => Execute(
        @"INSERT INTO Ticket (TicketID, BookingID, SeatID, PriceID, TicketNumber, TicketStatus)
          VALUES ((SELECT NVL(MAX(TicketID),0)+1 FROM Ticket), :bookingID, :seatID, :priceID, :ticketNumber, :ticketStatus)",
        Param("bookingID", t.BookingID), Param("seatID", t.SeatID),
        Param("priceID", t.PriceID), Param("ticketNumber", t.TicketNumber),
        Param("ticketStatus", t.TicketStatus));

    public void UpdateTicket(Ticket t) => Execute(
        @"UPDATE Ticket SET BookingID=:bookingID, SeatID=:seatID, PriceID=:priceID,
          TicketNumber=:ticketNumber, TicketStatus=:ticketStatus WHERE TicketID=:id",
        Param("bookingID", t.BookingID), Param("seatID", t.SeatID),
        Param("priceID", t.PriceID), Param("ticketNumber", t.TicketNumber),
        Param("ticketStatus", t.TicketStatus), Param("id", t.TicketID));

    public void DeleteTicket(int ticketId) => Execute(
        "DELETE FROM Ticket WHERE TicketID = :id",
        Param("id", ticketId));
}
