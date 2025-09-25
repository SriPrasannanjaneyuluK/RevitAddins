using Autodesk.Revit.UI;
using RevitAddins.UI;
using System.Reflection;
using System.Windows.Media.Imaging;

namespace RevitAddins.Panels
{
    public class FireModelingPanel
    {
        public static RibbonPanel Create(UIControlledApplication a, string tabName)
        {
            RibbonPanel panel = RibbonHelper.CreateRibbonPanel(a, tabName, "Fire Modeling");

            string assemblyPath = Assembly.GetExecutingAssembly().Location;

            PushButtonData pipeCreateButtonData = new PushButtonData(
                "PipeCreateId",
                "Pipe\nCreate",
                assemblyPath,
                "RevitAddins.Application.Commands.FirePipeCreation"
            );


            pipeCreateButtonData.LargeImage = new BitmapImage(new System.Uri(
                "pack://application:,,,/RevitAddins;component/Resources/fire_icon.png"));

            panel.AddItem(pipeCreateButtonData);

            return panel;
        }
    }
}
