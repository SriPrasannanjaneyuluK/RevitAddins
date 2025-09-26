using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Plumbing;
using Autodesk.Revit.UI;
using RevitAddins.UI;
using RevitAddins.Helpers;
using System.Linq;
using System.Windows.Forms;
using System.Collections.Generic;

namespace RevitAddins.Application.Commands
{
    [Transaction(TransactionMode.Manual)]
    public class FirePipeCreation : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIDocument uidoc = commandData.Application.ActiveUIDocument;
            Document doc = uidoc.Document;

            // Ask user to select CAD import/link
            Reference cadRef = uidoc.Selection.PickObject(Autodesk.Revit.UI.Selection.ObjectType.Element, new CADSelectionFilter(), "Select a CAD file");
            if (cadRef == null) return Result.Cancelled;

            ImportInstance cadInstance = doc.GetElement(cadRef) as ImportInstance;
            if (cadInstance == null)
            {
                TaskDialog.Show("Error", "Selected element is not a valid CAD import/link.");
                return Result.Failed;
            }

            // Get CAD filename and layers via CADLayerUtils (your helper)
            string cadName = CADLayerUtils.GetCadFileName(cadInstance);
            string[] cadLayers = CADLayerUtils.GetCadLayers(cadInstance);

            // Pipe types: name -> ElementId
            Dictionary<string, ElementId> pipeDict = PipeUtils.GetPipeTypeNames(doc);

            // System types (names)
            string[] systemTypes = new FilteredElementCollector(doc)
                .OfClass(typeof(PipingSystemType))
                .Cast<PipingSystemType>()
                .Select(s => s.Name)
                .ToArray();

            // Show form (pass pipeDict)
            using (var form = new FirePipeSettingsForm(doc, cadName, cadLayers, pipeDict, systemTypes))
            {
                if (form.ShowDialog() == DialogResult.OK)
                {
                    string selectedLayer = form.SelectedLayer;
                    string selectedPipeName = form.SelectedPipeTypeName;
                    ElementId selectedPipeTypeId = form.SelectedPipeTypeId;
                    string selectedSystemType = form.SelectedSystemType;
                    ElementId selectedLevelId = form.SelectedLevelId;
                    double diameter = form.Diameter;
                    double offset = form.Offset;

                    Level selectedLevel = doc.GetElement(selectedLevelId) as Level;

                    TaskDialog.Show("Info",
                        $"CAD: {cadName}\n" +
                        $"Layer: {selectedLayer}\n" +
                        $"Pipe Type: {selectedPipeName} (Id: {selectedPipeTypeId.IntegerValue})\n" +
                        $"System: {selectedSystemType}\n" +
                        $"Level: {selectedLevel?.Name}\n" +
                        $"Diameter: {diameter} mm\n" +
                        $"Offset: {offset} mm");

                    // TODO: Implement pipe creation logic using selectedPipeTypeId, selectedLevelId, diameter, etc.
                }
            }

            return Result.Succeeded;
        }
    }

    // Selection filter for CAD imports / links
    public class CADSelectionFilter : Autodesk.Revit.UI.Selection.ISelectionFilter
    {
        public bool AllowElement(Element elem)
        {
            return elem is ImportInstance;
        }

        public bool AllowReference(Autodesk.Revit.DB.Reference reference, Autodesk.Revit.DB.XYZ position)
        {
            return false;
        }
    }
}
