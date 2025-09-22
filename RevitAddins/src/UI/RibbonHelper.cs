using System;
using System.Reflection;
using System.Windows.Media.Imaging;
using Autodesk.Revit.UI;

namespace RevitAddins.UI
{
    public static class RibbonHelper
    {
        // This method creates a PushButton with a consistent style
        public static PushButtonData CreatePushButton(string commandId, string buttonText, string commandClass, string imageName)
        {
            string assemblyPath = Assembly.GetExecutingAssembly().Location;
            PushButtonData buttonData = new PushButtonData(commandId, buttonText, assemblyPath, commandClass);

            // Set consistent large image size and path
            if (!string.IsNullOrEmpty(imageName))
            {
                buttonData.LargeImage = new BitmapImage(new Uri($"pack://application:,,,/RevitAddins;component/Resources/{imageName}"));
            }

            // Set a consistent tooltip if you want
            buttonData.ToolTip = buttonText.Replace("\n", " ") + " Tool";

            return buttonData;
        }

        // This method creates a RibbonPanel with a consistent name and style
        public static RibbonPanel CreateRibbonPanel(UIControlledApplication a, string tabName, string panelName)
        {
            RibbonPanel panel = a.CreateRibbonPanel(tabName, panelName);
            return panel;
        }
    }
}