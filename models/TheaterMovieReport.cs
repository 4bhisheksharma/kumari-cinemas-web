namespace KumariCinemas.Web.Models;

/// <summary>
/// View-model for the "TheaterCityHall Movie" complex report.
/// Shows movie/showtime details for a selected theatre.
/// </summary>
public class TheaterMovieReport
{
    public int TheatreID { get; set; }
    public string TheatreName { get; set; } = string.Empty;
    public string TheatreCity { get; set; } = string.Empty;
    public string TheatreAddress { get; set; } = string.Empty;
    public string TheatreContactNumber { get; set; } = string.Empty;
    public int TotalShows { get; set; }
    public int UniqueMovies { get; set; }
    public List<TheaterMovieRow> Shows { get; set; } = new();
}

public class TheaterMovieRow
{
    public int ShowID { get; set; }
    public DateTime ShowDate { get; set; }
    public DateTime ShowTime { get; set; }
    public DateTime? StartTime { get; set; }
    public DateTime? EndTime { get; set; }
    public string MovieTitle { get; set; } = string.Empty;
    public string Genre { get; set; } = string.Empty;
    public string Language { get; set; } = string.Empty;
    public int Duration { get; set; }
    public DateTime? ReleaseDate { get; set; }
    public string HallName { get; set; } = string.Empty;
    public int HallCapacity { get; set; }
    public string ExperienceType { get; set; } = string.Empty;
    public string ScreenSize { get; set; } = string.Empty;
}
