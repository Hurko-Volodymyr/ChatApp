using ChatApp.Abstractions;
using ChatApp.Models;
using Microsoft.EntityFrameworkCore;

namespace ChatApp.Data.Repositories
{
    public class ChatRepository : IChatRepository
    {
        private readonly ChatContext _context;

        public ChatRepository(ChatContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Message>> GetAllMessagesAsync()
        {
            return await _context.Messages.OrderBy(m => m.Timestamp).ToListAsync();
        }

        public async Task<Message> AddMessageAsync(Message message)
        {
            _context.Messages.Add(message);
            await _context.SaveChangesAsync();
            return message;
        }

        public async Task<bool> DeleteMessageAsync(int id)
        {
            var message = await _context.Messages.FindAsync(id);
            if (message == null)
            {
                return false;
            }

            _context.Messages.Remove(message);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
