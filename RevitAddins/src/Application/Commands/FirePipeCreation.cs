using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Plumbing;
using Autodesk.Revit.UI;
using System.Linq;
using System.Windows.Forms; // For DialogResult
using RevitAddins.UI;        // ✅ This is where your form should live

namespace RevitAddins.Commands
{
    [Transaction(TransactionMode.Manual)]
    public class FirePipeCreation : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIDocument uidoc = commandData.Application.ActiveUIDocument;
            Document doc = uidoc.Document;

            try
            {
                // ✅ Pass document into the form (so dropdowns can be filled with PipeTypes, SystemTypes, Levels)
                using (var form = new FirePipeSettingsForm(doc))
                {
                    if (form.ShowDialog() != DialogResult.OK)
                        return Result.Cancelled;

                    // ✅ Pipe Type
                    PipeType pipeType = new FilteredElementCollector(doc)
                        .OfClass(typeof(PipeType))
                        .Cast<PipeType>()
                        .First(x => x.Name == form.SelectedPipeType);

                    // ✅ System Type
                    PipingSystemType systemType = new FilteredElementCollector(doc)
                        .OfClass(typeof(PipingSystemType))
                        .Cast<PipingSystemType>()
                        .First(x => x.Name == form.SelectedSystemType);

                    // ✅ Level
                    Level level = form.SelectedLevel;

                    // ✅ Diameter
                    double diameterInternal = UnitUtils.ConvertToInternalUnits(
                        form.Diameter, UnitTypeId.Millimeters);

                    // ✅ Pick pipe route
                    XYZ startPoint = uidoc.Selection.PickPoint("Pick start point for pipe");
                    XYZ endPoint = uidoc.Selection.PickPoint("Pick end point for pipe");

                    using (Transaction t = new Transaction(doc, "Create Fire Pipe"))
                    {
                        t.Start();

                        Pipe pipe = Pipe.Create(doc, systemType.Id, pipeType.Id, level.Id, startPoint, endPoint);

                        // Set diameter
                        pipe.get_Parameter(BuiltInParameter.RBS_PIPE_DIAMETER_PARAM)
                            ?.Set(diameterInternal);

                        t.Commit();
                    }

                    TaskDialog.Show("Success",
                        $"✅ Pipe created!\n" +
                        $"Type: {pipeType.Name}\n" +
                        $"System: {systemType.Name}\n" +
                        $"Level: {level.Name}\n" +
                        $"Diameter: {form.Diameter} mm");
                }

                return Result.Succeeded;
            }
            catch (Autodesk.Revit.Exceptions.OperationCanceledException)
            {
                // User pressed ESC while picking points
                return Result.Cancelled;
            }
            catch (System.Exception ex)
            {
                message = ex.Message;
                return Result.Failed;
            }
        }
    }
}
