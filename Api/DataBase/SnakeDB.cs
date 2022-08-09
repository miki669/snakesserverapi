using Microsoft.EntityFrameworkCore;
using SnakeServerAPI.DataBase.Data;

namespace SnakeServerAPI.DataBase
{
    public class SnakeDB : DbContext
    {
        public SnakeDB(DbContextOptions<SnakeDB> options) : base(options)
        {
            Database.EnsureCreated();
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.EnableSensitiveDataLogging();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<SnakeUser>().HasMany(p => p.Roles).WithMany(p => p.Users);
            modelBuilder.Entity<SnakeUser>().HasMany(p => p.Tokens).WithOne(p => p.SnakeUser);
            modelBuilder.Entity<SnakeRole>().HasIndex(p => p.RoleId).IsUnique(true);

        }



        public DbSet<BytesApplication> MainAppByte { get; set; }
        public DbSet<SnakeUser> Users { get; set; }
        public DbSet<SnakeRole> Roles { get; set; }
        public DbSet<UserToken> Tokens { get; set; }

    }
}
