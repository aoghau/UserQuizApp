using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UserQuizApp.Data
{
    public class Question
    {
        public int Id { get; set; }
        public string QuestionText { get; set; }

        public int QuizId { get; set; }
        public Quiz Quiz { get; set; }

        public List<Answer> Answers { get; set; }
    }
}
