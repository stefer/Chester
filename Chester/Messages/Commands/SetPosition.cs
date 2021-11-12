using Chester.Messages;
using Chester.Models;
using Chester.Models.Pgn;

namespace Chester.Messages.Commands
{
    internal class SetPosition : Command
    {
        public Fen Fen { get; set; }
        public bool StartPosition { get; set; }
        public MoveText Moves { get; set; } = MoveText.Empty;
    }
}
