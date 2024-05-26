using Microsoft.EntityFrameworkCore;

public class YourEntity
{
    public int id { get; set; }
    public string name { get; set; }

    public string email { get; set; }

    public string PASSWORD { get; set; }
}


public class ApplicationDbContext : DbContext
{

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }


    public DbSet<YourEntity> YourEntities { get; set; }



}







