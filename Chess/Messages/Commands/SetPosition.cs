using Chess.Models;
using Chess.Models.Pgn;

namespace Chess.Messages.Commands
{
    internal class SetPosition : Command 
    { 
        public Fen Fen { get; set; }
        public bool StartPosition { get; set; }
        public MoveText Moves { get; set; } = MoveText.Empty;
    }
}
