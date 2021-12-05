using Chester.Messages;
using Chester.Messages.Events;
using Chester.Models;
using Chester.Search;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;

namespace Chester.Services;

internal class SearchReporter : ISearchReporter
{
    private const long MinWaitTimeMs = 500;
    private readonly IMessageBus _bus;
    private readonly Stopwatch _watch = new();
    private long _countNodes = 0;
    private long _lastLineUpdateTime = 0;
    private long _lastNodesUpdateTime = 0;
    private long _currMoveUpdateTime = 0;


    public SearchReporter(IMessageBus bus)
    {
        _bus = bus;
        _watch.Start();
    }

    public void CurrentMove(Move move, long moveNumber, int score)
    {
        var ms = _watch.ElapsedMilliseconds;
        if (ms - _currMoveUpdateTime < 2000) return;

        _currMoveUpdateTime = ms;
        // Do not wait
        _bus.SendAsync(new Info { CurrentMove = move, CurrentMoveNumber = moveNumber, Score = score });
    }

    public void BestLine(int depth, int score, IEnumerable<Move> bestLine)
    {
        var ms = _watch.ElapsedMilliseconds;
        if (ms - _lastLineUpdateTime < 500) return;

        _lastLineUpdateTime = ms;
        var nodes = _countNodes;
        var nps = 1000 * nodes / ms;
        // Do not wait
        _bus.SendAsync(new Info { Depth = depth, Score = score, Pv = bestLine, Nodes = nodes, TimeMs = ms, NodesPerSec = nps });
    }

    public void Reset()
    {
        _watch.Stop();
        _watch.Reset();
        _countNodes = 0;
        _lastLineUpdateTime = 0;
        _lastNodesUpdateTime = 0;
        _watch.Start();
    }

    public void NodeVisited()
    {
        Interlocked.Add(ref _countNodes, 1);

        var ms = _watch.ElapsedMilliseconds;
        if (ms - _lastNodesUpdateTime < 10000) return;

        _lastNodesUpdateTime = ms;
        var nodes = _countNodes;
        var nps = 1000 * nodes / ms;

        _bus.SendAsync(new Info { Nodes = nodes, TimeMs = ms, NodesPerSec = nps });
    }
}
