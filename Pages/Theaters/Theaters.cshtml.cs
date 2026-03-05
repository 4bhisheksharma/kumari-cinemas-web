using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using KumariCinemas.Web.Services;

namespace KumariCinemas.Web.Pages.Theaters;

public class TheatersModel : PageModel
{
    private readonly TheaterService _theaterService;
    public List<Theater> Theaters { get; set; } = new();

    [BindProperty]
    public Theater TheaterInput { get; set; } = new() { TheatreName = "", TheatreCity = "" };

    [BindProperty(SupportsGet = false)]
    public int DeleteId { get; set; }

    public TheatersModel(TheaterService theaterService)
    {
        _theaterService = theaterService;
    }

    public void OnGet()
    {
        Theaters = _theaterService.GetAllTheaters();
    }

    public IActionResult OnPostCreate()
    {
        try
        {
            _theaterService.CreateTheater(TheaterInput);
            TempData["Success"] = "Theatre created successfully.";
        }
        catch (Exception ex)
        {
            TempData["Error"] = "Failed to create theatre: " + ex.Message;
        }
        return RedirectToPage();
    }

    public IActionResult OnPostUpdate()
    {
        try
        {
            _theaterService.UpdateTheater(TheaterInput);
            TempData["Success"] = "Theatre updated successfully.";
        }
        catch (Exception ex)
        {
            TempData["Error"] = "Failed to update theatre: " + ex.Message;
        }
        return RedirectToPage();
    }

    public IActionResult OnPostDelete()
    {
        try
        {
            _theaterService.DeleteTheater(DeleteId);
            TempData["Success"] = "Theatre deleted successfully.";
        }
        catch (Exception ex)
        {
            TempData["Error"] = "Failed to delete theatre: " + ex.Message;
        }
        return RedirectToPage();
    }
}
