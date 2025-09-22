using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Plumbing;
using Autodesk.Revit.UI;
using RevitAddins.UI;
using System.Linq;
using System.Windows.Forms; // WinForms reference

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
                var form = new FirePipeSettingsForm(doc);

                if (form.ShowDialog() != System.Windows.Forms.DialogResult.OK)
                    return Result.Cancelled;

                PipeType pipeType = new FilteredElementCollector(doc).OfClass(typeof(PipeType))
                    .Cast<PipeType>().First(x => x.Name == form.SelectedPipeType);

                PipingSystemType systemType = new FilteredElementCollector(doc).OfClass(typeof(PipingSystemType))
                    .Cast<PipingSystemType>().First(x => x.Name == form.SelectedSystemType);

                Level level = form.SelectedLevel;
                double diameterInternal = UnitUtils.ConvertToInternalUnits(form.Diameter, UnitTypeId.Millimeters);

                XYZ startPoint = uidoc.Selection.PickPoint("Pick start point for pipe");
                XYZ endPoint = uidoc.Selection.PickPoint("Pick end point for pipe");

                using (Transaction t = new Transaction(doc, "Create Fire Pipe"))
                {
                    t.Start();
                    Pipe pipe = Pipe.Create(doc, systemType.Id, pipeType.Id, level.Id, startPoint, endPoint);
                    pipe.get_Parameter(BuiltInParameter.RBS_PIPE_DIAMETER_PARAM)?.Set(diameterInternal);
                    t.Commit();
                }

                TaskDialog.Show("Success", $"✅ Pipe created!\nType: {pipeType.Name}\nSystem: {systemType.Name}\nLevel: {level.Name}\nDiameter: {form.Diameter} mm");

                return Result.Succeeded;
            }
            catch (Autodesk.Revit.Exceptions.OperationCanceledException)
            {
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
