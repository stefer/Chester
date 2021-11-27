using System.Threading.Tasks;

namespace Chester.Messages;

public interface ICommandHandler<TCommand> where TCommand : Message
{
    Task HandleAsync(TCommand message);
}
