public class Schedule
{
    public int ScheduleID { get; set; }
    public string MovieTitle { get; set; } = string.Empty;
    public string ScreenName { get; set; } = string.Empty;
    public DateTime ShowDate { get; set; }
    public string StartTime { get; set; } = string.Empty;
    public string EndTime { get; set; } = string.Empty;
    public decimal TicketPrice { get; set; }
    public string Status { get; set; } = "Scheduled";
}
