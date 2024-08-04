using ChatApp.Abstractions;
using ChatApp.Models;

namespace ChatApp.Services
{
    public class ChatService : IChatService
    {
        private readonly IChatRepository _chatRepository;

        public ChatService(IChatRepository chatRepository)
        {
            _chatRepository = chatRepository;
        }

        public async Task<IEnumerable<Message>> GetAllMessagesAsync()
        {
            return await _chatRepository.GetAllMessagesAsync();
        }

        public async Task<Message> AddMessageAsync(string sender, string content)
        {
            var message = new Message
            {
                Sender = sender,
                Content = content,
                Timestamp = DateTime.UtcNow,
                Sentiment = "neutral"
            };

            return await _chatRepository.AddMessageAsync(message);
        }

        public async Task<bool> DeleteMessageAsync(int id)
        {
            return await _chatRepository.DeleteMessageAsync(id);
        }
    }

}
