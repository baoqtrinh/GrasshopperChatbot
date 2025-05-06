# GhChatbot

GhChatbot is a Grasshopper plugin for Rhino that provides an AI chatbot interface powered by a local LLM (Large Language Model) endpoint, such as LM Studio. The plugin allows users to interact with an AI assistant directly within Grasshopper, with support for dialog history, system prompts, and a user-friendly chat window.

## Features

- Connects to a local LLM endpoint via HTTP.
- Customizable system prompt for guiding the assistant.
- Maintains dialog history and outputs it as JSON.
- User-friendly WPF chat window with message formatting and "thinking" indicators.
- Easily clear dialog history from Grasshopper or the chat window.

## Getting Started

### Prerequisites

- Rhino 8 with Grasshopper.
- .NET 7.0+ (for plugin compatibility).
- A local LLM endpoint (e.g., [LM Studio](https://lmstudio.ai/)) running and accessible via HTTP.

### Installation

1. Build the solution in Visual Studio or using the provided tasks in VS Code.
2. Copy the generated `GhChatbot.gha` file from `GhChatbot/bin/Debug/` to your Grasshopper Libraries folder.
3. Launch Rhino and open Grasshopper.

### Usage

1. Add the **Local LLM Chatbot** component from the "Params > AI" tab in Grasshopper.
2. Set the **Local LLM Endpoint** input to your local LLM server URL (e.g., `http://localhost:1234/v1/chat/completions`).
3. (Optional) Set a **System Prompt** to guide the assistant's behavior.
4. Use the **Clear Dialog** input to reset the conversation.
5. Right-click the component and select **Open Chat Window** to start chatting with the AI assistant.

### Outputs

- **Status**: Shows the initialization and status messages.
- **Dialog**: Outputs the dialog history as a JSON string.

## Development

- The main logic is in [`GhLocalChatbot`](GhChatbot/GhChatbot/Chatbot/Component/GhLocalChatbot.cs).
- The chat window UI is in [`ChatWindow.xaml`](GhChatbot/GhChatbot/Chatbot/UI/ChatWindow.xaml) and its code-behind.
- Message models and converters are in the `Chatbot/Models` and `Chatbot/UI/Converters` folders.

## License

This project is licensed under the MIT License. See [LICENSE](LICENSE) for details.

## Acknowledgements

- [Grasshopper](https://www.grasshopper3d.com/)
- [LM Studio](https://lmstudio.ai/)