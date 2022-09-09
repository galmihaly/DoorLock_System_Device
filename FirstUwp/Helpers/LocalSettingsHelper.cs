using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FirstUwp.Helpers
{
    public static class LocalSettingsHelper
    {
        // string functions
        public static bool Get(string key, ref string stringValue, string defaultValue)
        {
            var localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
            object oString = localSettings.Values[key];
            if (oString != null && !string.IsNullOrEmpty(oString.ToString()))
            {
                stringValue = oString.ToString();
                return true;
            }
            else
            {
                stringValue = defaultValue;
                return false;
            }
        }

        public static bool Get(string key, ref string stringValue)
        {
            var localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
            object oString = localSettings.Values[key];
            if (oString != null && !string.IsNullOrEmpty(oString.ToString()))
            {
                stringValue = oString.ToString();
                return true;
            }
            return false;
        }

        public static bool Set(string key, string stringValue)
        {
            var localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
            if (!localSettings.Values.ContainsKey(key))
            {
                localSettings.Values.Add(key, stringValue);
                return false; // false value indicates, that the key wasn'n exist before
            }
            else
            {
                localSettings.Values[key] = stringValue;
                return true;
            }
        }

        // double functions
        public static bool Get(string key, ref double doubleValue, double defaultValue)
        {
            var localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
            object odouble = localSettings.Values[key];
            if (odouble != null && !string.IsNullOrEmpty(odouble.ToString()))
            {
                return double.TryParse(odouble.ToString(), out doubleValue);
            }
            else
            {
                doubleValue = defaultValue;
                return false;
            }
        }

        public static bool Get(string key, ref double doubleValue)
        {
            var localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
            object odouble = localSettings.Values[key];
            if (odouble != null && !string.IsNullOrEmpty(odouble.ToString()))
            {
                return double.TryParse(odouble.ToString(), out doubleValue);
            }
            return false;
        }

        public static bool Set(string key, double doubleValue)
        {
            var localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
            if (!localSettings.Values.ContainsKey(key))
            {
                localSettings.Values.Add(key, doubleValue);
                return false; // false value indicates, that the key wasn'n exist before
            }
            else
            {
                localSettings.Values[key] = doubleValue;
                return true;
            }
        }

        // int functions
        public static bool Get(string key, ref int intValue, int defaultValue)
        {
            var localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
            object oInt = localSettings.Values[key];
            if (oInt != null && !string.IsNullOrEmpty(oInt.ToString()))
            {
                return int.TryParse(oInt.ToString(), out intValue);
            }
            else
            {
                intValue = defaultValue;
                return false;
            }
        }

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

        // bool functions
        public static bool Get(string key, ref bool boolValue, bool defaultValue)
        {
            var localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
            object obool = localSettings.Values[key];
            if (obool != null && !string.IsNullOrEmpty(obool.ToString()))
            {
                return bool.TryParse(obool.ToString(), out boolValue);
            }
            else
            {
                boolValue = defaultValue;
                return false;
            }
        }

        public static bool Get(string key, ref bool boolValue)
        {
            var localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
            object obool = localSettings.Values[key];
            if (obool != null && !string.IsNullOrEmpty(obool.ToString()))
            {
                return bool.TryParse(obool.ToString(), out boolValue);
            }
            return false;
        }

        public static bool Set(string key, bool boolValue)
        {
            var localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
            if (!localSettings.Values.ContainsKey(key))
            {
                localSettings.Values.Add(key, boolValue);
                return false; // false value indicates, that the key wasn'n exist before
            }
            else
            {
                localSettings.Values[key] = boolValue;
                return true;
            }
        }

        // helpers
        public static bool Remove(string key)
        {
            var localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
            if (localSettings.Values.ContainsKey(key))
            {
                localSettings.Values.Remove(key);
                return true;
            }
            return false;
        }
    }
}
