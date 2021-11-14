using Chester.Models;

namespace Chester.Messages.Events
{
    internal class Info: Event
    {
        public Move CurrentMove { get; set; }
        public long? CurrentMoveNumber { get; set; }
        public int? Score { get; set; }
    }
}
