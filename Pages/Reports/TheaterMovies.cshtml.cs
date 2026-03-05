using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using KumariCinemas.Web.Services;
using KumariCinemas.Web.Models;

namespace KumariCinemas.Web.Pages.Reports;

public class TheaterMoviesModel : PageModel
{
    private readonly ReportService _reportService;

    public TheaterMoviesModel(ReportService reportService)
    {
        _reportService = reportService;
    }

    [BindProperty(SupportsGet = true)]
    public int? SelectedTheatreId { get; set; }

    public List<SelectListItem> TheatreOptions { get; set; } = new();

    public TheaterMovieReport? Report { get; set; }

    public void OnGet()
    {
        TheatreOptions = _reportService.GetTheatreOptions();

        if (SelectedTheatreId.HasValue && SelectedTheatreId.Value > 0)
        {
            Report = _reportService.GetTheaterMovieReport(SelectedTheatreId.Value);
        }
    }

    public IActionResult OnPost()
    {
        TheatreOptions = _reportService.GetTheatreOptions();

        if (SelectedTheatreId.HasValue && SelectedTheatreId.Value > 0)
        {
            Report = _reportService.GetTheaterMovieReport(SelectedTheatreId.Value);
            if (Report == null)
                TempData["Info"] = "No shows found for this theatre.";
        }

        return Page();
    }
}
