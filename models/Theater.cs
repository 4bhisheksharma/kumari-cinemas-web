public class Theater
{
    public int TheatreID { get; set; }
    public required string TheatreName { get; set; }
    public required string TheatreCity { get; set; }
    public string TheatreAddress { get; set; } = string.Empty;
    public string TheatreContactNumber { get; set; } = string.Empty;
}
