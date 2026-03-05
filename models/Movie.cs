public class Movie
{
    public int MovieID { get; set; }
    public required string Title { get; set; }
    public int Duration { get; set; }               // minutes (NUMBER(4))
    public string Genre { get; set; } = string.Empty;
    public string Language { get; set; } = string.Empty;
    public DateTime? ReleaseDate { get; set; }
}
