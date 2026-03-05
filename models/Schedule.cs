public class Schedule
{
    public int ShowID { get; set; }
    public DateTime ShowDate { get; set; }
    public DateTime ShowTime { get; set; }          // TIMESTAMP NOT NULL
    public DateTime? StartTime { get; set; }        // TIMESTAMP nullable
    public DateTime? EndTime { get; set; }          // TIMESTAMP nullable
    public int MovieID { get; set; }
    public int HallID { get; set; }
    // Display fields populated from JOINs
    public string MovieTitle { get; set; } = string.Empty;
    public string HallName { get; set; } = string.Empty;
    public string TheatreName { get; set; } = string.Empty;
}
