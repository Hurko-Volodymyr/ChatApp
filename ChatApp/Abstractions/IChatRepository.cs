using ChatApp.Models;

namespace ChatApp.Abstractions
{
    public interface IChatRepository
    {
        Task<IEnumerable<Message>> GetAllMessagesAsync();
        Task<Message> AddMessageAsync(Message message);
        Task<bool> DeleteMessageAsync(int id);
    }
}
