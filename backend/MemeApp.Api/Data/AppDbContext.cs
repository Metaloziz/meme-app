using MemeApp.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace MemeApp.Api.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<Meme> Memes => Set<Meme>();
}
