using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace GhChatbot.Services
{
    public class ApiService
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiKey;
        
        // This URL is for Claude API - can be changed for other providers
        private const string ClaudeApiUrl = "https://api.anthropic.com/v1/messages";
        
        public ApiService(string apiKey)
        {
            _apiKey = apiKey;
            _httpClient = new HttpClient();
            
            // Configure HTTP client for Claude API
            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            _httpClient.DefaultRequestHeaders.Add("x-api-key", _apiKey);
            _httpClient.DefaultRequestHeaders.Add("anthropic-version", "2023-06-01");
        }

        public async Task<string> SendMessageAsync(string message)
        {
            try
            {
                var requestData = new
                {
                    model = "claude-3-opus-20240229",
                    max_tokens = 1000,
                    messages = new[]
                    {
                new { role = "user", content = message }
            }
                };

                var content = new StringContent(
                    JsonSerializer.Serialize(requestData),
                    Encoding.UTF8,
                    "application/json");

                var response = await _httpClient.PostAsync(ClaudeApiUrl, content);
                response.EnsureSuccessStatusCode();

                var responseContent = await response.Content.ReadAsStringAsync();

                // Parse the JSON response
                using (JsonDocument document = JsonDocument.Parse(responseContent))
                {
                    JsonElement root = document.RootElement;
                    JsonElement contentArray = root.GetProperty("content");

                    if (contentArray.ValueKind == JsonValueKind.Array && contentArray.GetArrayLength() > 0)
                    {
                        return contentArray[0].GetProperty("text").GetString() ?? "No response received.";
                    }

                    return "No response received.";
                }
            }
            catch (Exception ex)
            {
                return $"Error communicating with API: {ex.Message}";
            }
        }

    }
}
