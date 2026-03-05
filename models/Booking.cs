public class Booking
{
    public int BookingID { get; set; }
    public int UserID { get; set; }
    public int ShowID { get; set; }
    public DateTime? BookingTime { get; set; }
    public DateTime? ReservationDate { get; set; }
    public string ConfirmationNumber { get; set; } = string.Empty;
    // Display fields populated from JOINs
    public string UserName { get; set; } = string.Empty;
    public string MovieTitle { get; set; } = string.Empty;
    public string HallName { get; set; } = string.Empty;
    public string TheatreName { get; set; } = string.Empty;
    public DateTime? ShowDate { get; set; }
    public DateTime? ShowTime { get; set; }
}
