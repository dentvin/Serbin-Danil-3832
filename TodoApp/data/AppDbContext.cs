using Microsoft.EntityFrameworkCore;
using TodoApp.Models;

namespace TodoApp.Data
{
    public class AppDbContext : DbContext
    {
        public DbSet<TodoItem> Todos => Set<TodoItem>();
        public DbSet<Profile> Profiles => Set<Profile>();

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Data Source=todos.db");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Настройка связи один-ко-многим
            modelBuilder.Entity<TodoItem>()
                .HasOne(t => t.Profile)
                .WithMany(p => p.TodoItems)
                .HasForeignKey(t => t.ProfileId)
                .OnDelete(DeleteBehavior.Cascade);

            // Уникальный индекс на логин
            modelBuilder.Entity<Profile>()
                .HasIndex(p => p.Login)
                .IsUnique();

            base.OnModelCreating(modelBuilder);
        }
    }
}