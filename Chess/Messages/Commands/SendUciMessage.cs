namespace Chester.Messages.Commands
{
    internal class SendUciMessage : Event
    {
        public string Status { get; }

        public SendUciMessage(string status)
        {
            Status = status;
        }
    }
}
