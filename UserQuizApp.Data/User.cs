using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UserQuizApp.Data
{
    public class User
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Password {  get; set; }

        public List<Quiz> Quizzes { get; set; }

        public User(string name, string password)
        {
            Name = name;
            Password = password;
        }
        public User() { };
    }
}
