using Iot.Device.Nmea0183;
using Iot.Device.Pn532;
using Iot.Device.Pn532.ListPassive;
using System;
using System.Collections.Generic;
using System.Device.I2c;
using System.Device.Spi;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace FirstUwp.Classes
{
    internal class NfcReader
    {
        
        /*
         * - nfcid változó deklarálása, valamint a Pn532 osztály féldányosítása
         * - Pn532 osztály -> ez az osztály a Microsoft hivatalos Nuget package-ben van benne(Iot.Device.Pn532)
         * - olvasáshoz a I2C technológiát használjuk
         */
        private Pn532 pn532 = new Pn532(SpiDevice.Create(new SpiConnectionSettings(0) { DataFlow = DataFlow.LsbFirst, Mode = SpiMode.Mode0 }), 5);
        private string nfcId = null;


        /*
         * - ezzel függvénnyel olvassuk ki az NFC ID-t az olvasó pufferjéből
         */
        public String GetNfcId()
        {
            
            try
            {
                var retData = pn532.ListPassiveTarget(MaxTarget.One, TargetBaudRate.B106kbpsTypeA);

                if (retData is null) return null;

                var decrypted = pn532.TryDecode106kbpsTypeA(retData.AsSpan().Slice(1));

                nfcId = BitConverter.ToString(decrypted.NfcId);

            }
            catch(Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return null;
            }
            
            return nfcId;
        }
    }
}
