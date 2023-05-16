using Api.Controllers;
using BusinessLogic.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Models.Twitter;
using Moq;
using System;
using Xunit;

namespace Api.Tests.Controllers
{
    public class TwitterControllerTests
    {
        [Fact]
        public void Metrics_ShouldReturnMetrics()
        {
            // Arrange
            var serviceMock = new Mock<ITwitterService>();
            var expectedMetrics = new TwitterMetrics
            {
                NumberOfMessages = 10,
                ShortestMessageLenght = 5,
                LongestMessageLenght = 20,
                AverageMessageLenght = 10.5
            };
            serviceMock.Setup(s => s.GetMetrics()).Returns(expectedMetrics);

            var controller = new TwitterController(serviceMock.Object);

            // Act
            var result = controller.Metrics();

            // Assert
            Assert.IsType<ActionResult<TwitterMetrics>>(result);
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            Assert.Equal(expectedMetrics, okResult.Value);
        }
    }
}
