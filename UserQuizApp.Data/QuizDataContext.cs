using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UserQuizApp.Data
{
    public class QuizDataContext : DbContext
    {
        public QuizDataContext(DbContextOptions<QuizDataContext> options) : base(options) 
        {
            Database.EnsureCreated();
        }
        public QuizDataContext() :base() { }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseNpgsql("Server=localhost;Database=QuizDB;Port=5432;User Id=postgres;Password=l'horizon");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Quiz>()
            .HasOne(q => q.User)
            .WithMany(q => q.Quizzes)
            .HasForeignKey(q => q.UserId);
            
            modelBuilder.Entity<Question>()
            .HasOne(q => q.Quiz)
            .WithMany(q => q.Questions)
            .HasForeignKey(q => q.QuizId);

            modelBuilder.Entity<Answer>()
            .HasOne(a => a.Question)
            .WithMany(a => a.Answers)
            .HasForeignKey(a => a.QuestionId);
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Quiz> Quizzes { get; set; }
        public DbSet<Question> Questions { get; set; }
        public DbSet<Answer> Answers { get; set; }
    }
}
