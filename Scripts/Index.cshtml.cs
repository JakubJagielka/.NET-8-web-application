using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace WebApplication1.Pages;

public class IndexModel : PageModel
{
    private readonly ILogger<IndexModel> _logger;


    public string[][] bots = new string[][] {
        new string[] {"Marcus Aurelius","A roman emperor, stoic, a virtual man and a wise man.","~/img/bot0.png","/chat?bot=0"},
        new string[] {"Evil AI", "Tired of nice and polite AI's? This AI will flame you more then you expect.", "~/img/bot1.png","/chat?bot=1"},
        new string[] {"Joseph Stalin", "Stalin invates you to a talk, it's an invitation you can't reject, literally if you reject you will go to guag.", "~/img/bot2.png","/chat?bot=2"},
        new string[] {"Patrict Bateman", "Are you a sigma or not, i don't know but Patrick Bateman is.", "~/img/bot3.png","/chat?bot=3"},
        new string[] {"Albert Einstein ", "Albert Einstein was a German-born theoretical physicist who is widely held to be one of the greatest and most influential scientists of all time.", "~/img/bot4.png","/chat?bot=4"},
        new string[] {"Carl Jung", "A famous psychologist, he is here to talk with you about your experiences and your dreams.", "~/img/bot5.png","/chat?bot=5"},
    };

    public IndexModel(ILogger<IndexModel> logger)
    {
        _logger = logger;
    }

    public  void OnGet()
    {
        
    }
    public IActionResult OnPost()
{
    return RedirectToPage("/chat.cshtml");
}
}
