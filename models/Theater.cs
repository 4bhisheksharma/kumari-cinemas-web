public class Theater
{
    public int TheaterID { get; set; }
    public required string ScreenName { get; set; }
    public string HallName { get; set; } = string.Empty;
    public string TheaterType { get; set; } = "Standard";
    public int SeatRows { get; set; }
    public int SeatColumns { get; set; }
    public int TotalSeats { get; set; }
    public string Status { get; set; } = "Active";
}
