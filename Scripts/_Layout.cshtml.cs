using Microsoft.AspNetCore.Mvc.RazorPages;

namespace WebApplication1.Pages;

public class LayoutModel : PageModel
{
    public string Username {get => HttpContext.Session.GetString("username")?? "LogIn";}

    public void OnGet()
    {
    }
}

