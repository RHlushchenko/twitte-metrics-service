using System.Collections;
using System.Collections.Concurrent;
using DataAccess.Interfaces;
using Microsoft.Extensions.Logging;
using Models.Twitter;

namespace DataAccess
{
    public class TwitterRepository : IRepository<MessageData>
    {
        private readonly ConcurrentDictionary<string, MessageData> storage;
        private readonly ILogger<MessageData> logger;

        public TwitterRepository(ILogger<MessageData> logger)
        {
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.storage = new ConcurrentDictionary<string, MessageData>();
        }

        public MessageData Add(MessageData entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            if (this.storage.TryAdd(entity.Id!, entity))
            {
                this.logger.LogInformation($"Message added to the storage. Id={entity.Id}");

                return entity;
            }
            else
            {
                throw new ArgumentException($"The message with Id={entity.Id} already exists.");
            }
        }

        public void Clear()
        {
            this.storage.Clear();
            this.logger.LogInformation("Storage cleared.");
        }

        public int Count()
        {
            return this.storage.Count;
        }

        public IEnumerator<MessageData> GetEnumerator()
        {
            return this.storage.Values.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.storage.Values.GetEnumerator();
        }
    }
}
