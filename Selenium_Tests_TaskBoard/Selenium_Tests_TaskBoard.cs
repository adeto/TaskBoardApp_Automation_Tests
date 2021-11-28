using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Remote;
using System;

namespace Selenium_Tests_TaskBoard
{
    public class Selenium_Tests_TaskBoard
    {
        const string AppBaseUrl = "https://taskboard.adelinapetrova.repl.co";
        RemoteWebDriver driver;

        [OneTimeSetUp]
        public void Setup()
        {
            driver = new ChromeDriver();
        }

        [Test]
        public void Test_ListTasks()
        {
            // Arrange
            string tasksUrl = AppBaseUrl + "/boards";

            // Act
            driver.Navigate().GoToUrl(tasksUrl);

            // Assert
            var sectionTitle = driver.FindElementByXPath("//h1[contains(text(),'Done')]");
            Assert.AreEqual("Done", sectionTitle.Text);
            var textBoxTitle = driver.FindElementByCssSelector("table#task1.task-entry tbody tr.title > td");
            Assert.AreEqual("Project skeleton", textBoxTitle.Text);
        }

        [Test]
        public void Test_FindTaskByKeyword_ValidResults()
        {
            // Arrange
            string keyword = "home";
            string searchUrl = AppBaseUrl + "/tasks/search";

            // Act
            driver.Navigate().GoToUrl(searchUrl);
            var keywordTextBox = driver.FindElementByCssSelector("input#keyword");
            keywordTextBox.SendKeys(keyword);
            var buttonSearch = driver.FindElementByCssSelector("button#search");
            buttonSearch.Click();

            // Assert
            var searchResultsDiv = driver.FindElement(
                By.CssSelector("main > div"));
            StringAssert.Contains("tasks found", searchResultsDiv.Text);
            var firstResultName = driver.FindElement(
                By.CssSelector("tbody tr.title > td"));
            Assert.AreEqual("Home page", firstResultName.Text);
        }

        [Test]
        public void Test_FindTaskByKeyword_NoResults()
        {
            // Arrange
            string keyword = "missing" + DateTime.Now.Ticks;
            string searchUrl = AppBaseUrl + "/tasks/search";

            // Act
            driver.Navigate().GoToUrl(searchUrl);
            var keywordTextBox = driver.FindElementByCssSelector("input#keyword");
            keywordTextBox.SendKeys(keyword);
            var buttonSearch = driver.FindElementByCssSelector("button#search");
            buttonSearch.Click();

            // Assert
            var searchResultsDiv = driver.FindElement(
                By.CssSelector("main > div"));
            Assert.AreEqual("No tasks found.", searchResultsDiv.Text);
        }

        [Test]
        public void Test_CreateTask_InvalidData()
        {
            // Arrange
            string title = "";
            string description = "This is my new task for today.";
            string createContactUrl = AppBaseUrl + "/tasks/create";

            // Act
            driver.Navigate().GoToUrl(createContactUrl);
            var textBoxTitle = driver.FindElementByCssSelector("input#title");
            textBoxTitle.SendKeys(title);
            var textBoxDescription = driver.FindElementByCssSelector("textarea#description");
            textBoxDescription.SendKeys(description);
            var buttonCreate = driver.FindElementByCssSelector("button#create");
            buttonCreate.Click();

            // Assert
            var errDiv = driver.FindElement(By.CssSelector("div.err"));
            Assert.AreEqual("Error: Title cannot be empty!", errDiv.Text);
        }

        [Test]
        public void Test_CreateTask_ValidData()
        {
            // Arrange
            string title = "Open the shop";
            string description = "Open the shop at 15:00";
            string board = "Done";
            string createTaskUrl = AppBaseUrl + "/tasks/create";

            // Act
            driver.Navigate().GoToUrl(createTaskUrl);

            var textBoxTitle = driver.FindElementByCssSelector("input#title");
            textBoxTitle.Clear();
            textBoxTitle.SendKeys(title);

            var textBoxDescription = driver.FindElementByCssSelector("textarea#description");
            textBoxDescription.Clear();
            textBoxDescription.SendKeys(description);

            var dropDownBoard = driver.FindElementByCssSelector("select#boardName");
            dropDownBoard.SendKeys(board);

            var buttonCreate = driver.FindElementByCssSelector("button#create");
            buttonCreate.Click();

            // Assert
            var pageHeading = driver.FindElementByCssSelector("header.top h1");
            Assert.AreEqual("Task Board", pageHeading.Text);

            var tasksTables = driver.FindElements(By.CssSelector("table.task-entry"));
            var lastTaskTable = tasksTables[tasksTables.Count - 1];
            var textFieldTitle = lastTaskTable.FindElement(
                By.CssSelector("tr.title td"));
            Assert.AreEqual(title, textFieldTitle.Text);

            var textFieldDescription = lastTaskTable.FindElement(
                By.CssSelector("div.description"));
            Assert.AreEqual(description, textFieldDescription.Text);
        }

        [OneTimeTearDown]
        public void ShutDown()
        {
            driver.Quit();
        }
    }
}