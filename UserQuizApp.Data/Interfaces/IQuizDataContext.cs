using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UserQuizApp.Data.Interfaces
{
    public interface IQuizDataContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Quiz> Quizzes { get; set; }
        public DbSet<Question> Questions { get; set; }
        public DbSet<Answer> Answers { get; set; }

        public List<User> GetUsers();
        public List<Quiz> GetQuizzes();
        public List<Question> GetQuestions();
        public List<Answer> GetAnswers();
        public int SaveChanges();
    }
}
