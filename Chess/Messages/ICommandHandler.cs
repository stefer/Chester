using System.Threading.Tasks;

namespace Chess.Messages
{

    public interface ICommandHandler<TCommand> where TCommand : Message
    {
        Task HandleAsync(TCommand message);
    }
}
