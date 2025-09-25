using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Plumbing;
using Autodesk.Revit.UI;
using RevitAddins.Helpers;  // ✅ use the Helpers version
using RevitAddins.UI;
using System;
using System.Linq;
using System.Windows.Forms;
using RevitAddins.Helpers;


namespace RevitAddins.Application.Commands
{
    [Transaction(TransactionMode.Manual)]
    public class FirePipeCreation : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIDocument uidoc = commandData.Application.ActiveUIDocument;
            Document doc = uidoc.Document;

            // Ask user to select CAD
            Reference cadRef = uidoc.Selection.PickObject(
                Autodesk.Revit.UI.Selection.ObjectType.Element,
                new CADSelectionFilter(),
                "Select a CAD file");

            if (cadRef == null) return Result.Cancelled;

            ImportInstance cadInstance = doc.GetElement(cadRef) as ImportInstance;
            if (cadInstance == null)
            {
                TaskDialog.Show("Error", "Selected element is not a valid CAD import/link.");
                return Result.Failed;
            }

            // ✅ Get the CAD file name properly
            string cadName = CADLayerUtils.GetCadFileName(cadInstance);

            // ✅ Extract layers
            string[] cadLayers = CADLayerUtils.GetCadLayers(cadInstance);

            // Get all pipe types (name → ElementId)
            var pipeDict = PipeUtils.GetPipeTypeNames(doc);

            // Populate ComboBox with names only
            string[] pipeTypes = pipeDict.Keys.OrderBy(n => n).ToArray();
            // Collect system types
            string[] systemTypes = new FilteredElementCollector(doc)
                .OfClass(typeof(PipingSystemType))
                .Cast<PipingSystemType>()
                .Select(s => s.Name)
                .ToArray();

            // Collect levels
            Level[] levels = new FilteredElementCollector(doc)
                .OfClass(typeof(Level))
                .Cast<Level>()
                .ToArray();

            // Show the settings form
            FirePipeSettingsForm form = new FirePipeSettingsForm(doc, cadName, cadLayers, pipeTypes, systemTypes, levels);
            if (form.ShowDialog() == DialogResult.OK)
            {
                string selectedLayer = form.SelectedLayer;
                string selectedPipeType = form.SelectedPipeType;
                string selectedSystemType = form.SelectedSystemType;
                Level selectedLevel = form.SelectedLevel;
                double diameter = form.Diameter;
                double offset = form.Offset;

                TaskDialog.Show("Info",
                    $"CAD: {cadName}\n" +
                    $"Layer: {selectedLayer}\n" +
                    $"Pipe Type: {selectedPipeType}\n" +
                    $"System: {selectedSystemType}\n" +
                    $"Level: {selectedLevel.Name}\n" +
                    $"Diameter: {diameter} mm\n" +
                    $"Offset: {offset} mm");

                // TODO: Implement actual pipe creation
            }

            return Result.Succeeded;
        }
    }

    // ✅ Selection filter only for CAD imports/links
    public class CADSelectionFilter : Autodesk.Revit.UI.Selection.ISelectionFilter
    {
        public bool AllowElement(Element elem) => elem is ImportInstance;
        public bool AllowReference(Reference reference, XYZ position) => false;
    }
}
