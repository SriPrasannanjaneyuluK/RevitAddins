using Autodesk.Revit.DB;
using Autodesk.Revit.UI.Selection;

namespace RevitAddins.Helpers
{
    /// <summary>
    /// Filter to allow only CAD import instances to be selected
    /// </summary>
    public class CADSelectionFilter : ISelectionFilter
    {
        public bool AllowElement(Element elem)
        {
            return elem is ImportInstance; // Only allow imported CAD
        }

        public bool AllowReference(Reference reference, XYZ position) => false;
    }
}
