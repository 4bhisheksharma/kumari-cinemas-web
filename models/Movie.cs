public class Movie
{
    public int MovieID { get; set; }
    public required string Title { get; set; }
    public string Genre { get; set; } = string.Empty;
    public string Duration { get; set; } = string.Empty;
    public string Certificate { get; set; } = string.Empty;
    public string Language { get; set; } = string.Empty;
    public string Status { get; set; } = "Active";
}
