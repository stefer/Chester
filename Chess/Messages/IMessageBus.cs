using System.Threading.Tasks;

namespace Chess.Messages
{
    public interface IMessageBus
    {
        Task SendAsync<TCommand>(TCommand command) where TCommand : Message;
    }
}
