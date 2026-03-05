public class Ticket
{
    public int TicketID { get; set; }
    public int BookingID { get; set; }
    public int SeatID { get; set; }
    public int PriceID { get; set; }
    public string TicketNumber { get; set; } = string.Empty;
    public string TicketStatus { get; set; } = "Booked";
    // Display fields populated from JOINs
    public string MovieTitle { get; set; } = string.Empty;
    public string SeatRow { get; set; } = string.Empty;
    public int SeatNumber { get; set; }
    public decimal Amount { get; set; }
    public string HallName { get; set; } = string.Empty;
    public DateTime ShowDate { get; set; }
    public string UserName { get; set; } = string.Empty;
    public string ConfirmationNumber { get; set; } = string.Empty;
}
