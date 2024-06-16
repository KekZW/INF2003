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

    public DbSet<Rental> Rental { get; set; }
    
    public DbSet<Vehicle> Vehicle { get; set; }
    
    public DbSet<VehicleType> VehicleType { get; set; }

    public DbSet<VehicleViewModel> VehicleViewModel { get; set; }
    
    public DbSet<Maintenance> Maintenance { get; set; }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<VehicleViewModel>().HasNoKey();

    }

}