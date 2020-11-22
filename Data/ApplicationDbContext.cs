using Fanfiction.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Fanfiction.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public DbSet<Fanfic> Fanfics { get; set; }
        public DbSet<Chapter> Chapters { get; set; }
        public DbSet<Show> Shows { get; set; }
        public DbSet<Score> Scores { get; set; }
        public DbSet<ChScore> ChScores { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<Tag> Tags { get; set; }
        public DbSet<Genre> Genres { get; set; }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
           // Database.EnsureDeleted(); 
            Database.EnsureCreated();
        }
       /* protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(@"Server=(localdb)\mssqllocaldb;Database=relationsdb;Trusted_Connection=True;");
        }*/
    }
}
