namespace LogMonitorService.Models.API.Results
{
    /// <summary>
    /// Used by services that facilitate API controllers to abstract the results so that API responses can be generated accordingly
    /// </summary>
    public class ServiceResult
    {
        public ResultType ResultType { get; }
        public Exception Exception { get; }

        public ServiceResult(ResultType resultTpye, Exception exception = null)
        {
            ResultType = resultTpye;
            Exception = exception;
        }
    }
}
