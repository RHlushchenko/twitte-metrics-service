using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BusinessLogic.Interfaces;
using DataAccess.Interfaces;
using Models.Twitter;

namespace BusinessLogic.Services
{
    public class TwitterService : ITwitterService
    {
        private readonly IRepository<MessageData> repository;

        public TwitterService(IRepository<MessageData> repository)
        {
            this.repository = repository;
        }

        public TwitterMetrics GetMetrics()
        {
            return new TwitterMetrics()
            {
                NumberOfMessages = this.repository.Count(),
                ShortestMessageLenght = this.repository.Min(message => message.Text!.Length),
                LongestMessageLenght = this.repository.Max(message => message.Text!.Length),
                AverageMessageLenght = this.repository.Average(message => message.Text!.Length)
            };
        }
    }
}
