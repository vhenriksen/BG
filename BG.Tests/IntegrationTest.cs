using System;
using BG.Controller;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BG.Tests
{
    [TestClass]
    public class IntegrationTest
    {

        private const int EmployeeId = 50;

        private readonly IService service = Service.Instance;

        [TestMethod]
        public void LogInTest()
        {

           //service.CreateTestData();

            var username = Guid.NewGuid() + "aA";
            var password = Guid.NewGuid() + "aA";

            service.CreateUserIfNotExists(username, password, EmployeeId,true);

            Assert.IsTrue(service.UserExists(username, password), "The user was not created.");

            Assert.IsTrue(service.UserExists(username.ToLower(), password), "The username is case sensitive.");
            Assert.IsFalse(service.UserExists(username, password.ToLower()), "The password is not case sensitive.");

        }
    }
}
