using System;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.ComponentModel;

namespace Glab.C_AI.Chatbot
{
    public partial class ChatWindow : Window, INotifyPropertyChanged
    {
        private object _currentService; // Can be either LocalLlmService or AgentService
        private ObservableCollection<ChatMessage> _messages;
        private bool _isSendingMessage;
        private bool _isThinkingHidden = true;

        public event PropertyChangedEventHandler PropertyChanged;

        public bool IsThinkingHidden
        {
            get => _isThinkingHidden;
            set
            {
                if (_isThinkingHidden != value)
                {
                    _isThinkingHidden = value;

                    // Update the global state
                    ChatMessage.GlobalIsThinkingHidden = _isThinkingHidden;

                    OnPropertyChanged(nameof(IsThinkingHidden));

                    // Notify all messages to re-evaluate their visibility
                    foreach (var message in _messages)
                    {
                        message.NotifyVisibilityChanged();
                    }
                }
            }
        }


        public ChatWindow()
        {
            InitializeComponent();
            DataContext = this; // Set the DataContext for binding

            // Initialize chat messages
            _messages = new ObservableCollection<ChatMessage>();
            ChatMessages.ItemsSource = _messages;

            // Link global state
            ChatMessage.GlobalIsThinkingHidden = IsThinkingHidden;

            // Add welcome message
            _messages.Add(new ChatMessage
            {
                Role = "Assistant",
                Content = "Hello! How can I help you today?",
                Timestamp = DateTime.Now,
                IsUserMessage = false
            });

            MessageInput.Focus();
        }

        /// <summary>
        /// Sets the current service (LocalLlmService or AgentService) dynamically.
        /// </summary>
        /// <param name="service">The service to set (must be LocalLlmService or AgentService).</param>
        public void SetService(object service)
        {
            if (service is LocalLlmService)
            {
                _currentService = service;
            }
            else
            {
                throw new ArgumentException("Invalid service type. Must be LocalLlmService or AgentService.");
            }
        }

        private async void SendButton_Click(object sender, RoutedEventArgs e)
        {
            await SendMessage();
        }

        private async void MessageInput_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                if (Keyboard.Modifiers == ModifierKeys.Shift)
                {
                    // Allow Shift + Enter to insert a new line
                    e.Handled = false; // Let the TextBox handle the new line
                }
                else
                {
                    // Handle Enter to send the message
                    e.Handled = true; // Prevent default behavior
                    await SendMessage();
                }
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
                    Role = "You",
                    Content = messageText,
                    Timestamp = DateTime.Now,
                    IsUserMessage = true
                });

                // Clear input
                MessageInput.Clear();

                // Scroll to bottom
                ChatScrollViewer.ScrollToBottom();

                // Show "thinking..." indicator
                ThinkingIndicator.Visibility = Visibility.Visible;

                string response = await SendMessageToService(messageText);

                // Add response message
                _messages.Add(new ChatMessage
                {
                    Role = "Assistant",
                    Content = response,
                    Timestamp = DateTime.Now,
                    IsUserMessage = false
                });

                // Scroll to bottom
                ChatScrollViewer.ScrollToBottom();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                // Hide "thinking..." indicator
                ThinkingIndicator.Visibility = Visibility.Collapsed;

                _isSendingMessage = false;
                SendButton.IsEnabled = true;
                MessageInput.IsEnabled = true;
                MessageInput.Focus();
            }
        }

        /// <summary>
        /// Sends a message to the currently set service (LocalLlmService or AgentService).
        /// </summary>
        /// <param name="message">The message to send.</param>
        /// <returns>The response from the service.</returns>
        private async Task<string> SendMessageToService(string message)
        {
            if (_currentService is LocalLlmService localLlmService)
            {
                // Set the system prompt if it exists
                string systemPrompt = "Your system prompt here"; // Replace with the actual system prompt logic
                localLlmService.SystemPrompt = systemPrompt;

                // Send the message
                return await localLlmService.SendMessageAsync(message);
            }
            else
            {
                throw new InvalidOperationException("No valid service is set.");
            }
        }

        
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public void ClearMessages()
        {
            _messages.Clear();
        }

        public void ClearDialogHistory()
        {
            _messages.Clear(); 
        }
    }
}
