using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.Attributes;
using System.Linq;

namespace RevitAddins.Commands
{
    [Transaction(TransactionMode.ReadOnly)]
    public class CountDoorsCommand : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            // Get the active document
            UIDocument uidoc = commandData.Application.ActiveUIDocument;
            Document doc = uidoc.Document;

            // Use a LINQ query to find all door elements
            var doors = new FilteredElementCollector(doc)
                .OfCategory(BuiltInCategory.OST_Doors)
                .WhereElementIsNotElementType()
                .ToElements();

            // Count the doors and show the result in a TaskDialog
            TaskDialog.Show("Door Counter", $"This project has {doors.Count} doors.");

            return Result.Succeeded;
        }
    }
}