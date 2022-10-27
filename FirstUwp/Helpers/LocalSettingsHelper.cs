using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FirstUwp.Helpers
{
    public static class LocalSettingsHelper
    {
        public static bool Get(string key, ref int intValue)
        {
            var localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
            object oInt = localSettings.Values[key];
            if (oInt != null && !string.IsNullOrEmpty(oInt.ToString()))
            {
                return int.TryParse(oInt.ToString(), out intValue);
            }
            return false;
        }

        public static bool Set(string key, int intValue)
        {
            var localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
            if (!localSettings.Values.ContainsKey(key))
            {
                localSettings.Values.Add(key, intValue);
                return false; // false value indicates, that the key wasn'n exist before
            }
            else
            {
                localSettings.Values[key] = intValue;
                return true;
            }
        }
    }
}
