using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using KumariCinemas.Web.Services;

namespace KumariCinemas.Web.Pages.Users;

public class UsersModel : PageModel
{
    private readonly UserService _userService;
    public List<User> Users { get; set; } = new();

    [BindProperty]
    public User UserInput { get; set; } = new() { UserName = "", ContactNumber = "" };

    [BindProperty(SupportsGet = false)]
    public int DeleteId { get; set; }

    public string? SuccessMessage { get; set; }
    public string? ErrorMessage { get; set; }

    public UsersModel(UserService userService)
    {
        _userService = userService;
    }

    public void OnGet()
    {
        Users = _userService.GetAllUsers();
    }

    public IActionResult OnPostCreate()
    {
        try
        {
            _userService.CreateUser(UserInput);
            TempData["Success"] = "User created successfully.";
        }
        catch (Exception ex)
        {
            TempData["Error"] = "Failed to create user: " + ex.Message;
        }
        return RedirectToPage();
    }

    public IActionResult OnPostUpdate()
    {
        try
        {
            _userService.UpdateUser(UserInput);
            TempData["Success"] = "User updated successfully.";
        }
        catch (Exception ex)
        {
            TempData["Error"] = "Failed to update user: " + ex.Message;
        }
        return RedirectToPage();
    }

    public IActionResult OnPostDelete()
    {
        try
        {
            _userService.DeleteUser(DeleteId);
            TempData["Success"] = "User deleted successfully.";
        }
        catch (Exception ex)
        {
            TempData["Error"] = "Failed to delete user: " + ex.Message;
        }
        return RedirectToPage();
    }
}