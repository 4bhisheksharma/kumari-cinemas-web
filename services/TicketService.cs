using Oracle.ManagedDataAccess.Client;

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
        using var cmd = new OracleCommand(
            "SELECT TicketID, RefNumber, MovieTitle, ShowDateTime, ScreenName, Seats, Amount, Status FROM TicketTable ORDER BY TicketID DESC", conn);
        using var reader = cmd.ExecuteReader();

        while (reader.Read())
        {
            tickets.Add(new Ticket
            {
                TicketID = reader.GetInt32(0),
                RefNumber = reader.IsDBNull(1) ? string.Empty : reader.GetString(1),
                MovieTitle = reader.IsDBNull(2) ? string.Empty : reader.GetString(2),
                ShowDateTime = reader.IsDBNull(3) ? DateTime.MinValue : reader.GetDateTime(3),
                ScreenName = reader.IsDBNull(4) ? string.Empty : reader.GetString(4),
                Seats = reader.IsDBNull(5) ? string.Empty : reader.GetString(5),
                Amount = reader.IsDBNull(6) ? 0 : reader.GetDecimal(6),
                Status = reader.IsDBNull(7) ? "Confirmed" : reader.GetString(7)
            });
        }

        return tickets;
    }

    public void CreateTicket(Ticket ticket)
    {
        using var conn = _dbHelper.GetConnection();
        conn.Open();
        using var cmd = new OracleCommand(
            "INSERT INTO TicketTable (RefNumber, MovieTitle, ShowDateTime, ScreenName, Seats, Amount, Status) VALUES (:refNo, :movie, :showDt, :screen, :seats, :amount, :status)", conn);
        cmd.Parameters.Add(new OracleParameter("refNo", ticket.RefNumber));
        cmd.Parameters.Add(new OracleParameter("movie", ticket.MovieTitle));
        cmd.Parameters.Add(new OracleParameter("showDt", ticket.ShowDateTime));
        cmd.Parameters.Add(new OracleParameter("screen", ticket.ScreenName));
        cmd.Parameters.Add(new OracleParameter("seats", ticket.Seats));
        cmd.Parameters.Add(new OracleParameter("amount", ticket.Amount));
        cmd.Parameters.Add(new OracleParameter("status", ticket.Status));
        cmd.ExecuteNonQuery();
    }

    public void UpdateTicket(Ticket ticket)
    {
        using var conn = _dbHelper.GetConnection();
        conn.Open();
        using var cmd = new OracleCommand(
            "UPDATE TicketTable SET RefNumber = :refNo, MovieTitle = :movie, ShowDateTime = :showDt, ScreenName = :screen, Seats = :seats, Amount = :amount, Status = :status WHERE TicketID = :id", conn);
        cmd.Parameters.Add(new OracleParameter("refNo", ticket.RefNumber));
        cmd.Parameters.Add(new OracleParameter("movie", ticket.MovieTitle));
        cmd.Parameters.Add(new OracleParameter("showDt", ticket.ShowDateTime));
        cmd.Parameters.Add(new OracleParameter("screen", ticket.ScreenName));
        cmd.Parameters.Add(new OracleParameter("seats", ticket.Seats));
        cmd.Parameters.Add(new OracleParameter("amount", ticket.Amount));
        cmd.Parameters.Add(new OracleParameter("status", ticket.Status));
        cmd.Parameters.Add(new OracleParameter("id", ticket.TicketID));
        cmd.ExecuteNonQuery();
    }

    public void DeleteTicket(int ticketId)
    {
        using var conn = _dbHelper.GetConnection();
        conn.Open();
        using var cmd = new OracleCommand("DELETE FROM TicketTable WHERE TicketID = :id", conn);
        cmd.Parameters.Add(new OracleParameter("id", ticketId));
        cmd.ExecuteNonQuery();
    }
}
