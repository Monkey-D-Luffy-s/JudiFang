using instademo.Models;
using Microsoft.EntityFrameworkCore;

namespace instademo.data
{
    public class InstaReelDbContext : DbContext
    {
        public InstaReelDbContext(DbContextOptions options) : base(options)
        {
            
        }

        public DbSet<VideoClass> Reels { get; set; }
    }
}
