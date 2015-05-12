using System;
using System.Data.Objects;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using BG.Controller;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using BG.Model;

namespace BG.Tests
{
    [TestClass]
    public class ServiceTest
    {

        private const int EmployeeId = 50;

        private readonly IService service = Service.Instance;

        [TestMethod]
        public void CreateUserTest()
        {

            var username = Guid.NewGuid() + "";
            var password = Guid.NewGuid() + "";

            service.CreateUserIfNotExists(username, password, EmployeeId,true);

            Assert.IsTrue(service.UserExists(username, password), "The user was not created.");

        }

        [TestMethod]
        public void CreateEconomicProjectTest()
        {
            //service.CreateTestData();

            const int economicProjectId = 7913;
            var name = Guid.NewGuid() + "";

            var group = service.GetAllGroupsFromCache().First();
            var project = service.CreateEconomicProjectIfNotExists(economicProjectId, name,group);
            service.Container.Detach(project);
            service.Container.EconomicProjectSet.AddObject(project);
            service.Container.SaveChanges();

            Assert.IsTrue(service.ProjectExists(economicProjectId), "The project was not created.");

            //TODO: check if name is correct.

        }

        [TestMethod]
        public void GetTasksForUser()
        {
            //service.CreateTestData();

            User user = service.GetUserFromCache("Jørgen", "1234");
            Assert.IsNotNull(user);

            // henter den alle tasks
            int blabla = service.GetTasksForUser(user).Count();
            Assert.AreEqual(blabla, user.DailyReport.LastOrDefault().Task.Count);
        }

        [TestMethod]
        public void CreateNewDailyReportForUser()
        {
            User user = new User();

            Assert.IsTrue(user.DailyReport.LastOrDefault() == null);
            service.CreateNewDailyReportForUser(user);
            Assert.IsTrue(user.DailyReport.LastOrDefault() != null);
        }

        [TestMethod]
        /*
         * Create dummy objects 
         * Check if task was saved
         * */
        public void SaveTaskTest()
        {
            //service.CreateTestData();

            User user = service.GetUserFromCache("Jørgen", "1234");

            EconomicProject projekt = service.Container.EconomicProjectSet.First(p => p.EconomicTaskTypes.Any());
            EconomicTaskType taskType = projekt.EconomicTaskTypes.First();

            if (!user.DailyReport.Any())
            {
                service.CreateNewDailyReportForUser(user);

                DailyReport dr = user.DailyReport.First();
                dr.DayStarted = DateTime.Now;

                service.Container.SaveChanges();
            }

            DateTime start = DateTime.Now;
            DateTime end = start;
            end.AddHours(2);

            int preTaskCount = user.DailyReport.Last().Task.Count();

            Assert.IsTrue(service.SaveTask(projekt, taskType, start, end, user), "The task for this user was not saved");

            int postTaskCount = user.DailyReport.Last().Task.Count();

            Assert.AreEqual(preTaskCount + 1, postTaskCount);


        }

        [TestMethod]
        public void RefreshCacheOnlyTest()
        {

            var cache = service.Container.EconomicCacheSet.FirstOrDefault();
            var oldUpdatedValue = new DateTime(2000, 1, 1);
            if (cache == null)
            {
                cache = EconomicCache.CreateEconomicCache(0, oldUpdatedValue);
                service.Container.EconomicCacheSet.AddObject(cache);
                service.Container.SaveChanges();
            }
            else
            {
                foreach (var project in cache.EconomicProject.ToList())
                {
                    foreach (var taskType in project.EconomicTaskTypes.ToList())
                    {
                        foreach (var task in taskType.Task.ToList())
                        {
                            service.Container.DeleteObject(task);
                        }
                        service.Container.DeleteObject(taskType);
                    }
                    service.Container.DeleteObject(project);
                }
                cache.LastUpdated = oldUpdatedValue;
                service.Container.SaveChanges();
            }

            service.RefreshCache();

            Console.Beep();
        }

