namespace LogMonitorService.Exceptions
{
    /// <summary>
    /// Exception to use when there is a bad request for an API
    /// </summary>
    public class InvalidRequestException : Exception
    {
        public InvalidRequestException(string message): base(message) 
        { 
        }
    }
}
