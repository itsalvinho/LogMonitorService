namespace LogMonitorService.Models.API.Results
{
    public class BaseServiceResult
    {
        public ResultType ResultType { get; }
        public Exception Exception { get; }

        public BaseServiceResult(ResultType resultTpye, Exception exception = null)
        {
            ResultType = resultTpye;
            Exception = exception;
        }
    }
}
