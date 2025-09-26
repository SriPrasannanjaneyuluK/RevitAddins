using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Plumbing;
using System.Linq;

namespace RevitAddins.Helpers
{
    public static class SystemTypeUtils
    {
        /// <summary>
        /// Returns all piping system type names in the document.
        /// </summary>
        public static string[] GetSystemTypeNames(Document doc)
        {
            if (doc == null) return new string[0];

            return new FilteredElementCollector(doc)
                .OfClass(typeof(PipingSystemType))
                .Cast<PipingSystemType>()
                .Select(s => s.Name)
                .OrderBy(n => n)
                .ToArray();
        }
    }
}
