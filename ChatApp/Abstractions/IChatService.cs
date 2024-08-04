using ChatApp.Models;

namespace ChatApp.Abstractions
{
    public interface IChatService
    {
        Task<IEnumerable<Message>> GetAllMessagesAsync();
        Task<Message> AddMessageAsync(string sender, string content);
        Task<bool> DeleteMessageAsync(int id);
    }

}
