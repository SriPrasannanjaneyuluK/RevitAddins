using Autodesk.Revit.UI;
using RevitAddins.UI;

namespace RevitAddins.Panels
{
    public class DocumentToolsPanel
    {
        public static RibbonPanel Create(UIControlledApplication a, string tabName)
        {
            RibbonPanel panel = RibbonHelper.CreateRibbonPanel(a, tabName, "Document Tools");

            // Create the primary button for this panel
            PushButtonData docTitleButtonData = RibbonHelper.CreatePushButton(
                "GetDocTitleId", "Get Doc\nTitle", "RevitAddins.Commands.GetDocTitleCommand", "doc_icon.png");

            panel.AddItem(docTitleButtonData);

            // Add a separator to space out the next set of buttons
            panel.AddSeparator();

            // You can add more buttons here to fill the space
            // PushButtonData button4 = RibbonHelper.CreatePushButton(...);
            // panel.AddItem(button4);

            return panel;
        }
    }
}