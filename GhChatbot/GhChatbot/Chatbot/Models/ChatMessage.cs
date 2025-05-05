using System;
using System.ComponentModel;

namespace Glab.C_AI.Chatbot
{
    public class ChatMessage : INotifyPropertyChanged
    {
        public string Role { get; set; }
        public string Content { get; set; }
        public DateTime Timestamp { get; set; }
        public bool IsUserMessage { get; set; }

        // Reference to the global IsThinkingHidden state
        public static bool GlobalIsThinkingHidden { get; set; }

        // Computed property to determine thought visibility
        public bool IsThoughtVisible
        {
            get
            {
                if (GlobalIsThinkingHidden) return false;
                return !string.IsNullOrEmpty(Content) && Content.Contains("<think>") && Content.Contains("</think>");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        // Notify when visibility changes
        public void NotifyVisibilityChanged()
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsThoughtVisible)));
        }
    }
}
