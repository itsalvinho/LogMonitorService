namespace LogMonitorService.Models.API.Results
{
    /// <summary>
    /// Result category for each ServiceResult
    /// </summary>
    public enum ResultType
    {
        Success,
        InvalidRequest,
        NotFound,
        NoPermission,
        UnknownError
    }
}
