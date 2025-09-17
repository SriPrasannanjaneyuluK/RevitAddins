using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.Attributes;

namespace RevitAddins.Commands
{
    [Transaction(TransactionMode.Manual)]
    public class GetDocTitleCommand : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            // Get the active document
            UIDocument uidoc = commandData.Application.ActiveUIDocument;
            Document doc = uidoc.Document;

            // Get the document title and show it
            TaskDialog.Show("Document Info", $"The current document title is:\n{doc.Title}");

            return Result.Succeeded;
        }
    }
}