using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using KumariCinemas.Web.Services;

namespace KumariCinemas.Web.Pages.Tickets;

public class TicketsModel : PageModel
{
    private readonly TicketService _ticketService;
    public List<Ticket> Tickets { get; set; } = new();
    public List<SelectListItem> BookingOptions { get; set; } = new();
    public List<SelectListItem> SeatOptions { get; set; } = new();
    public List<SelectListItem> PricingOptions { get; set; } = new();

    [BindProperty]
    public Ticket TicketInput { get; set; } = new();

    [BindProperty(SupportsGet = false)]
    public int DeleteId { get; set; }

    public TicketsModel(TicketService ticketService)
    {
        _ticketService = ticketService;
    }

    public void OnGet()
    {
        Tickets = _ticketService.GetAllTickets();
        BookingOptions = _ticketService.GetBookingOptions();
        SeatOptions = _ticketService.GetSeatOptions();
        PricingOptions = _ticketService.GetPricingOptions();
    }

    public IActionResult OnPostCreate()
    {
        try
        {
            _ticketService.CreateTicket(TicketInput);
            TempData["Success"] = "Ticket created successfully.";
        }
        catch (Exception ex)
        {
            TempData["Error"] = "Failed to create ticket: " + ex.Message;
        }
        return RedirectToPage();
    }

    public IActionResult OnPostUpdate()
    {
        try
        {
            _ticketService.UpdateTicket(TicketInput);
            TempData["Success"] = "Ticket updated successfully.";
        }
        catch (Exception ex)
        {
            TempData["Error"] = "Failed to update ticket: " + ex.Message;
        }
        return RedirectToPage();
    }

    public IActionResult OnPostDelete()
    {
        try
        {
            _ticketService.DeleteTicket(DeleteId);
            TempData["Success"] = "Ticket deleted successfully.";
        }
        catch (Exception ex)
        {
            TempData["Error"] = "Failed to delete ticket: " + ex.Message;
        }
        return RedirectToPage();
    }
}
