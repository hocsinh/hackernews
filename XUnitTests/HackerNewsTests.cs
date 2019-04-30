using hackernews.Controllers;
using hackernews.Models;
using hackernews.Services;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace XUnitTests
{
    public class HackerNewsTests
    {
        [Fact]
        public async Task Index_ReturnsAViewResultWithCompleteNewsList_NoFilter()
        {
            // Arrange
            var searchString = "";
            var mockRepo = new Mock<IHackerNewsDataService>();
            mockRepo.Setup(repo => repo.GetNewsListAsync()).ReturnsAsync(GetNewsListFakeData());

            var controller = new HomeController(mockRepo.Object);

            // Act
            ActionResult result = await controller.Index(searchString);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<IEnumerable<HackerNewsModel>>(viewResult.ViewData.Model);

            Assert.Equal(2, model.Count());
        }

        [Fact]
        public async Task Index_ReturnsAViewResultWithNewsList_WithAMatchFilter()
        {
            // Arrange
            var mockRepo = new Mock<IHackerNewsDataService>();
            mockRepo.Setup(repo => repo.GetNewsListAsync()).ReturnsAsync(GetNewsListFakeData());
            var searchString = "John";

            var controller = new HomeController(mockRepo.Object);

            // Act
            ActionResult result = await controller.Index(searchString);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<IEnumerable<HackerNewsModel>>(viewResult.ViewData.Model);

            Assert.Single(model);
        }

        [Fact]
        public async Task Index_ReturnsAViewResultWithNoResult_NoMatchFilter()
        {
            // Arrange
            var mockRepo = new Mock<IHackerNewsDataService>();
            mockRepo.Setup(repo => repo.GetNewsListAsync()).ReturnsAsync(GetNewsListFakeData());
            var searchString = "agasgaygyryyryahahg";

            var controller = new HomeController(mockRepo.Object);

            // Act
            ActionResult result = await controller.Index(searchString);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<IEnumerable<HackerNewsModel>>(viewResult.ViewData.Model);

            Assert.Empty(model);
        }

        private List<HackerNewsModel> GetNewsListFakeData()
        {
            var news = new List<HackerNewsModel>();
            news.Add(new HackerNewsModel()
            {
                By = "John",
                Title = "Test One"
            });
            news.Add(new HackerNewsModel()
            {
                By = "Robert",
                Title = "Test Two"
            });
            return news;
        }
    }
}
