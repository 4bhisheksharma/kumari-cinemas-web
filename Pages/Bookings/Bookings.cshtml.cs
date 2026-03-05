using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using KumariCinemas.Web.Services;

namespace KumariCinemas.Web.Pages.Bookings;

public class BookingsModel : PageModel
{
    private readonly BookingService _bookingService;
    public List<Booking> Bookings { get; set; } = new();
    public List<SelectListItem> UserOptions { get; set; } = new();
    public List<SelectListItem> ShowOptions { get; set; } = new();

    [BindProperty]
    public Booking BookingInput { get; set; } = new();

    [BindProperty(SupportsGet = false)]
    public int DeleteId { get; set; }

    public BookingsModel(BookingService bookingService)
    {
        _bookingService = bookingService;
    }

    public void OnGet()
    {
        Bookings = _bookingService.GetAllBookings();
        UserOptions = _bookingService.GetUserOptions();
        ShowOptions = _bookingService.GetShowOptions();
    }

    public IActionResult OnPostCreate()
    {
        try
        {
            _bookingService.CreateBooking(BookingInput);
            TempData["Success"] = "Booking created successfully.";
        }
        catch (Exception ex)
        {
            TempData["Error"] = "Failed to create booking: " + ex.Message;
        }
        return RedirectToPage();
    }

    public IActionResult OnPostUpdate()
    {
        try
        {
            _bookingService.UpdateBooking(BookingInput);
            TempData["Success"] = "Booking updated successfully.";
        }
        catch (Exception ex)
        {
            TempData["Error"] = "Failed to update booking: " + ex.Message;
        }
        return RedirectToPage();
    }

    public IActionResult OnPostDelete()
    {
        try
        {
            _bookingService.DeleteBooking(DeleteId);
            TempData["Success"] = "Booking deleted successfully.";
        }
        catch (Exception ex)
        {
            TempData["Error"] = "Failed to delete booking: " + ex.Message;
        }
        return RedirectToPage();
    }
}
