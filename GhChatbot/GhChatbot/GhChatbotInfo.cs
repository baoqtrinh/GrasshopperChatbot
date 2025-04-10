using System;
using System.Drawing;
using Grasshopper;
using Grasshopper.Kernel;

namespace GhChatbot
{
    public class GhChatbotInfo : GH_AssemblyInfo
    {
        public override string Name => "GhChatbot";

        //Return a 24x24 pixel bitmap to represent this GHA library.
        public override Bitmap Icon => null;

        //Return a short string describing the purpose of this GHA library.
        public override string Description => "";

        public override Guid Id => new Guid("1bc01bd7-fb43-48f5-9288-2f0bc8ae481c");

        //Return a string identifying you or your company.
        public override string AuthorName => "";

        //Return a string representing your preferred contact details.
        public override string AuthorContact => "";

        //Return a string representing the version.  This returns the same version as the assembly.
        public override string AssemblyVersion => GetType().Assembly.GetName().Version.ToString();
    }
}