        [TestMethod]
        public void RefreshCacheTest()
        {
            var cache = service.Container.EconomicCacheSet.FirstOrDefault();
            var oldUpdatedValue = new DateTime(2000, 1, 1);
            if (cache == null)
            {
                cache = EconomicCache.CreateEconomicCache(0, oldUpdatedValue);
                service.Container.EconomicCacheSet.AddObject(cache);
                service.Container.SaveChanges();
            }
            else
            {
                foreach (var project in cache.EconomicProject.ToList())
                {
                    foreach (var taskType in project.EconomicTaskTypes.ToList())
                    {
                        foreach(var task in taskType.Task.ToList())
                        {
                            service.Container.DeleteObject(task);
                        }
                        service.Container.DeleteObject(taskType);
                    }
                    service.Container.DeleteObject(project);
                }
                cache.LastUpdated = oldUpdatedValue;
                service.Container.SaveChanges();
            }

            var rawEconomicTasks = service.GetTaskTypesFromEconomic().ToList();
            var rawEconomicProjects = rawEconomicTasks.SelectMany(t => t.EconomicProject).ToList();
            Assert.IsTrue(rawEconomicProjects.Count() > 5, "There are not enough tasks in Economic to run this test.");

            Console.Beep();

            var correctNamedTaskTypes = new List<EconomicTaskType>();
            var correctUnnamedTaskTypes = new List<EconomicTaskType>();

            var incorrectTaskTypes = new List<EconomicTaskType>();

            var projectsFromDatabase = service.GetAllEconomicProjectsFromCache();

            var correctNamedProjects = new List<EconomicProject>();
            var correctUnnamedProjects = new List<EconomicProject>();
            var incorrectProjects = new List<EconomicProject>();

            for (int i = 0; i < rawEconomicProjects.Count; i++)
            {

                var original = rawEconomicProjects[i];
                var clone = original.Clone() as EconomicProject;

                switch (i % 3)
                {

                    case 0:
                        //projects that exist in Economic.
                        clone = EconomicProject.CreateEconomicProject(0, clone.EconomicProjectId,
                                                                        Guid.NewGuid() + "");
                        correctUnnamedProjects.Add(clone);
                        break;

                    case 1:
                        //projects that exist in Economic.
                        clone = EconomicProject.CreateEconomicProject(0, clone.EconomicProjectId,
                                                                        clone.Name);
                        correctNamedProjects.Add(clone);
                        break;

                    case 2:
                        //projects that don't exist in Economic but in the database.
                        clone = EconomicProject.CreateEconomicProject(0, int.MaxValue, Guid.NewGuid() + "");
                        incorrectProjects.Add(clone);
                        break;

                }

                cache.EconomicProject.Add(clone);
                service.Container.EconomicProjectSet.AddObject(clone);

            }

            service.Container.SaveChanges();

            for (int i = 0; i < rawEconomicTasks.Count; i++)
            {

                var original = rawEconomicTasks[i];
                var clone = original.Clone() as EconomicTaskType;

                switch (i % 3)
                {

                    case 0:
                        //tasktype that exist in Economic with wrong name.
                        clone = EconomicTaskType.CreateEconomicTaskType(0, clone.EconomicTaskTypeId,
                                                                        Guid.NewGuid() + "");
                        correctUnnamedTaskTypes.Add(clone);
                        break;

                    case 1:
                        //projects that exist in Economic.
                        clone = EconomicTaskType.CreateEconomicTaskType(0, clone.EconomicTaskTypeId,
                                                                        clone.Name);
                        correctNamedTaskTypes.Add(clone);
                        break;

                    case 2:
                        //projects that don't exist in Economic but in the database.
                        clone = EconomicTaskType.CreateEconomicTaskType(0, int.MaxValue, Guid.NewGuid() + "");
                        incorrectTaskTypes.Add(clone);
                        break;

                }

                foreach (var project in original.EconomicProject)
                {
                    if (!cache.EconomicProject.Contains(project))
                    {
                        cache.EconomicProject.Add(project);
                    }
                    clone.EconomicProject.Add(project);
                }

                service.Container.EconomicTaskTypeSet.AddObject(clone);

            }

            service.Container.SaveChanges();

            Console.Beep();

            service.RecreateModelContainer();
            service.RefreshCache();
            service.RecreateModelContainer();

            cache = null;
            cache = service.Container.EconomicCacheSet.First();

            projectsFromDatabase = null;
            projectsFromDatabase = service.GetAllEconomicProjectsFromCache();

            Assert.AreNotEqual(oldUpdatedValue, cache.LastUpdated, "The LastUpdated value was not set on the cache.");

            foreach (var project in incorrectProjects)
            {
                Assert.IsFalse(cache.EconomicProject.Any(p => p.Id == project.Id),
                               "The project \"" + project.Name +
                               "\" was not removed from the cache since it was added. This is not correct, since the project has the invalid project ID " +
                               project.EconomicProjectId + ".");
            }

            foreach (var project in correctNamedProjects)
            {
                Assert.IsTrue(
                    cache.EconomicProject.Any(
                        p =>
                        p.Id == project.Id && p.Name == project.Name && p.EconomicProjectId == project.EconomicProjectId),
                    "The project \"" + project.Name +
                    "\" was removed from the cache or renamed since it was added. This is not correct, since the project has a valid project ID " +
                    project.EconomicProjectId + " and an identical name.");
            }

            foreach (var project in correctUnnamedProjects)
            {
                Assert.IsTrue(
                    cache.EconomicProject.Any(
                        p =>
                        p.Id == project.Id && p.Name != project.Name && p.EconomicProjectId == project.EconomicProjectId),
                    "The project \"" + project.Name +
                    "\" was removed from the cache or not renamed since it was added. This is not correct, since the project has a valid project ID " +
                    project.EconomicProjectId + " but an invalid name which should have been updated.");
            }

            foreach (var taskType in incorrectTaskTypes)
            {
                Assert.IsFalse(service.Container.EconomicProjectSet.Any(p => p.EconomicTaskTypes.Any(t => t.Id == taskType.Id)),
                          "The taskType \"" + taskType.Name +
                          "\" was not removed from the project since it was added. This is not correct, since the taskType has the invalid taskType ID " +
                          taskType.EconomicTaskTypeId + ".");

            }

            foreach (var taskType in correctNamedTaskTypes)
            {
                Assert.IsTrue(
                service.Container.EconomicProjectSet.Any(p => p.EconomicTaskTypes.Any(t =>
                    t.Id == taskType.Id && t.Name == taskType.Name && t.EconomicTaskTypeId == taskType.EconomicTaskTypeId)),
                "The taskType \"" + taskType.Name +
                "\" was removed from the project or renamed since it was added. This is not correct, since the taskType has a valid taskType ID " +
                taskType.EconomicTaskTypeId + " and an identical name.");

            }

            foreach (var taskType in correctUnnamedTaskTypes)
            {
                Assert.IsTrue(
                service.Container.EconomicProjectSet.Any(p => p.EconomicTaskTypes.Any(t =>
                    t.Id == taskType.Id && t.Name != taskType.Name && t.EconomicTaskTypeId == taskType.EconomicTaskTypeId)),
                "The taskType \"" + taskType.Name +
                "\" was removed from the project or not renamed since it was added. This is not correct, since the taskType has a valid taskType ID " +
                taskType.EconomicTaskTypeId + " but an invalid name which should have been updated.");
            }

        }

        [TestCleanup]
        public void Cleanup()
        {
        }

    }
}
