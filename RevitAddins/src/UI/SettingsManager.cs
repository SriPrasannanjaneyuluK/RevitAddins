using System;
using System.Configuration;
using System.Linq;

namespace RevitAddins.UI
{
    public static class SettingsManager
    {
        public static string Get(string key, string defaultValue = "")
        {
            var value = ConfigurationManager.AppSettings[key];
            return string.IsNullOrEmpty(value) ? defaultValue : value;
        }

        public static double GetDouble(string key, double defaultValue = 0)
        {
            var value = Get(key);
            return double.TryParse(value, out double d) ? d : defaultValue;
        }

        public static int[] GetIntArray(string key, string defaultValue = "")
        {
            var value = Get(key, defaultValue);
            return value.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                        .Select(v => int.TryParse(v.Trim(), out int i) ? i : 0)
                        .Where(i => i > 0)
                        .ToArray();
        }
    }
}
