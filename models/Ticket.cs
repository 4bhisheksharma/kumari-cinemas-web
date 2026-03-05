public class Ticket
{
    public int TicketID { get; set; }
    public string RefNumber { get; set; } = string.Empty;
    public string MovieTitle { get; set; } = string.Empty;
    public DateTime ShowDateTime { get; set; }
    public string ScreenName { get; set; } = string.Empty;
    public string Seats { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public string Status { get; set; } = "Confirmed";
}
