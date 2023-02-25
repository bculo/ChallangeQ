namespace QCode.Core.Exceptions
{
    public class QCodeBaseException : Exception
    {
        public string? UserMessage { get; set; }

        public QCodeBaseException(string message) : base(message)
        {
            UserMessage = message;
        }
    }
}
