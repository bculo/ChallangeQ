namespace QCode.Core.Exceptions
{
    public class QCodeValidationException : QCodeBaseException
    {
        public IDictionary<string, string[]>? Errors { get; set; }

        public QCodeValidationException(string message) : base(message)
        {
        }

        public QCodeValidationException(IDictionary<string, string[]> validationErrors, string message = "Validation exception")
            : base(message)
        {
            Errors = validationErrors;
        }
    }
}
