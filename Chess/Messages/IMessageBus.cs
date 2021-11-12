using System.Threading.Tasks;

namespace Chester.Messages
{
    public interface IMessageBus
    {
        Task SendAsync<TCommand>(TCommand command) where TCommand : Message;
    }
}
