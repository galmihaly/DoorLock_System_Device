using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Iot.Device.Pn532;
using Iot.Device.Pn532.ListPassive;
using System.IO.Ports;
using System.Device.Gpio;
using System.Threading;
using Windows.Devices.SerialCommunication;
using Windows.Storage.Streams;
using System.Threading.Tasks;
using Windows.Devices.Enumeration;
using System.Diagnostics;
using Communicator;
using Iot.Device.Card.Mifare;
using Iot.Device.FtCommon;
using System.Device.Spi;
using System.Device.I2c;
using FirstUwp.Classes;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace FirstUwp
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {

        /*
         * A MainPage dokumentációja:
         * 
         * - Az eszköz elindításakor megjelenő kezdőképernyő felületét tartalmazza
         * - az oldal felülete még nincs kész, viszont tartalmazza az NFC olvasáshoz szükésges kódot
         *      - működése: 2 másodpercenként képes beolvasni egy NFC kódot (a két másodperc helyett még 10, amit még ki kell javítani)
         */


        /*
         * az nfcId, az NFC olvasó osztály(saját osztály), egy időzítő, a Raspberry Pi GPIO-ait kezelő osztály és a LED bekötéséhez használt pin definiálása  
         */
        string nfcId;
        //ledstring prevnfcId = "";
        private NfcReader nr = null;
        private System.Timers.Timer timer=null;
        private GpioController gc = null;
        int ledPin = 18;

        public MainPage()
        {
            this.InitializeComponent();

        }


        /*
         * - a Page_Loaded eseménykezelő függvény a program indulásával együtt töltődik be
         * - az NFC olvasó osztály, valamint Gpio kontroller példányosítása, valamint a LED pin kinyitása a Raspberry Pi-n
         * - Timer beállítása 0.5 másodpercre, valamint az idő eltelését szabályozó függvény meghívása és az óra engedélyezése
         * - kezeljük a hibákat(kivételeket) -> ui.: még nincs az összes hiba lekezelve
         */
        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                nr = new NfcReader();
                gc = new GpioController();
                gc.OpenPin(ledPin, PinMode.Output);
                gc.Write(ledPin, PinValue.Low);

                timer = new System.Timers.Timer(2000);
                timer.Elapsed += Timer_Elapsed;
                timer.Enabled = true;
            }
            catch (Exception en)
            {
                Debug.WriteLine(en.Message);
            }

        }
       
        /*
         * - Itt kezeljük az idő eltelésének módját, valamint az NFC olvasását és annak módját 
         * - NFC ID olvasásakor 2 másodpercig (valóságban 10, de javítani kell még) felkapcsolunk egy LED, ami azt jelzi, hogy ha történi egy beolvaás, akkor addig nem töltődhet be
         * az NFC olvasó puffejébe másik NFC ID, tehát meggátoljuk azt, hogy folyamatos kártya olvasás ne terheljje le az olvasót
         * - LED felvillanása a felhasználó számára pontosabban azt jelzi, hogy egy NFC ID olvasás során bekerült az id az olvasóba, ha pedig a LED elalszik, akkor ismét lehet 
         * beolvastatni NFC id => a felhasználó pontosan fogja tudni, hogy mikor lehet odatenni a kártyát olvasásra és mikor nem
         */

        private void Timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {

            nfcId = nr.GetNfcId();

            if (!string.IsNullOrEmpty(nfcId))
            {
                
                Debug.WriteLine("Beolvasott NfcId: " + nfcId);

                gc.Write(ledPin, PinValue.High);
                

            }
            else
            {
                
                Debug.WriteLine("Nincs beolvasott NFC kód!");
                gc.Write(ledPin, PinValue.Low);
                
            }

        }

       
    }
}
