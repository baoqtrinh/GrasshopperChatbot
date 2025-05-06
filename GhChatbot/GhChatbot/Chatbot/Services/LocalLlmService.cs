using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Glab.C_AI.Chatbot
{
    public class LocalLlmService
    {
        private readonly HttpClient _httpClient;
        private readonly string _baseUrl;
        private readonly List<ChatMessage> _dialogHistory = new List<ChatMessage>(); // To store dialog history
        public string SystemPrompt { get; set; } // New property for the system prompt

        public LocalLlmService(string localLlmEndpoint)
        {
            if (string.IsNullOrEmpty(localLlmEndpoint))
                throw new ArgumentException("Local LLM endpoint cannot be null or empty.", nameof(localLlmEndpoint));

            _httpClient = new HttpClient();
            _baseUrl = localLlmEndpoint;
        }

        public void SetDialogHistory(List<ChatMessage> dialog)
        {
            _dialogHistory.Clear();
            if (dialog != null)
                _dialogHistory.AddRange(dialog);
        }


        public async Task<string> SendMessageAsync(string message)
        {
            try
            {
                var messages = new List<object>();

                // Add the system prompt if it exists
                if (!string.IsNullOrEmpty(SystemPrompt))
                {
                    messages.Add(new
                    {
                        role = "system",
                        content = SystemPrompt
                    });
                }

                // Add the dialog history
                messages.AddRange(_dialogHistory.Select(h => new
                {
                    role = h.Role,
                    content = h.Content
                }));

                // Add the user message
                messages.Add(new
                {
                    role = "user",
                    content = message
                });

                var payload = new
                {
                    messages = messages
                };

                var content = new StringContent(
                    JsonSerializer.Serialize(payload),
                    Encoding.UTF8,
                    "application/json");

                var response = await _httpClient.PostAsync(_baseUrl, content);
                response.EnsureSuccessStatusCode();

                var responseContent = await response.Content.ReadAsStringAsync();
                var assistantResponse = ParseResponse(responseContent);

                // Update the dialog history
                _dialogHistory.Add(new ChatMessage { Role = "user", Content = message });
                _dialogHistory.Add(new ChatMessage { Role = "assistant", Content = assistantResponse });

                return assistantResponse;
            }
            catch (Exception ex)
            {
                return $"Error communicating with Local LLM: {ex.Message}";
            }
        }

        public List<ChatMessage> GetDialogHistory()
        {
            return _dialogHistory;
        }

        public void ClearDialogHistory()
        {
            _dialogHistory.Clear();
        }


        private string ParseResponse(string responseContent)
        {
            using (JsonDocument document = JsonDocument.Parse(responseContent))
            {
                var root = document.RootElement;

                if (root.TryGetProperty("choices", out var choices) && choices.ValueKind == JsonValueKind.Array)
                {
                    var firstChoice = choices.EnumerateArray().FirstOrDefault();
                    if (firstChoice.TryGetProperty("message", out var messageElement) &&
                        messageElement.TryGetProperty("content", out var contentElement))
                    {
                        return contentElement.GetString() ?? "No response received.";
                    }
                }
            }

            return "No response received.";
        }
    }
}
