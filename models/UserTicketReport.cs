namespace KumariCinemas.Web.Models;

/// <summary>
/// View-model for the "User Ticket History" complex report.
/// Holds user details plus every ticket purchased in the queried period.
/// </summary>
public class UserTicketReport
{
    // ── User details ──
    public int UserID { get; set; }
    public string UserName { get; set; } = string.Empty;
    public string ContactNumber { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? Address { get; set; }

    // ── Aggregates ──
    public int TotalTickets { get; set; }
    public decimal TotalSpent { get; set; }

    // ── Ticket rows ──
    public List<UserTicketRow> Tickets { get; set; } = new();
}

/// <summary>Single ticket row within the report (populated from a multi-table JOIN).</summary>
public class UserTicketRow
{
    public int TicketID { get; set; }
    public string TicketNumber { get; set; } = string.Empty;
    public string TicketStatus { get; set; } = string.Empty;
    public string ConfirmationNumber { get; set; } = string.Empty;

    // Show / Movie / Hall
    public string MovieTitle { get; set; } = string.Empty;
    public string Genre { get; set; } = string.Empty;
    public string Language { get; set; } = string.Empty;
    public int Duration { get; set; }
    public DateTime ShowDate { get; set; }
    public DateTime ShowTime { get; set; }
    public string HallName { get; set; } = string.Empty;
    public string TheatreName { get; set; } = string.Empty;

    // Seat / Pricing
    public string SeatRow { get; set; } = string.Empty;
    public int SeatNumber { get; set; }
    public decimal Amount { get; set; }

    // Booking
    public DateTime BookingTime { get; set; }
    public DateTime ReservationDate { get; set; }
}
