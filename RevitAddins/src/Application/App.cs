using Autodesk.Revit.UI;
using RevitAddins.Panels;

namespace RevitAddins
{
    public class App : IExternalApplication
    {
        public Result OnStartup(UIControlledApplication a)
        {
            string tabName = "10 Addin";
            a.CreateRibbonTab(tabName);

            GeneralToolsPanel.Create(a, tabName);
            DocumentToolsPanel.Create(a, tabName);
            FireModelingPanel.Create(a, tabName); // Fire Modeling panel

            return Result.Succeeded;
        }

        public Result OnShutdown(UIControlledApplication a)
        {
            return Result.Succeeded;
        }
    }
}
