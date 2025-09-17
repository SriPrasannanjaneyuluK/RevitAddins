using System;
using System.Reflection;
using System.Windows.Media.Imaging;
using Autodesk.Revit.UI;

namespace RevitAddins
{
    public class App : IExternalApplication
    {
        public Result OnStartup(UIControlledApplication a)
        {
            string tabName = "10 Addins";
            a.CreateRibbonTab(tabName);

            RibbonPanel panel = a.CreateRibbonPanel(tabName, "Tools");

            string assemblyPath = Assembly.GetExecutingAssembly().Location;

            // 1. Create the 'Hello World' button as before
            PushButtonData button1Data = new PushButtonData(
                "Command1Id", "Hello\nWorld", assemblyPath, "RevitAddins.Commands.Command1");

            // 2. Create the 'Count Doors' button
            PushButtonData button2Data = new PushButtonData(
                "CountDoorsId", "Count\nDoors", assemblyPath, "RevitAddins.Commands.CountDoorsCommand");

            // 3. Create the 'Get Document Title' button
            PushButtonData button3Data = new PushButtonData(
                "GetDocTitleId", "Get Doc\nTitle", assemblyPath, "RevitAddins.Commands.GetDocTitleCommand");

            // Add the icon to the button
            button2Data.LargeImage = new BitmapImage(new Uri("pack://application:,,,/RevitAddins;component/Resources/door_icon.png"));
            button3Data.LargeImage = new BitmapImage(new Uri("pack://application:,,,/RevitAddins;component/Resources/doc_icon.png"));

            // Add all the buttons to the panel
            panel.AddItem(button1Data);
            panel.AddItem(button2Data);
            panel.AddItem(button3Data);

            return Result.Succeeded;
        }

        public Result OnShutdown(UIControlledApplication a)
        {
            return Result.Succeeded;
        }
    }
}