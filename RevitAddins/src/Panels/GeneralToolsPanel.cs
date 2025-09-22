using Autodesk.Revit.UI;
using RevitAddins.UI;
using System.Collections.Generic;

namespace RevitAddins.Panels
{
    public class GeneralToolsPanel
    {
        public static RibbonPanel Create(UIControlledApplication a, string tabName)
        {
            RibbonPanel panel = RibbonHelper.CreateRibbonPanel(a, tabName, "General Tools");

            // Create PushButtonData for the stacked items
            PushButtonData countDoorsData = RibbonHelper.CreatePushButton(
                "CountDoorsId", "Count Doors", "RevitAddins.Commands.CountDoorsCommand", "");

            PushButtonData getDocTitleData = RibbonHelper.CreatePushButton(
                "GetDocTitleId", "Get Doc Title", "RevitAddins.Commands.GetDocTitleCommand", "");

            // Create a stack of items with the AddStackedItems method
            IList<RibbonItem> stackedItems = panel.AddStackedItems(
                countDoorsData,
                getDocTitleData);

            return panel;
        }
    }
}