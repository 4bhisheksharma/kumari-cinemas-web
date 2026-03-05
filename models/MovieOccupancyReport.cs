namespace KumariCinemas.Web.Models;

/// <summary>
/// View-model for the "MovieTheaterCityHallOccupancyPerformer" complex report.
/// For any movie, show top 3 theatres with maximum seat occupancy based on paid tickets.
/// </summary>
public class MovieOccupancyReport
{
    public int MovieID { get; set; }
    public string MovieTitle { get; set; } = string.Empty;
    public string Genre { get; set; } = string.Empty;
    public string Language { get; set; } = string.Empty;
    public int Duration { get; set; }
    public int TotalPaidTickets { get; set; }
    public List<MovieOccupancyRow> TopTheatres { get; set; } = new();
}

public class MovieOccupancyRow
{
    public int Rank { get; set; }
    public int TheatreID { get; set; }
    public string TheatreName { get; set; } = string.Empty;
    public string TheatreCity { get; set; } = string.Empty;
    public string HallName { get; set; } = string.Empty;
    public int HallCapacity { get; set; }
    public int TotalShows { get; set; }
    public int TotalSeats { get; set; }        // HallCapacity * TotalShows
    public int PaidTickets { get; set; }
    public decimal OccupancyPercent { get; set; }
}
