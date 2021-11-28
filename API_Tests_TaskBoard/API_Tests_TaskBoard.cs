using NUnit.Framework;
using RestSharp;
using RestSharp.Serialization.Json;
using System;
using System.Collections.Generic;
using System.Net;

namespace API_Tests_TaskBoard
{
    public class API_Tests_TaskBoard
    {
        const string ApiBaseUrl = "https://taskboard.adelinapetrova.repl.co/api";

        private RestClient client = new RestClient(ApiBaseUrl) { Timeout = 3000 };

        [Test]
        public void Test_ListTasks()
        {
            // Arrange
            string tasksUrl = ApiBaseUrl + "/tasks";
            var request = new RestRequest(tasksUrl, Method.GET);

            // Act
            var response = client.Execute(request);

            // Assert
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
            var tasks = new JsonDeserializer()
                .Deserialize<List<TaskResponse>>(response);
        
            Assert.IsTrue(tasks.Count > 0);
            Assert.IsTrue(tasks[0].id > 0);
            Assert.AreEqual(tasks[0].board?.name, "Done");
            Assert.AreEqual(tasks[0].title, "Project skeleton");
        }

        [Test]
        public void Test_FindTaskByKeyword_ValidResults()
        {
            // Arrange
            string keyword = "home";
            string taskUrl = ApiBaseUrl + "/tasks/search/{keyword}";
            var request = new RestRequest(taskUrl, Method.GET);
            request.AddUrlSegment("keyword", keyword);

            // Act
            var response = client.Execute(request);

            // Assert
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
            var tasks = new JsonDeserializer()
                .Deserialize<List<TaskResponse>>(response);
            Assert.IsTrue(tasks.Count > 0);
            var firstContact = tasks[0];
            Assert.AreEqual("Home page", firstContact.title);
        }

        [Test]
        public void Test_FindTaskByKeyword_NoResults()
        {
            // Arrange
            string keyword = "missing" + DateTime.Now.Ticks;
            string taskUrl = ApiBaseUrl + "/tasks/search/{keyword}";
            var request = new RestRequest(taskUrl, Method.GET);
            request.AddUrlSegment("keyword", keyword);

            // Act
            var response = client.Execute(request);

            // Assert
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
            var tasks = new JsonDeserializer()
                .Deserialize<List<TaskResponse>>(response);
            Assert.IsTrue(tasks.Count == 0);
        }

        [Test]
        public void Test_CreateTask_InvalidData()
        {
            // Arrange
            string title = "";
            string description = "new task";
            string createTaskUrl = ApiBaseUrl + "/tasks";
            var request = new RestRequest(createTaskUrl, Method.POST);
            request.AddJsonBody(new { title, description });

            // Act
            var response = client.Execute(request);

            // Assert
            Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Test]
        public void Test_CreateTask_ValidData()
        {
            // Arrange
            string title = "Task007";
            string description = "New Task for current weekend";
            string createTaskUrl = ApiBaseUrl + "/tasks";
            var request = new RestRequest(createTaskUrl, Method.POST);
            request.AddJsonBody(
                new { title, description });

            // Act
            var response = client.Execute(request);

            // Assert
            Assert.AreEqual(HttpStatusCode.Created, response.StatusCode);

            string tasksUrl = ApiBaseUrl + "/tasks";
            var tasksRequest = new RestRequest(tasksUrl, Method.GET);
            var tasksResponse = client.Execute(tasksRequest);

            // Assert
            Assert.AreEqual(HttpStatusCode.OK, tasksResponse.StatusCode);
            var tasks = new JsonDeserializer()
                .Deserialize<List<TaskResponse>>(tasksResponse);
            var lastContact = tasks[tasks.Count - 1];

            Assert.IsTrue(lastContact.id > 0);
            Assert.AreEqual(title, lastContact.title);
            Assert.AreEqual(description, lastContact.description);
        }
    }
}