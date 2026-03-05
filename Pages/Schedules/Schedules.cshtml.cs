using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using KumariCinemas.Web.Services;

namespace KumariCinemas.Web.Pages.Schedules;

public class SchedulesModel : PageModel
{
    private readonly ScheduleService _scheduleService;
    public List<Schedule> Schedules { get; set; } = new();
    public List<SelectListItem> MovieOptions { get; set; } = new();
    public List<SelectListItem> HallOptions { get; set; } = new();

    [BindProperty]
    public Schedule ScheduleInput { get; set; } = new();

    [BindProperty(SupportsGet = false)]
    public int DeleteId { get; set; }

    public SchedulesModel(ScheduleService scheduleService)
    {
        _scheduleService = scheduleService;
    }

    public void OnGet()
    {
        Schedules = _scheduleService.GetAllSchedules();
        MovieOptions = _scheduleService.GetMovieOptions();
        HallOptions = _scheduleService.GetHallOptions();
    }

    public IActionResult OnPostCreate()
    {
        try
        {
            _scheduleService.CreateSchedule(ScheduleInput);
            TempData["Success"] = "Show created successfully.";
        }
        catch (Exception ex)
        {
            TempData["Error"] = "Failed to create show: " + ex.Message;
        }
        return RedirectToPage();
    }

    public IActionResult OnPostUpdate()
    {
        try
        {
            _scheduleService.UpdateSchedule(ScheduleInput);
            TempData["Success"] = "Show updated successfully.";
        }
        catch (Exception ex)
        {
            TempData["Error"] = "Failed to update show: " + ex.Message;
        }
        return RedirectToPage();
    }

    public IActionResult OnPostDelete()
    {
        try
        {
            _scheduleService.DeleteSchedule(DeleteId);
            TempData["Success"] = "Show deleted successfully.";
        }
        catch (Exception ex)
        {
            TempData["Error"] = "Failed to delete show: " + ex.Message;
        }
        return RedirectToPage();
    }
}
