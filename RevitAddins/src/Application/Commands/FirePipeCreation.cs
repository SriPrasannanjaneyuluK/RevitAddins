using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Plumbing;
using Autodesk.Revit.UI;
using RevitAddins.UI;
using System.Linq;
using WF = System.Windows.Forms;

namespace RevitAddins.Commands
{
    [Transaction(TransactionMode.Manual)]
    public class FirePipeCreation : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            var uiDoc = commandData.Application.ActiveUIDocument;
            var doc = uiDoc.Document;

            // Select CAD element
            var selectedIds = uiDoc.Selection.GetElementIds();
            if (!selectedIds.Any())
            {
                WF.MessageBox.Show("Please select a CAD element first.", "Error", WF.MessageBoxButtons.OK, WF.MessageBoxIcon.Error);
                return Result.Failed;
            }

            var cadElement = doc.GetElement(selectedIds.First());
            string cadElementId = cadElement.Name;

            // Get CAD layers (example using ImportInstance)
            string[] cadLayers = new string[] { "Layer1", "Layer2", "Layer3" }; // replace with real CAD layer extraction

            using (WF.Form form = new FirePipeSettingsForm(doc, cadElementId, cadLayers))
            {
                if (form.ShowDialog() != WF.DialogResult.OK)
                    return Result.Cancelled;

                string selectedLayer = ((FirePipeSettingsForm)form).SelectedLayer;
                string selectedPipe = ((FirePipeSettingsForm)form).SelectedPipeType;
                string systemType = ((FirePipeSettingsForm)form).SelectedSystemType;
                Level level = ((FirePipeSettingsForm)form).SelectedLevel;
                double diameter = ((FirePipeSettingsForm)form).Diameter;
                double offset = ((FirePipeSettingsForm)form).Offset;

                using (Transaction t = new Transaction(doc, "Create Fire Pipe"))
                {
                    t.Start();
                    // Create pipe logic here
                    t.Commit();
                }
            }

            return Result.Succeeded;
        }
    }
}
