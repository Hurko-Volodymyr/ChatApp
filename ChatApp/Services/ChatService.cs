using Azure.AI.TextAnalytics;
using ChatApp.Abstractions;
using ChatApp.Models;

namespace ChatApp.Services
{
    public class ChatService : IChatService
    {
        private readonly IChatRepository _chatRepository;
        private readonly TextAnalyticsClient _textAnalyticsClient;

        public ChatService(IChatRepository chatRepository, TextAnalyticsClient textAnalyticsClient)
        {
            _chatRepository = chatRepository;
            _textAnalyticsClient = textAnalyticsClient;
        }

        public async Task<IEnumerable<Message>> GetAllMessagesAsync()
        {
            return await _chatRepository.GetAllMessagesAsync();
        }

        public async Task<Message> AddMessageAsync(string sender, string content)
        {
            var sentimentResult = await AnalyzeSentimentAsync(content);
            var message = new Message
            {
                Sender = sender,
                Content = content,
                Timestamp = DateTime.UtcNow,
                Sentiment = sentimentResult
            };

            return await _chatRepository.AddMessageAsync(message);
        }

        public async Task<bool> DeleteMessageAsync(int id)
        {
            return await _chatRepository.DeleteMessageAsync(id);
        }

        private async Task<string> AnalyzeSentimentAsync(string text)
        {
            try
            {
                var response = await _textAnalyticsClient.AnalyzeSentimentAsync(text);
                var sentiment = response.Value.Sentiment.ToString().ToLower();
                return sentiment;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error analyzing sentiment: {ex.Message}");
                return "neutral";
            }
        }
    }

}
