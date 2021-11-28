using NUnit.Framework;
using OpenQA.Selenium.Appium;
using OpenQA.Selenium.Appium.Android;
using System;

namespace Android_Tests_TaskBoard
{
    public class Android_Tests_TaskBoard
    {
        const string AppForTesting = @"C:\Users\Lenovo\Downloads\taskboard-androidclient.apk";
        const string ApiServiceUrl = "https://taskboard.adelinapetrova.repl.co/api";

        private AndroidDriver<AndroidElement> driver;

        [OneTimeSetUp]
        public void SetupLocalService()
        {
            var appiumOptions = new AppiumOptions() { PlatformName = "Android" };
            appiumOptions.AddAdditionalCapability("app", AppForTesting);
            driver = new AndroidDriver<AndroidElement>(
                new Uri("http://[::1]:4723/wd/hub"), appiumOptions);

            driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(3);
        }

        [Test]
        public void Test_AndroidApp_SearchTasks()
        {
            // Connect to the RESTful service
            var editTextApiUrl = driver.FindElementById(
                "taskboard.androidclient:id/editTextApiUrl");
            editTextApiUrl.Clear();
            editTextApiUrl.SendKeys(ApiServiceUrl);

            var buttonConnect = driver.FindElementById(
                "taskboard.androidclient:id/buttonConnect");
            buttonConnect.Click();

            //Assert the first listed tasks has title "Project skeleton".
            var titleResult = driver.FindElementById("taskboard.androidclient:id/textViewTitle");
            titleResult.Click();
            StringAssert.Contains("Project skeleton", titleResult.Text);

            //Add new task with unique title
            var addTask = driver.FindElementById("taskboard.androidclient:id/buttonAdd");
            addTask.Click();

            driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(30);

            var title = driver.FindElementById("taskboard.androidclient:id/editTextTitle");
            var uniqueName = Guid.NewGuid().ToString();
            title.Clear();
            title.SendKeys(uniqueName);

            var description = driver.FindElementById("taskboard.androidclient:id/editTextDescription");
            description.Click();
            description.SendKeys("New task");

            var buttonTaskCreate = driver.FindElementById("taskboard.androidclient:id/buttonCreate");
            buttonTaskCreate.Click();

            // Search the newly created task
            var searchTasks = driver.FindElementById("taskboard.androidclient:id/editTextKeyword");
            searchTasks.Clear();
            searchTasks.Click();
            searchTasks.SendKeys(uniqueName);

            var buttonSearch = driver.FindElementById("taskboard.androidclient:id/buttonSearch");
            buttonSearch.Click();

            driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(30);

            // Assert that one or several tasks are displayed
            var textViewTaskResult = driver.FindElementById(
                "taskboard.androidclient:id/textViewStatus");
            StringAssert.Contains("Tasks found:", textViewTaskResult.Text);

            // Assert that the new task exist
            var textViewTitle = driver.FindElementById(
               "taskboard.androidclient:id/textViewTitle");
            Assert.AreEqual(uniqueName, textViewTitle.Text);
        }

        [OneTimeTearDown]
        public void ShutDown()
        {
            driver.Quit();
        }
    }
}
