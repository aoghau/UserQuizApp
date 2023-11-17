using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UserQuizApp.Data
{
    public class Quiz
    {
        public int Id { get; set; }
        public string QuizName { get; set; }
        public bool IsCompleted {  get; set; }

        public List<Question> Questions { get; set; }

        public int UserId { get; set; }
        public User User { get; set; }
    }
}
