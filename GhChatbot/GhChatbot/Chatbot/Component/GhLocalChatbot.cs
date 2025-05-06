using System;
using System.Collections.Generic;
using System.Windows.Forms;
using Grasshopper;
using Grasshopper.Kernel;
using Grasshopper.GUI.Canvas;
using System.Text.Encodings.Web;
using System.Text.Json;

namespace Glab.C_AI.Chatbot.Component
{
    public class GhLocalChatbot : GH_Component
    {
        private string _localLlmEndpoint = string.Empty;
        private LocalLlmService _localLlmService; // For interacting with the local LLM
        private ChatWindow _chatWindow; // For managing the chat window
        private string _dialogJson = "[]"; // To store the dialog as a JSON string

        public GhLocalChatbot()
          : base("Local LLM Chatbot", "LocalChat",
            "Opens an AI chatbot window using a local LLM endpoint.",
            "Params", "AI")
        {
        }

        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            // Local LLM Endpoint (required)
            pManager.AddTextParameter("Local LLM Endpoint", "endpoint", "Endpoint URL for the local LLM (e.g., LM Studio)", GH_ParamAccess.item);

            // System Prompt (optional)
            pManager.AddTextParameter("System Prompt", "Prompt", "Optional system prompt to guide the LLM", GH_ParamAccess.item, string.Empty);
            pManager[1].Optional = true;

            // Clear Dialog (toggle)
            pManager.AddBooleanParameter("Clear Dialog", "Clear", "Toggle to clear the dialog output record", GH_ParamAccess.item, false);
        }

        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            // Output message to indicate the status of the local LLM
            pManager.AddTextParameter("Status", "Status", "Status and log of the Local LLM", GH_ParamAccess.item);

            // Output JSON dialog record
            pManager.AddTextParameter("Dialog", "Dialog", "Recorded dialog in JSON format", GH_ParamAccess.item);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            string localLlmEndpoint = string.Empty;
            string systemPrompt = string.Empty;
            bool clearDialog = false;

            // Retrieve inputs
            if (!DA.GetData(0, ref localLlmEndpoint))
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "Please provide a Local LLM Endpoint");
                return;
            }
            DA.GetData(1, ref systemPrompt);
            DA.GetData(2, ref clearDialog);

            // Initialize the Local LLM Service
            if (_localLlmService == null || _localLlmEndpoint != localLlmEndpoint)
            {
                _localLlmEndpoint = localLlmEndpoint;
                _localLlmService = new LocalLlmService(_localLlmEndpoint);
            }

            // Set the system prompt
            _localLlmService.SystemPrompt = systemPrompt;

            if (clearDialog)
            {
                _localLlmService.ClearDialogHistory();
                _dialogJson = "[]";
                if (_chatWindow != null && _chatWindow.IsVisible)
                {
                    _chatWindow.ClearMessages();
                }
                return;
            }

            // Retrieve the dialog history from the LocalLlmService
            var dialogHistory = _localLlmService.GetDialogHistory();

            // Serialize the dialog history to JSON
            _dialogJson = SerializeDialogToJson(dialogHistory);

            // Set the output messages
            DA.SetData(0, "Local LLM Service initialized.");
            DA.SetData(1, _dialogJson);
        }

        public void RefreshDialogOutput()
        {
            this.ExpireSolution(true);
        }

        private string SerializeDialogToJson(List<ChatMessage> dialog)
        {
            return JsonSerializer.Serialize(dialog, new JsonSerializerOptions
            {
                WriteIndented = true,
                Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
            });
        }

        public override void AppendAdditionalMenuItems(ToolStripDropDown menu)
        {
            base.AppendAdditionalMenuItems(menu);
            Menu_AppendItem(menu, "Open Chat Window", OnOpenChatWindow);
        }

        private void OnOpenChatWindow(object sender, EventArgs e)
        {
            // Check if endpoint is provided
            if (string.IsNullOrEmpty(_localLlmEndpoint))
            {
                MessageBox.Show("Please provide a Local LLM Endpoint", "Missing Configuration",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                // Create or reuse the chat window
                if (_chatWindow == null || !_chatWindow.IsVisible)
                {
                    _chatWindow = new ChatWindow();
                    _chatWindow.SetService(_localLlmService);
                    _chatWindow.SetHostComponent(this);
                    _chatWindow.Show();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error initializing chat window: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public override void CreateAttributes()
        {
            m_attributes = new GhChatbotComponentAttributes(this);
        }

        protected override System.Drawing.Bitmap Icon
        {
            get
            {
                return null;
            }
        }

        public override Guid ComponentGuid => new Guid("2a6b62e9-1c83-42f7-ade4-7ed928410e4a");
    }

    public class GhChatbotComponentAttributes : Grasshopper.Kernel.Attributes.GH_ComponentAttributes
    {
        public GhChatbotComponentAttributes(GhLocalChatbot owner) : base(owner) { }

        public override GH_ObjectResponse RespondToMouseDoubleClick(GH_Canvas sender, Grasshopper.GUI.GH_CanvasMouseEvent e)
        {
            GhLocalChatbot comp = Owner as GhLocalChatbot;
            if (comp != null)
            {
                comp.GetType().GetMethod("OnOpenChatWindow", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                    ?.Invoke(comp, new object[] { sender, EventArgs.Empty });
            }
            return Grasshopper.GUI.Canvas.GH_ObjectResponse.Handled;
        }
    }
}
