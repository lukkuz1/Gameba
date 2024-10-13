using System.ComponentModel;
using DemoRest2024Live.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace DemoRest2024Live.Data;

public class GamebaDbContext : DbContext
{
    private readonly IConfiguration _configuration;
    public DbSet<Category> Categories { get; set; }
    public DbSet<Game> Games { get; set; }
    public DbSet<Comment> Comments { get; set; }

    
    public GamebaDbContext(IConfiguration configuration)
    {
        _configuration = configuration;
    }
    
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseNpgsql(_configuration.GetConnectionString(("PostgreSQL")));
    }
}