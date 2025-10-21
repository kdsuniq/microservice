namespace AuthService.Core.Infrastructure
{
    public class TraceService
    {
        private string _traceId = "";

        public string GetTraceId() => _traceId;

        public void SetTraceId(string traceId)
        {
            _traceId = string.IsNullOrEmpty(traceId) ? Guid.NewGuid().ToString() : traceId;
        }
    }
}