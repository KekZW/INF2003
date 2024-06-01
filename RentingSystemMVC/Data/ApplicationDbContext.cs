using RentingSystemMVC.Models;

namespace RentingSystemMVC.Data;
using Microsoft.EntityFrameworkCore;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }
    
    public DbSet<User> User { get; set; }
    
    public DbSet<License> License { get; set; }
    
    public DbSet<Vehicle> Vehicle { get; set; }
    
}