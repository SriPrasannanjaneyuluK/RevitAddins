using Autodesk.Revit.DB;
using System.Collections.Generic;
using System.Linq;

namespace RevitAddins.Helpers
{
    public static class LevelUtils
    {
        // Returns dictionary of display string -> Level.ElementId
        public static Dictionary<string, ElementId> GetLevelDisplayNames(Document doc)
        {
            var levels = new FilteredElementCollector(doc)
                .OfClass(typeof(Level))
                .Cast<Level>()
                .ToList();

            var levelDict = new Dictionary<string, ElementId>();

            foreach (var level in levels)
            {
                // Convert elevation from feet to mm
                double elevationMm = level.Elevation * 304.8;
                string displayName = $"{level.Name} (Height: {elevationMm:0.##} mm)";

                if (!levelDict.ContainsKey(displayName))
                    levelDict.Add(displayName, level.Id); // store ElementId instead of Level
            }

            return levelDict;
        }

        // Active view level as ElementId
        public static ElementId GetActiveViewLevelId(Document doc)
        {
            var activeView = doc.ActiveView;
            if (activeView?.GenLevel != null)
            {
                return activeView.GenLevel.Id;
            }
            return ElementId.InvalidElementId;
        }
    }
}
