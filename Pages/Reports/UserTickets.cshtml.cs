using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using KumariCinemas.Web.Services;
using KumariCinemas.Web.Models;

namespace KumariCinemas.Web.Pages.Reports;

public class UserTicketsModel : PageModel
{
    private readonly ReportService _reportService;

    public UserTicketsModel(ReportService reportService)
    {
        _reportService = reportService;
    }

    // ── Form input ──
    [BindProperty(SupportsGet = true)]
    public int? SelectedUserId { get; set; }

    // ── Dropdown ──
    public List<SelectListItem> UserOptions { get; set; } = new();

    // ── Report result (null = not yet searched) ──
    public UserTicketReport? Report { get; set; }

    public void OnGet()
    {
        UserOptions = _reportService.GetUserOptions();

        if (SelectedUserId.HasValue && SelectedUserId.Value > 0)
        {
            Report = _reportService.GetUserTicketReport(SelectedUserId.Value);
        }
    }

    public IActionResult OnPost()
    {
        UserOptions = _reportService.GetUserOptions();

        if (SelectedUserId.HasValue && SelectedUserId.Value > 0)
        {
            Report = _reportService.GetUserTicketReport(SelectedUserId.Value);
            if (Report == null)
                TempData["Info"] = "No tickets found for this user in the last 6 months.";
        }

        return Page();
    }
}
