using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using KumariCinemas.Web.Services;

namespace KumariCinemas.Web.Pages.Movies;

public class MoviesModel : PageModel
{
    private readonly MovieService _movieService;
    public List<Movie> Movies { get; set; } = new();

    [BindProperty]
    public Movie MovieInput { get; set; } = new() { Title = "" };

    [BindProperty(SupportsGet = false)]
    public int DeleteId { get; set; }

    public MoviesModel(MovieService movieService)
    {
        _movieService = movieService;
    }

    public void OnGet()
    {
        Movies = _movieService.GetAllMovies();
    }

    public IActionResult OnPostCreate()
    {
        try
        {
            _movieService.CreateMovie(MovieInput);
            TempData["Success"] = "Movie created successfully.";
        }
        catch (Exception ex)
        {
            TempData["Error"] = "Failed to create movie: " + ex.Message;
        }
        return RedirectToPage();
    }

    public IActionResult OnPostUpdate()
    {
        try
        {
            _movieService.UpdateMovie(MovieInput);
            TempData["Success"] = "Movie updated successfully.";
        }
        catch (Exception ex)
        {
            TempData["Error"] = "Failed to update movie: " + ex.Message;
        }
        return RedirectToPage();
    }

    public IActionResult OnPostDelete()
    {
        try
        {
            _movieService.DeleteMovie(DeleteId);
            TempData["Success"] = "Movie deleted successfully.";
        }
        catch (Exception ex)
        {
            TempData["Error"] = "Failed to delete movie: " + ex.Message;
        }
        return RedirectToPage();
    }
}
