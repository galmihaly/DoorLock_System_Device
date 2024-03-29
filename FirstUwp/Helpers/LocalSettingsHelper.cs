﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.NetworkInformation;
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

        public static string GetMacAddress()
        {
            string hardwareIdString = "";

            foreach (NetworkInterface nic in NetworkInterface.GetAllNetworkInterfaces())
            {
                if (nic.NetworkInterfaceType == NetworkInterfaceType.Ethernet)
                {
                    if (nic.OperationalStatus == OperationalStatus.Up)
                    {
                        hardwareIdString += nic.GetPhysicalAddress().ToString();
                    }
                }
            }


            if (hardwareIdString.Equals(null)) return null;

            return hardwareIdString;
        }

        public static string MACAddressConnectWidthCharachter(string _macaddress, char _charachter) 
        {
            string seged = "";

            for (int i = 0; i < _macaddress.Length - 2; i += 2)
            {
                seged += _macaddress[i];
                seged += _macaddress[i + 1];
                seged += _charachter;
            }
            seged += _macaddress[_macaddress.Length - 2];
            seged += _macaddress[_macaddress.Length - 1];

            if(seged == null) return null;

            return seged;
        }
    }
}
