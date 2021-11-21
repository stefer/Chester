using Chester.Models;
using System.Collections.Generic;

namespace Chester.Messages.Events
{
    /// <summary>
    /// currmove: info currmove e2e4 currmovenumber 1
    /// pv: info depth 2 score cp 214 time 1242 nodes 2124 nps 34928 pv e2e4 e7e5 g1f3
    /// </summary>
    internal class Info: Event
    {
        public Move CurrentMove { get; set; }
        public long? CurrentMoveNumber { get; set; }
        public int? Score { get; set; }
        public long? TimeMs { get; set; }
        public long? Nodes { get; set; }
        public long? NodesPerSec { get; set; }
        public int? Depth { get; set; }
        public IEnumerable<Move> Pv { get; set; }
    }
}
