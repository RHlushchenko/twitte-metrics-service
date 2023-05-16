using DataAccess;
using Microsoft.Extensions.Logging;
using Models.Twitter;
using Moq;
using System;
using Xunit;

namespace DataAccess.Tests
{
    public class TwitterRepositoryTests
    {
        private readonly MockRepository mockRepository;
        private readonly Mock<ILogger<MessageData>> mockLogger;

        public TwitterRepositoryTests()
        {
            this.mockRepository = new MockRepository(MockBehavior.Default);
            this.mockLogger = this.mockRepository.Create<ILogger<MessageData>>();
        }

        [Fact]
        public void Add_ValidMessage_ShouldAddMessageToStorage()
        {
            // Arrange
            var messageData = new MessageData { Id = "1", Text = "Hello, world!" };
            var repository = new TwitterRepository(this.mockLogger.Object);

            // Act
            var result = repository.Add(messageData);

            // Assert
            Assert.Equal(messageData, result);
            Assert.Contains(messageData, repository);
        }

        [Fact]
        public void Add_NullMessage_ShouldThrowArgumentNullException()
        {
            // Arrange
            var repository = new TwitterRepository(this.mockLogger.Object);

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => repository.Add(null));
        }

        [Fact]
        public void Add_DuplicateMessage_ShouldThrowArgumentException()
        {
            // Arrange
            var messageData = new MessageData { Id = "1", Text = "Hello, world!" };
            var repository = new TwitterRepository(this.mockLogger.Object);

            // Add the message for the first time
            repository.Add(messageData);

            // Act & Assert
            Assert.Throws<ArgumentException>(() => repository.Add(messageData));
        }

        [Fact]
        public void Clear_ShouldClearStorage()
        {
            // Arrange
            var messageData = new MessageData { Id = "1", Text = "Hello, world!" };
            var repository = new TwitterRepository(this.mockLogger.Object);

            // Add a message to the storage
            repository.Add(messageData);

            // Act
            repository.Clear();

            // Assert
            Assert.Empty(repository);
        }

        [Fact]
        public void Count_ShouldReturnNumberOfMessagesInStorage()
        {
            // Arrange
            var messageData1 = new MessageData { Id = "1", Text = "Hello, world!" };
            var messageData2 = new MessageData { Id = "2", Text = "Another tweet" };
            var repository = new TwitterRepository(this.mockLogger.Object);

            // Add two messages to the storage
            repository.Add(messageData1);
            repository.Add(messageData2);

            // Act
            var count = repository.Count();

            // Assert
            Assert.Equal(2, count);
        }
    }
}
