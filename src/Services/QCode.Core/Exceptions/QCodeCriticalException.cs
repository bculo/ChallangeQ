namespace QCode.Core.Exceptions
{
    /// <summary>
    /// Exception should stop application from running
    /// </summary>
    public class QCodeCriticalException : QCodeBaseException
    {
        public QCodeCriticalException(string message) : base(message)
        {
            
        }
    }
}
