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

            Document doc = importInstance.Document;

            // Get the type of this ImportInstance
            ElementId typeId = importInstance.GetTypeId();
            ElementType type = doc.GetElement(typeId) as ElementType;

            if (type != null)
            {
                ExternalFileReference extRef = type.GetExternalFileReference();
                if (extRef != null)
                {
                    ModelPath modelPath = extRef.GetPath();
                    string userPath = ModelPathUtils.ConvertModelPathToUserVisiblePath(modelPath);

                    if (!string.IsNullOrEmpty(userPath) && userPath != "<Not Shared>")
                    {
                        return System.IO.Path.GetFileName(userPath); // e.g. "MySite.dwg"
                    }
                }
            }

            // Fallback
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
