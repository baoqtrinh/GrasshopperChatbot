using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using GhChatbot.Models;
using GhChatbot.Services;

namespace GhChatbot.UI
{
    public partial class ChatWindow : Window
    {
        private readonly string _apiKey;
        private readonly ApiService _apiService;
        private ObservableCollection<ChatMessage> _messages;
        private bool _isSendingMessage;
        
        public ChatWindow(string apiKey)
        {
            InitializeComponent();
            
            _apiKey = apiKey;
            _apiService = new ApiService(_apiKey);
            _messages = new ObservableCollection<ChatMessage>();
            ChatMessages.ItemsSource = _messages;
            
            // Add welcome message
            _messages.Add(new ChatMessage
            {
                SenderName = "Assistant",
                Content = "Hello! How can I help you today?",
                Timestamp = DateTime.Now,
                IsUserMessage = false
            });
            
            MessageInput.Focus();
        }

        private async void SendButton_Click(object sender, RoutedEventArgs e)
        {
            await SendMessage();
        }

        private async void MessageInput_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter && Keyboard.Modifiers == ModifierKeys.Control)
            {
                e.Handled = true;
                await SendMessage();
            }
        }

        private async Task SendMessage()
        {
            if (_isSendingMessage) return;
            
            string messageText = MessageInput.Text.Trim();
            if (string.IsNullOrEmpty(messageText)) return;
            
            _isSendingMessage = true;
            SendButton.IsEnabled = false;
            MessageInput.IsEnabled = false;
            
            try
            {
                // Add user message
                _messages.Add(new ChatMessage
                {
                    SenderName = "You",
                    Content = messageText,
                    Timestamp = DateTime.Now,
                    IsUserMessage = true
                });
                
                // Clear input
                MessageInput.Clear();
                
                // Scroll to bottom
                ChatScrollViewer.ScrollToBottom();
                
                // Send to API
                string response = await _apiService.SendMessageAsync(messageText);
                
                // Add response message
                _messages.Add(new ChatMessage
                {
                    SenderName = "Assistant",
                    Content = response,
                    Timestamp = DateTime.Now,
                    IsUserMessage = false
                });
                
                // Scroll to bottom
                ChatScrollViewer.ScrollToBottom();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "API Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                _isSendingMessage = false;
                SendButton.IsEnabled = true;
                MessageInput.IsEnabled = true;
                MessageInput.Focus();
            }
        }
    }
    
    public class MessageBorderConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            bool isUserMessage = (bool)value;
            return isUserMessage ? "#1E88E5" : "#E0E0E0";
        }
        
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
    
    public class MessageBackgroundConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            bool isUserMessage = (bool)value;
            return isUserMessage ? "#E3F2FD" : "#FFFFFF";
        }
        
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
    
    public class MessageAlignmentConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            bool isUserMessage = (bool)value;
            return isUserMessage ? HorizontalAlignment.Right : HorizontalAlignment.Left;
        }
        
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
