using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using KumariCinemas.Web.Services;
using KumariCinemas.Web.Models;

namespace KumariCinemas.Web.Pages.Reports;

public class MovieOccupancyModel : PageModel
{
    private readonly ReportService _reportService;

    public MovieOccupancyModel(ReportService reportService)
    {
        _reportService = reportService;
    }

    [BindProperty(SupportsGet = true)]
    public int? SelectedMovieId { get; set; }

    public List<SelectListItem> MovieOptions { get; set; } = new();

    public MovieOccupancyReport? Report { get; set; }

    public void OnGet()
    {
        MovieOptions = _reportService.GetMovieOptions();

        if (SelectedMovieId.HasValue && SelectedMovieId.Value > 0)
        {
            Report = _reportService.GetMovieOccupancyReport(SelectedMovieId.Value);
        }
    }

    public IActionResult OnPost()
    {
        MovieOptions = _reportService.GetMovieOptions();

        if (SelectedMovieId.HasValue && SelectedMovieId.Value > 0)
        {
            Report = _reportService.GetMovieOccupancyReport(SelectedMovieId.Value);
            if (Report == null)
                TempData["Info"] = "No data found for this movie.";
            else if (Report.TopTheatres.Count == 0)
                TempData["Info"] = "No paid ticket data available for this movie yet.";
        }

        return Page();
    }
}
