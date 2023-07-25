namespace LogMonitorService.Models.API.Results
{
    public class ServiceResult<T>: BaseServiceResult
    {
        public T Data { get; }

        public ServiceResult(ResultType resultType, T data = default(T), Exception exception = null) : base(resultType, exception)
        {
            Data = data;
        }
    }
}
