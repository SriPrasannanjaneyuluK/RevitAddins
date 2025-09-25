using Autodesk.Revit.DB;
using System.Collections.Generic;
using System.Linq;

namespace RevitAddins.Helpers
{
    public static class CADLayerUtils
    {
        public static string GetCadFileName(ImportInstance importInstance)
        {
            if (importInstance == null) return "Unknown CAD";

            // 1. Try external file reference (works for Linked CAD)
            try
            {
                ExternalFileReference extRef = importInstance.GetExternalFileReference();
                if (extRef != null)
                {
                    string path = ModelPathUtils.ConvertModelPathToUserVisiblePath(extRef.GetPath());
                    if (!string.IsNullOrEmpty(path) && path != "<Not Shared>")
                        return System.IO.Path.GetFileName(path); // actual DWG file name
                }
            }
            catch
            {
                // ignore for Imported CAD
            }

            // 2. If no external reference → fallback to instance name
            //    (This is exactly what your pyRevit script uses)
            return importInstance.Name;
        }

        public static string[] GetCadLayers(ImportInstance importInstance)
        {
            var layers = new HashSet<string>();

            if (importInstance?.Category?.SubCategories != null)
            {
                foreach (Category subCat in importInstance.Category.SubCategories)
                {
                    layers.Add(subCat.Name);
                }
            }

            return layers.Count > 0 ? layers.OrderBy(n => n).ToArray() : new string[] { "DefaultLayer" };
        }
    }
}
