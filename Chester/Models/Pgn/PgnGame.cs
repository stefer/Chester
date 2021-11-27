using System.Collections.Generic;

namespace Chester.Models.Pgn;

public class PgnGame
{
    public PgnGame(Dictionary<string, string> attributes, MoveText moves)
    {
        Attributes = attributes;
        Moves = moves;
    }

    public string Event => Attributes.GetValueOrDefault(nameof(Event));
    public string Site => Attributes.GetValueOrDefault(nameof(Site));
    public PgnDate Date => PgnDate.FromString(Attributes.GetValueOrDefault(nameof(Date)));
    public int Round => int.TryParse(Attributes.GetValueOrDefault(nameof(Round)), out var value) ? value : 0;
    public string White => Attributes.GetValueOrDefault(nameof(White));
    public string Black => Attributes.GetValueOrDefault(nameof(Black));
    public PgnResult Result => PgnResult.FromString(Attributes.GetValueOrDefault(nameof(Result)));

    public IReadOnlyDictionary<string, string> Attributes { get; private set; }

    public MoveText Moves { get; private set; }
}
