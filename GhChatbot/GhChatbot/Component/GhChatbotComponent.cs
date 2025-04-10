using System;
using System.Collections.Generic;
using System.Windows.Forms;
using Grasshopper;
using Grasshopper.Kernel;
using Rhino.Geometry;
using GhChatbot.UI;

namespace GhChatbot.Component
{
    public class GhChatbotComponent : GH_Component
    {
        private string _apiKey = string.Empty;

        /// <summary>
        /// Each implementation of GH_Component must provide a public 
        /// constructor without any arguments.
        /// Category represents the Tab in which the component will appear, 
        /// Subcategory the panel. If you use non-existing tab or panel names, 
        /// new tabs/panels will automatically be created.
        /// </summary>
        public GhChatbotComponent()
          : base("AI Chatbot", "Chatbot",
            "Opens an AI chatbot window using the provided API key",
            "IAAC", "AI Tools")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddTextParameter("API Key", "key", "API Key for Claude or similar AI service", GH_ParamAccess.item);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            // No outputs needed
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object can be used to retrieve data from input parameters and 
        /// to store data in output parameters.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            string apiKey = string.Empty;
            if (!DA.GetData(0, ref apiKey)) return;

            _apiKey = apiKey;
        }

        public override void AppendAdditionalMenuItems(ToolStripDropDown menu)
        {
            base.AppendAdditionalMenuItems(menu);
            Menu_AppendItem(menu, "Open Chat Window", OnOpenChatWindow);
        }

        private void OnOpenChatWindow(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(_apiKey))
            {
                MessageBox.Show("Please provide a valid API key", "Missing API Key",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var chatWindow = new ChatWindow(_apiKey);
            chatWindow.Show();
        }

        public override void CreateAttributes()
        {
            m_attributes = new GhChatbotComponentAttributes(this);
        }

        /// <summary>
        /// Provides an Icon for every component that will be visible in the User Interface.
        /// Icons need to be 24x24 pixels.
        /// </summary>
        protected override System.Drawing.Bitmap Icon
        {
            get
            {
                //You can add image files to your project resources and access them like this:
                // return Resources.IconForThisComponent;
                return null;
            }
        }

        /// <summary>
        /// Each component must have a unique Guid to identify it. 
        /// It is vital this Guid doesn't change otherwise old ghx files 
        /// that use the old ID will partially fail during loading.
        /// </summary>
        public override Guid ComponentGuid => new Guid("2a6b62e9-1c83-42f7-ade4-7ed928410e4a");
    }

    public class GhChatbotComponentAttributes : Grasshopper.Kernel.Attributes.GH_ComponentAttributes
    {
        public GhChatbotComponentAttributes(GhChatbotComponent owner) : base(owner) { }

        public override Grasshopper.GUI.Canvas.GH_ObjectResponse RespondToMouseDoubleClick(Grasshopper.GUI.Canvas.GH_Canvas sender, Grasshopper.GUI.GH_CanvasMouseEvent e)
        {
            GhChatbotComponent comp = Owner as GhChatbotComponent;
            if (comp != null)
            {
                comp.GetType().GetMethod("OnOpenChatWindow", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                    ?.Invoke(comp, new object[] { this, EventArgs.Empty });
            }
            return Grasshopper.GUI.Canvas.GH_ObjectResponse.Handled;
        }
    }
}