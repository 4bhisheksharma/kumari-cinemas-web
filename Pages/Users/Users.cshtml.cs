using Microsoft.AspNetCore.Mvc.RazorPages;
using KumariCinemas.Web.Services;

namespace KumariCinemas.Web.Pages.Users;

public class UsersModel : PageModel
{
    private readonly UserService _userService;
    public List<User> Users { get; set; } = new();

    public UsersModel(UserService userService)
    {
        _userService = userService;
    }

    public void OnGet()
    {
        Users = _userService.GetAllUsers();
    }
}