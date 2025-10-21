using System.Text.Json;

namespace AuthService.Core.Infrastructure
{
    public class HttpService
    {
        private readonly HttpClient _httpClient;
        private readonly TraceService _traceService;

        public HttpService(HttpClient httpClient, TraceService traceService)
        {
            _httpClient = httpClient;
            _traceService = traceService;
        }

        public string GetTraceId() => _traceService.GetTraceId();

        public async Task<T> GetAsync<T>(string url)
        {
            try
            {
                var request = new HttpRequestMessage(HttpMethod.Get, url);
                
                // Пробрасываем TraceId
                var traceId = _traceService.GetTraceId();
                if (!string.IsNullOrEmpty(traceId))
                {
                    request.Headers.Add("traceparent", traceId);
                }

                var response = await _httpClient.SendAsync(request);
                response.EnsureSuccessStatusCode();
                
                var content = await response.Content.ReadAsStringAsync();
                
                // Опции десериализации
                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };
                
                return JsonSerializer.Deserialize<T>(content, options);
            }
            catch (Exception ex)
            {
                throw new Exception($"HTTP request failed: {ex.Message}");
            }
        }
    }
}