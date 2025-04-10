using System;

namespace GhChatbot.Models
{
    public class ChatMessage
    {
        public string SenderName { get; set; }
        public string Content { get; set; }
        public DateTime Timestamp { get; set; }
        public bool IsUserMessage { get; set; }
    }
}
