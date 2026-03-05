using Oracle.ManagedDataAccess.Client;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace KumariCinemas.Web.Services;

public class TicketService
{
    private readonly DatabaseConnHelper _dbHelper;

    public TicketService(DatabaseConnHelper dbHelper)
    {
        _dbHelper = dbHelper;
    }

    public List<Ticket> GetAllTickets()
    {
        var tickets = new List<Ticket>();

        using var conn = _dbHelper.GetConnection();
        conn.Open();
        using var cmd = new OracleCommand(@"
            SELECT t.TicketID, t.BookingID, t.SeatID, t.PriceID, t.TicketNumber, t.TicketStatus,
                   m.Title, st.SeatRow, st.SeatNumber, NVL(p.Amount, 0),
                   h.HallName, s.ShowDate, u.UserName, b.ConfirmationNumber
            FROM Ticket t
            LEFT JOIN Booking b ON t.BookingID = b.BookingID
            LEFT JOIN UserTable u ON b.UserID = u.UserID
            LEFT JOIN Show s ON b.ShowID = s.ShowID
            LEFT JOIN Movie m ON s.MovieID = m.MovieID
            LEFT JOIN Hall h ON s.HallID = h.HallID
            LEFT JOIN Seat st ON t.SeatID = st.SeatID
            LEFT JOIN Pricing p ON t.PriceID = p.PriceID
            ORDER BY t.TicketID DESC", conn);
        using var reader = cmd.ExecuteReader();

        while (reader.Read())
        {
            tickets.Add(new Ticket
            {
                TicketID = reader.GetInt32(0),
                BookingID = reader.IsDBNull(1) ? 0 : reader.GetInt32(1),
                SeatID = reader.IsDBNull(2) ? 0 : reader.GetInt32(2),
                PriceID = reader.IsDBNull(3) ? 0 : reader.GetInt32(3),
                TicketNumber = reader.IsDBNull(4) ? string.Empty : reader.GetString(4),
                TicketStatus = reader.IsDBNull(5) ? "Booked" : reader.GetString(5),
                MovieTitle = reader.IsDBNull(6) ? string.Empty : reader.GetString(6),
                SeatRow = reader.IsDBNull(7) ? string.Empty : reader.GetString(7),
                SeatNumber = reader.IsDBNull(8) ? 0 : Convert.ToInt32(reader.GetValue(8)),
                Amount = reader.GetDecimal(9),
                HallName = reader.IsDBNull(10) ? string.Empty : reader.GetString(10),
                ShowDate = reader.IsDBNull(11) ? DateTime.MinValue : reader.GetDateTime(11),
                UserName = reader.IsDBNull(12) ? string.Empty : reader.GetString(12),
                ConfirmationNumber = reader.IsDBNull(13) ? string.Empty : reader.GetString(13)
            });
        }

        return tickets;
    }

    public List<SelectListItem> GetBookingOptions()
    {
        var items = new List<SelectListItem>();
        using var conn = _dbHelper.GetConnection();
        conn.Open();
        using var cmd = new OracleCommand(@"
            SELECT b.BookingID, b.ConfirmationNumber || ' - ' || u.UserName
            FROM Booking b
            LEFT JOIN UserTable u ON b.UserID = u.UserID
            ORDER BY b.BookingID DESC", conn);
        using var reader = cmd.ExecuteReader();
        while (reader.Read())
        {
            items.Add(new SelectListItem
            {
                Value = reader.GetInt32(0).ToString(),
                Text = reader.IsDBNull(1) ? $"Booking #{reader.GetInt32(0)}" : reader.GetString(1)
            });
        }
        return items;
    }

    public List<SelectListItem> GetSeatOptions()
    {
        var items = new List<SelectListItem>();
        using var conn = _dbHelper.GetConnection();
        conn.Open();
        using var cmd = new OracleCommand(@"
            SELECT s.SeatID, 'Row ' || s.SeatRow || ' Seat ' || s.SeatNumber || ' (' || h.HallName || ')'
            FROM Seat s
            LEFT JOIN Hall h ON s.HallID = h.HallID
            ORDER BY h.HallName, s.SeatRow, s.SeatNumber", conn);
        using var reader = cmd.ExecuteReader();
        while (reader.Read())
        {
            items.Add(new SelectListItem
            {
                Value = reader.GetInt32(0).ToString(),
                Text = reader.IsDBNull(1) ? $"Seat #{reader.GetInt32(0)}" : reader.GetString(1)
            });
        }
        return items;
    }

    public List<SelectListItem> GetPricingOptions()
    {
        var items = new List<SelectListItem>();
        using var conn = _dbHelper.GetConnection();
        conn.Open();
        using var cmd = new OracleCommand(@"
            SELECT p.PriceID, 'Rs. ' || p.Amount || ' (Show #' || p.ShowID || ')'
            FROM Pricing p
            ORDER BY p.PriceID", conn);
        using var reader = cmd.ExecuteReader();
        while (reader.Read())
        {
            items.Add(new SelectListItem
            {
                Value = reader.GetInt32(0).ToString(),
                Text = reader.IsDBNull(1) ? $"Price #{reader.GetInt32(0)}" : reader.GetString(1)
            });
        }
        return items;
    }

    public void CreateTicket(Ticket ticket)
    {
        using var conn = _dbHelper.GetConnection();
        conn.Open();
        using var cmd = new OracleCommand(@"
            INSERT INTO Ticket (TicketID, BookingID, SeatID, PriceID, TicketNumber, TicketStatus) 
            VALUES ((SELECT NVL(MAX(TicketID),0)+1 FROM Ticket), :bookingID, :seatID, :priceID, :ticketNumber, :ticketStatus)", conn);
        cmd.Parameters.Add(new OracleParameter("bookingID", ticket.BookingID));
        cmd.Parameters.Add(new OracleParameter("seatID", ticket.SeatID));
        cmd.Parameters.Add(new OracleParameter("priceID", ticket.PriceID));
        cmd.Parameters.Add(new OracleParameter("ticketNumber", ticket.TicketNumber));
        cmd.Parameters.Add(new OracleParameter("ticketStatus", ticket.TicketStatus));
        cmd.ExecuteNonQuery();
    }

    public void UpdateTicket(Ticket ticket)
    {
        using var conn = _dbHelper.GetConnection();
        conn.Open();
        using var cmd = new OracleCommand(@"
            UPDATE Ticket SET BookingID = :bookingID, SeatID = :seatID, PriceID = :priceID, 
            TicketNumber = :ticketNumber, TicketStatus = :ticketStatus WHERE TicketID = :id", conn);
        cmd.Parameters.Add(new OracleParameter("bookingID", ticket.BookingID));
        cmd.Parameters.Add(new OracleParameter("seatID", ticket.SeatID));
        cmd.Parameters.Add(new OracleParameter("priceID", ticket.PriceID));
        cmd.Parameters.Add(new OracleParameter("ticketNumber", ticket.TicketNumber));
        cmd.Parameters.Add(new OracleParameter("ticketStatus", ticket.TicketStatus));
        cmd.Parameters.Add(new OracleParameter("id", ticket.TicketID));
        cmd.ExecuteNonQuery();
    }

    public void DeleteTicket(int ticketId)
    {
        using var conn = _dbHelper.GetConnection();
        conn.Open();
        using var cmd = new OracleCommand("DELETE FROM Ticket WHERE TicketID = :id", conn);
        cmd.Parameters.Add(new OracleParameter("id", ticketId));
        cmd.ExecuteNonQuery();
    }
}
