using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using RevitAddins.Helpers;
using RevitAddins.UI;
using System.Collections.Generic;
using System.Windows.Forms;

namespace RevitAddins.Application.Commands
{
    [Transaction(TransactionMode.Manual)]
    public class FirePipeCreation : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIDocument uidoc = commandData.Application.ActiveUIDocument;
            Document doc = uidoc.Document;

            Reference cadRef = uidoc.Selection.PickObject(Autodesk.Revit.UI.Selection.ObjectType.Element, new CADSelectionFilter(), "Select a CAD file");
            if (cadRef == null) return Result.Cancelled;

            var cadInstance = doc.GetElement(cadRef) as Autodesk.Revit.DB.ImportInstance;
            if (cadInstance == null) { TaskDialog.Show("Error", "Selected element is not a valid CAD."); return Result.Failed; }

            string cadName = CADLayerUtils.GetCadFileName(cadInstance);
            string[] cadLayers = CADLayerUtils.GetCadLayers(cadInstance);
            Dictionary<string, Autodesk.Revit.DB.ElementId> pipeDict = PipeUtils.GetPipeTypeNames(doc);

            using (var form = FormFactory.CreatePipeSettingsForm(doc, cadName, cadLayers, pipeDict))
            {
                if (form.ShowDialog() == DialogResult.OK)
                {
                    // Extract values from form controls if needed
                }
            }

            return Result.Succeeded;
        }
    }
}
