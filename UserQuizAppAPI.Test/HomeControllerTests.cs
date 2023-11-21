using Microsoft.AspNetCore.Mvc;
using Moq;
using UserQuizApp.Controllers;
using UserQuizApp.Data;
using UserQuizApp.Data.Interfaces;
using UserQuizApp.Interfaces;
using UserQuizApp.Middleware;
using UserQuizApp.Utility;

namespace UserQuizAppAPI.Test
{
    public class Tests
    {
        [SetUp]
        public void Setup()
        {
            sample = new User() { Id = 0, Name = "user", Password = "password", Quizzes = quizzes };
            quiz = new Quiz() { Id = 0, QuizName = "Sample Quiz", Questions = questions, UserId = 0, IsCompleted = false };
            first = new Question() { Id = 0, QuestionText = "Why did the chicken cross the road?", QuizId = 0 };
            joke = new Answer() { Id = 0, AnswerText = "To get to the other side", IsCorrect = true, QuestionId = 0 };
            quizzes.Add(quiz);
            questions.Add(first);
            answers.Add(joke);
            users.Add(sample);
        }

        [Test]
        public void Home_ReturnsValidResponseWhenLoggedIn()
        {
            //Arrange
            var mockDb = new Mock<IQuizDataContext>();
            var mockAuth = new Mock<IAuthService>();
            mockDb.Setup<List<User>>(x => x.GetUsers()).Returns(users);
            mockDb.Setup<List<Quiz>>(x => x.GetQuizzes()).Returns(quizzes);
            mockAuth.Setup<bool>(x => x.ValidateUser()).Returns(true);
            mockAuth.Setup<string>(x => x.ValidatedUserName()).Returns(sample.Name);
            var controller = new HomeController(mockDb.Object, mockAuth.Object);
            var combwrap = new CombinedWrapper<Quiz>(){ List = quizzes.ToArray(), WrapName = sample.Name};
            var actual = new JsonResult(combwrap);
            //Act
            var response = controller.Home();

            //Assert
            Assert.AreEqual(response.ToString(), actual.ToString());
        }

        [Test]
        public void AssignmentSelection_ShouldReturnJSON()
        {
            //Arrange
            var mockDb = new Mock<IQuizDataContext>();
            var mockAuth = new Mock<IAuthService>();
            mockDb.Setup<List<User>>(x => x.GetUsers()).Returns(users);
            mockDb.Setup<List<Quiz>>(x => x.GetQuizzes()).Returns(quizzes);
            mockAuth.Setup<bool>(x => x.ValidateUser()).Returns(true);
            var controller = new HomeController(mockDb.Object, mockAuth.Object);

            //Act
            var response = controller.AssignmentSelection();
            var actual = new JsonResult(new ListWrapper<Quiz> { List = quizzes.ToArray() });

            //Assert
            Assert.AreEqual(response.ToString(), actual.ToString());
        }

        [Test]
        public void AssignQuiz_ShouldAssignQuizToUser()
        {
            //Arrange
            var mockDb = new Mock<IQuizDataContext>();
            var mockAuth = new Mock<IAuthService>();
            var newquizzes = quizzes;
            mockDb.Setup<List<User>>(x => x.GetUsers()).Returns(users);
            mockDb.Setup<List<Quiz>>(x => x.GetQuizzes()).Returns(newquizzes);
            mockAuth.Setup<bool>(x => x.ValidateUser()).Returns(true);
            mockAuth.Setup<string>(x => x.ValidatedUserName()).Returns(sample.Name);
            var newquiz = new Quiz() { QuizName = "new", IsCompleted = false, Questions = null, Id = 1 };
            newquizzes.Add(newquiz);
            var controller = new HomeController(mockDb.Object, mockAuth.Object);

            //Act
            controller.AssignQuiz(newquiz.Id);

            //Assert
            Assert.IsNotNull(sample.Quizzes.Where(x => x.QuizName.Equals(newquiz.QuizName)).FirstOrDefault());
        }

        [Test]
        public void PassQuiz_ShouldMarkQuizAsPassed()
        {
            //Arrange
            var mockDb = new Mock<IQuizDataContext>();
            var mockAuth = new Mock<IAuthService>();
            var newquizzes = quizzes;
            mockDb.Setup<List<User>>(x => x.GetUsers()).Returns(users);
            mockDb.Setup<List<Quiz>>(x => x.GetQuizzes()).Returns(newquizzes);
            mockAuth.Setup<bool>(x => x.ValidateUser()).Returns(true);
            mockAuth.Setup<string>(x => x.ValidatedUserName()).Returns(sample.Name);
            var newquiz = new Quiz() { QuizName = "new", IsCompleted = false, Questions = null, Id = 1 };
            newquizzes.Add(newquiz);
            var controller = new HomeController(mockDb.Object, mockAuth.Object);

            //Act
            controller.PassQuiz(newquiz.Id);

            //Assert
            Assert.IsTrue(quizzes.Where(x => x.QuizName.Equals(newquiz.QuizName)).FirstOrDefault().IsCompleted);
        }

        [Test]
        public void LoadQuiz_ShouldReturnTheQuiz()
        {
            //Arrange
            var mockDb = new Mock<IQuizDataContext>();
            var mockAuth = new Mock<IAuthService>();
            var newquizzes = quizzes;
            mockDb.Setup<List<User>>(x => x.GetUsers()).Returns(users);
            mockDb.Setup<List<Quiz>>(x => x.GetQuizzes()).Returns(newquizzes);
            mockDb.Setup<List<Question>>(x => x.GetQuestions()).Returns(questions);
            mockDb.Setup<List<Answer>>(x => x.GetAnswers()).Returns(answers);
            mockAuth.Setup<bool>(x => x.ValidateUser()).Returns(true);
            mockAuth.Setup<string>(x => x.ValidatedUserName()).Returns(sample.Name);
            var newquiz = new Quiz() { QuizName = "new", IsCompleted = false, Questions = questions, Id = 1 };
            newquizzes.Add(newquiz);
            List<QuestionWrapper> actualWrappers = new List<QuestionWrapper>();
            for(int i = 0; i < newquiz.Questions.Count; i++) 
            {
                List<Answer> newanswers = answers.Where(x => x.QuestionId == newquiz.Questions[i].Id).ToList();
                QuestionWrapper questionWrapper = new QuestionWrapper(){ QuestionAnswers = newanswers, QuestionWrapText = newquiz.Questions[i].QuestionText };
                actualWrappers.Add(questionWrapper);
            }
            QuizWrapper wrapper = new QuizWrapper(){ List = actualWrappers, QuizWrapName = newquiz.QuizName};
            var actual = new JsonResult(wrapper);
            var controller = new HomeController(mockDb.Object, mockAuth.Object);

            //Act
            var response = controller.LoadQuiz(1);

            //Assert
            Assert.AreEqual(response.ToString(), actual.ToString());
        }

        private List<User> users = new List<User>();
        private List<Quiz> quizzes = new List<Quiz>();
        private List<Question> questions = new List<Question>();
        private List<Answer> answers = new List<Answer>();
        private User sample;
        private Quiz quiz;
        private Question first;
        private Answer joke;
    }
}