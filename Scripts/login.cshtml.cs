using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace WebApplication1.Pages
{
    public class LoginModel : PageModel
    {
        private readonly Actions _actions;



        // Constructor injection
        public LoginModel(Actions actions)
        {
            _actions = actions;
        }

        public async Task<IActionResult> OnPostLogin(string name,string password)
        {
            var user = await _actions.GetuserByName(name);
            if(user == null || user.PASSWORD != password)
            {
                return new JsonResult(new { response = "Invalid credentials", success = false });
            }

            HttpContext.Session.SetString("username", name);

            return new JsonResult(new { response = "Login successful", success = true });
        }

        public async Task<IActionResult> OnPostRegister(string name, string email, string password)
        {

            var user = await _actions.GetuserByName(name);
            if (user != null )
            {
                return new JsonResult(new { response = "User already exists" });
            }
            if(name.Length < 5 || password.Length < 5)
            {
                return new JsonResult(new { response = "Name and password must be at least 5 characters long" });
            }
            await _actions.Adduser(name, email, password);

            return new JsonResult(new { response = "Register successful" });    
        }
    }
}

    public class Actions
{
    private readonly ApplicationDbContext _context;

    // Constructor injection
    public Actions(ApplicationDbContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    public async Task Adduser(string name, string email, string PASSWORD)
    {
        var user = new YourEntity { name = name, email = email, PASSWORD = PASSWORD };
        _context.YourEntities.Add(user);
        await _context.SaveChangesAsync();
    }

    public async Task<YourEntity> GetuserById(int id)
    {
        return await _context.YourEntities.FindAsync(id);
    }

    public async Task<YourEntity> GetuserByName(string name)
    {
        return await _context.YourEntities.FirstOrDefaultAsync(e => e.name == name);
    }
}

