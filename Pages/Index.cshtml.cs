using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using KumariCinemas.Web.Models;
using KumariCinemas.Web.Services;

namespace KumariCinemas.Web.Pages;

public class IndexModel : PageModel
{
    private readonly ReportService _reportService;

    public int Movies { get; set; }
    public int ShowsToday { get; set; }
    public int Halls { get; set; }
    public int TicketsSoldToday { get; set; }
    public List<DashboardShowRow> TodaysShows { get; set; } = new();

    public IndexModel(ReportService reportService)
    {
        _reportService = reportService;
    }

    public void OnGet()
    {
        var stats = _reportService.GetDashboardStats();
        Movies = stats.Movies;
        ShowsToday = stats.ShowsToday;
        Halls = stats.Halls;
        TicketsSoldToday = stats.TicketsSoldToday;
        TodaysShows = _reportService.GetTodaysShows();
    }
}
