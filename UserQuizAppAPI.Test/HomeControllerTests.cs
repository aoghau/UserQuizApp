using Microsoft.AspNetCore.Mvc;
using Moq;
using UserQuizApp.Controllers;

namespace UserQuizAppAPI.Test
{
    public class Tests
    {
        [SetUp]
        public void Setup()
        {
        }

        public void HomeTest()
        {
            var mockHome = new Mock<HomeController>();
            //mockHome.Setup<IActionResult>
        }
    }
}