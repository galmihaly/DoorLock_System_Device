using FirstUwp.Classes;
using FirstUwp.Helpers;
using System;
using System.Device.Gpio;
using System.Diagnostics;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI;
using System.Drawing;
using Windows.UI.Xaml.Media;
using System.Threading;
using System.Threading.Tasks;
using FirstUwp.Repository;
using Org.BouncyCastle.Crypto.Tls;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace FirstUwp.UserControls
{
    public sealed partial class LoginControl : UserControl
    {
        public event EventHandler LoginAccepted;
        public event EventHandler LoginCancelled;
        public event EventHandler LoginUserInteraction;
        public event EventHandler result;

        private string nfcId;
        bool isEnabled = true;
        int voicePin = 23;

        private GpioController gpioController = null;
        private NfcReader nfcReader = null;

        //private DispatcherTimer PinCodeTimer = new DispatcherTimer();
        private DispatcherTimer RfidTimer = new DispatcherTimer();
        private DispatcherTimer AlertTimer = new DispatcherTimer();
        private DispatcherTimer TulhamarTimer = new DispatcherTimer();


        public LoginControl()
        {
            this.Loaded += LoginControl_Loaded;

            RfidTimer.Interval = TimeSpan.FromMilliseconds(200);
            RfidTimer.Tick += RfidTimer_Tick;

            AlertTimer.Interval = TimeSpan.FromSeconds(2);
            AlertTimer.Tick += AlertTimer_Tick;

            TulhamarTimer.Interval = TimeSpan.FromSeconds(5);
            TulhamarTimer.Tick += TulhamarTimer_Tick;

            this.InitializeComponent();

            Message.Text = "";

            this.CharacterReceived += LoginControl_CharacterReceived;
            this.PivotControl.SelectionChanged += PivotControl_SelectionChanged;

            int Index = -1;
            if (LocalSettingsHelper.Get("LoginIndex", ref Index))
            {
                PivotControl.SelectedIndex = 0; //index kell ide majd
            }

            handler += OnResult;
            handler2 += OnResult2;
        }

        private void LoginControl_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                nfcReader = new NfcReader();
                gpioController = new GpioController();

                gpioController.OpenPin(voicePin, PinMode.Output);
                gpioController.Write(voicePin, PinValue.Low);

            }
            catch (Exception en)
            {
                Debug.WriteLine(en.Message);
            }
        }

        private void PivotControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Pivot p = sender as Pivot;
            PivotItem pi = p?.SelectedItem as PivotItem;
            if (pi == null) return;
            if (pi.Name.ToLower() == "rfid")
            {
                RfidTimer.Start();
                //PinCodeTimer.Stop();
            }
            else if (pi.Name.ToLower() == "pin")
            {
                RfidTimer.Stop();
                //PinCodeTimer.Start();
            }


            LocalSettingsHelper.Set("LoginIndex", PivotControl.SelectedIndex);
        }

        private void LoginControl_CharacterReceived(UIElement sender, CharacterReceivedRoutedEventArgs args)
        {
            if (args.Character == 13) // Enter
            {
                switch (PivotControl.SelectedIndex)
                {
                    case 0:
                        if (isEnabled == true) PinOk_Click(sender, null);
                        break;
                    case 1:
                        break;
                }
            }
        }

        private void UserInteraction()
        {
            var eventHandler = this.LoginUserInteraction;
            if (eventHandler != null)
            {
                eventHandler(this, EventArgs.Empty);
            }
        }

        private void Input(string inputString)
        {
            UserInteraction();
            PinText.Password += inputString;
        }

        private void Pin1_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            Input("1");
        }

        private void Pin2_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            Input("2");
        }

        private void Pin3_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            Input("3");
        }

        private void Pin4_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            Input("4");
        }

        private void Pin5_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            Input("5");
        }

        private void Pin6_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            Input("6");
        }

        private void Pin7_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            Input("7");
        }

        private void Pin8_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            Input("8");
        }

        private void Pin9_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            Input("9");
        }

        private void Pin0_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            Input("0");
        }

        private void PinBackSpace_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            PinText.Password = "";
            PinText.SelectAll();
            PinText.Focus(Windows.UI.Xaml.FocusState.Programmatic);
        }


        private void PinOk_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            isEnabled = false;
            UserInteraction();
            if (PinText.Password != "")
            {
                Repository.Repository.LoggedInUser = Repository.Repository.Communicator.loginUserByCode(PinText.Password);

                if (Repository.Repository.LoggedInUser != null)
                {

                    if (this.LoginAccepted != null) this.LoginAccepted(this, EventArgs.Empty);

                    if (Repository.Repository.LoggedInUser.LoginId == 200 || Repository.Repository.LoggedInUser.LoginId == 201)
                    {
                        string a = $"Üdvözöllek {Repository.Repository.LoggedInUser.Name}!";
                        Alert(a, Colors.Green);
                        pitypity();
                    }
                    else if (Repository.Repository.LoggedInUser.LoginId == 300 || Repository.Repository.LoggedInUser.LoginId == 301)
                    {
                        string a = $"Viszont látásra {Repository.Repository.LoggedInUser.Name}!";
                        Alert(a, Colors.Green);
                        pitypity();

                    }
                    else if (Repository.Repository.LoggedInUser.IsActive == 0)
                    {
                        string a = $"Ön jelenleg inaktív állapotban van!";
                        Alert(a, Colors.Yellow);
                        piiity();
                    }
                    else if (Repository.Repository.LoggedInUser.IsActive == -1)
                    {
                        string a = $"Nincs ilyen beregisztrált kód!";
                        Alert(a, Colors.Yellow);
                        piiity();
                    }
                    //AlertClr();
                    PinText.Password = "";
                    PinText.SelectAll();
                    PinText.Focus(Windows.UI.Xaml.FocusState.Programmatic);
                }
                else
                {
                    Alert("Az adatbázis nem elérhető!", Colors.Red);
                    PinText.Password = "";
                    PinText.SelectAll();
                    PinText.Focus(Windows.UI.Xaml.FocusState.Programmatic);
                }
            }
            isEnabled = true;
        }

        private string masodlagosMsg;
        private Windows.UI.Color masodlagosColor;

        private void Alert(string msg, Windows.UI.Color color, bool autoHide = true, string msg2 = null, Windows.UI.Color? color2 = null)
        {
            _ = Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
            {
                Message.Text = msg;
                Message.Foreground = new SolidColorBrush(color);
                if (autoHide)
                {
                    masodlagosMsg = msg2;
                    masodlagosColor = (color2.HasValue) ? color2.Value : Colors.Yellow;
                    AlertTimer.Start();
                }
            });
        }

        private void AlertTimer_Tick(object sender, object e)
        {
            AlertTimer.Stop();
            if (!string.IsNullOrEmpty(masodlagosMsg))
            {
                Message.Text = masodlagosMsg;
                Message.Foreground = new SolidColorBrush(masodlagosColor);
            }
            else
            {
                AlertClr();
            }
            // ha van másodlagos message, pl. kérem, várjon, itt azt kell kitenni: Message.Text = msg;
            // törölni kell
            // egyébként AlertClr
        }

        private void AlertClr()
        {
            _ = Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
            {
                Message.Text = "";
            });
        }

        private void pitypity()
        {
            Task.Run(() =>
            {
                gpioController.Write(voicePin, PinValue.High);
                Task.Delay(250).Wait();
                gpioController.Write(voicePin, PinValue.Low);
                Task.Delay(125).Wait();
                gpioController.Write(voicePin, PinValue.High);
                Task.Delay(250).Wait();
                gpioController.Write(voicePin, PinValue.Low);
            });
        }

        private void piiity()
        {
            Task.Run(() =>
            {
                gpioController.Write(voicePin, PinValue.High);
                Task.Delay(625).Wait();
                gpioController.Write(voicePin, PinValue.Low);
            });
        }

        private void pity()
        {
            Task.Run(() =>
            {
                gpioController.Write(voicePin, PinValue.High);
                Task.Delay(250).Wait();
                gpioController.Write(voicePin, PinValue.Low);
            });
        }

        
        private object lock1 = new object();


        private void eredmenykiertekeles()
        {
            lock (lock1)
            {
                var e = new Model1();
                var e2 = new Model2();
                // beolvasás...
                string nfcId = nfcReader.GetNfcId();
                // beolvasva...               
                if (!string.IsNullOrEmpty(nfcId))
                {
                    pity();
                    e.isNfcId = true;
                    // beléptetés...
                    e2.message = "beléptetés...";
                    handler2?.Invoke(this, e2);
                    Repository.Repository.LoggedInUser = Repository.Repository.Communicator.loginUserByNFC_Id(nfcId);
                    // beléptetve

                    if (Repository.Repository.LoggedInUser != null)
                    {
                        e.isUser = true;
                        if (this.LoginAccepted != null) this.LoginAccepted(this, EventArgs.Empty);

                        if (Repository.Repository.LoggedInUser.LoginId == 200 || Repository.Repository.LoggedInUser.LoginId == 201)
                        {
                            e.message = $"Üdvözöllek {Repository.Repository.LoggedInUser.Name}!";
                            e.color = Colors.Green;
                            //pity();
                            //pitypity();
                        }
                        else if (Repository.Repository.LoggedInUser.LoginId == 300 || Repository.Repository.LoggedInUser.LoginId == 301)
                        {
                            e.message = $"Viszont látásra {Repository.Repository.LoggedInUser.Name}!";
                            e.color = Colors.Green;
                            //pity();
                            //pitypity();
                        }
                        else if (Repository.Repository.LoggedInUser.IsActive == 0)
                        {
                            e.message = $"Ön jelenleg inaktív állapotban van!";
                            e.color =Colors.Yellow;
                            //pity();
                        }
                        else if (Repository.Repository.LoggedInUser.IsActive == -1)
                        {
                            e.message = $"Nincs ilyen beregisztrált kód!";
                            e.color = Colors.Yellow;
                            //pity();
                        }
                    }
                    else
                    {
                        e.color = Colors.Red;
                        e.message = $"Az adatbázis nem elérhető!";             // nem volt user          
                    }
                }              
               
                handler?.Invoke(this, e);
            }
        }

        public class Model1
        {
            public string message;
            public Windows.UI.Color color;
            public bool isNfcId;
            public bool isUser;
        }

        EventHandler<Model1> handler;

        public void OnResult(object sender, Model1 e)
        {
#if DEBUG
            if (!e.isNfcId) Debug.WriteLine("Nincs beolvasott NFC kód!");
            if (!e.isUser) Debug.WriteLine("Nincs User!");
#endif
            if (!string.IsNullOrEmpty(e.message))
            {
                Alert(e.message, e.color, true, "Kérem várjon!", Colors.Yellow);
            }

            if (e.isUser)
            {
                _ = Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () => TulhamarTimer.Start());
            }
            else
            {
                tulhamar = false;
            }
        }

        public class Model2
        {
            public string message;
            //public Windows.UI.Color color;            
        }

        EventHandler<Model2> handler2;

        public void OnResult2(object sender, Model2 e)
        {
            Alert(e.message, Colors.Yellow, false);
        }

        private void TulhamarTimer_Tick(object sender, object e)
        {
            //Alert("Kérem várjon!", Colors.Yellow, false);
            TulhamarTimer.Stop();
            tulhamar = false;
        }

        bool tulhamar;

        private void RfidTimer_Tick(object sender, object e)
        {
            if (tulhamar == true) return;
            try
            {                
                tulhamar = true;
                Alert("Kérem a kártyáját!", Colors.Yellow, false);
                
                Task.Run(() => eredmenykiertekeles());
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }       
        }
    }
}