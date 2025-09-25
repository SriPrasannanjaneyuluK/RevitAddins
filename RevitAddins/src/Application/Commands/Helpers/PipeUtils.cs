using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Plumbing;
using System.Collections.Generic;
using System.Linq;

namespace RevitAddins.Helpers
{
    public static class PipeUtils
    {
        /// <summary>
        /// Fetches all pipe types in the document with meaningful names from parameters.
        /// </summary>
        public static Dictionary<string, ElementId> GetPipeTypeNames(Document doc)
        {
            var pipeDict = new Dictionary<string, ElementId>();

            var pipes = new FilteredElementCollector(doc)
                .OfClass(typeof(PipeType))
                .Cast<PipeType>();

            foreach (var pipe in pipes)
            {
                string pipeName = null;

                try
                {
                    // ParameterSet → cast to IEnumerable<Parameter>
                    var paramDict = pipe.Parameters
                        .Cast<Parameter>()
                        .Where(p => p.HasValue)
                        .ToDictionary(p => p.Definition.Name, p => p.AsString());

                    if (paramDict.ContainsKey("Type IfcGUID") && !string.IsNullOrEmpty(paramDict["Type IfcGUID"]))
                        pipeName = paramDict["Type IfcGUID"];
                    else if (paramDict.ContainsKey("Model") && !string.IsNullOrEmpty(paramDict["Model"]))
                        pipeName = paramDict["Model"];
                    else
                        pipeName = pipe.Name ?? "Unknown";

                    if (!pipeDict.ContainsKey(pipeName))
                        pipeDict.Add(pipeName, pipe.Id);
                }
                catch
                {
                    if (!pipeDict.ContainsKey(pipe.Name))
                        pipeDict.Add(pipe.Name ?? "Unknown", pipe.Id);
                }
            }

            return pipeDict;
        }
    }
}
