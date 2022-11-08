using FirstUwp.Classes;
using FirstUwp.Helpers;
using FirstUwp.Models;
using System;
using System.Device.Gpio;
using System.Diagnostics;
using System.Threading.Tasks;
using Windows.Networking.Connectivity;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;

namespace FirstUwp.UserControls
{
    public sealed partial class LoginControl : UserControl
    {

        public event EventHandler LoginAccepted;
        public event EventHandler LoginCancelled;
        public event EventHandler LoginUserInteraction;
        public event EventHandler result;

        private DispatcherTimer RfidTimer = new DispatcherTimer();
        private DispatcherTimer AlertTimer = new DispatcherTimer();
        private DispatcherTimer SoonTimer = new DispatcherTimer();

        private EventHandler<Data1> handler1;
        private EventHandler<Data2> handler2;

        private GpioController gpioController = null;
        private NfcReader nfcReader = null;

        private string nfcId;
        private int voicePin = 23;
        private bool soon;
        private bool isEnabled = true;
        private string secondMessage;
        private Windows.UI.Color secondColor;

        private object _lock = new object();
        private object _lock2 = new object();

        public LoginControl()
        {
            this.Loaded += LoginControl_Loaded;

            this.InitializeComponent();

            this.CharacterReceived += LoginControl_CharacterReceived;
            this.PivotControl.SelectionChanged += PivotControl_SelectionChanged;

            RfidTimer.Interval = TimeSpan.FromMilliseconds(200);
            RfidTimer.Tick += RfidTimer_Tick;

            AlertTimer.Interval = TimeSpan.FromSeconds(2);
            AlertTimer.Tick += AlertTimer_Tick;

            SoonTimer.Interval = TimeSpan.FromSeconds(5);
            SoonTimer.Tick += SoonTimer_Tick;

            Message.Text = "";

            int Index = -1;
            if (LocalSettingsHelper.Get("LoginIndex", ref Index))
            {
                PivotControl.SelectedIndex = Index;
            }

            handler1 += OnResult_Event1;
            handler2 += OnResult_Event2;
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
                soon = false;
                RfidTimer.Start();
            }
            else if (pi.Name.ToLower() == "pin")
            {
                Message.Text = "";
                RfidTimer.Stop();
            }


            LocalSettingsHelper.Set("LoginIndex", PivotControl.SelectedIndex);
        }

        private void LoginControl_CharacterReceived(UIElement sender, CharacterReceivedRoutedEventArgs args)
        {
            if (args.Character == 13)
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
            var event1 = new Data1();

            isEnabled = false;
            UserInteraction();

            if (PinText.Password != "")
            {
                ConnectionProfile InternetConnectionProfile = NetworkInformation.GetInternetConnectionProfile();

                if (InternetConnectionProfile != null)
                {

                    Repository.Repository.LoggedInUser = Repository.Repository.Communicator.loginUserByCode(PinText.Password);

                    if (Repository.Repository.LoggedInUser != null)
                    {
                        event1.isUser = true;

                        if (this.LoginAccepted != null) this.LoginAccepted(this, EventArgs.Empty);

                        if (Repository.Repository.LoggedInUser.LoginId == 200 || Repository.Repository.LoggedInUser.LoginId == 201)
                        {
                            Alert($"Üdvözöllek {Repository.Repository.LoggedInUser.Name}!", Colors.Green);
                            VoiceHelper.voiceAction_1(gpioController);
                        }
                        else if (Repository.Repository.LoggedInUser.LoginId == 300 || Repository.Repository.LoggedInUser.LoginId == 301)
                        {
                            Alert($"Viszont látásra {Repository.Repository.LoggedInUser.Name}!", Colors.Green);
                            VoiceHelper.voiceAction_1(gpioController);
                        }
                        else if (Repository.Repository.LoggedInUser.IsActive == 0)
                        {
                            Alert($"Ön jelenleg inaktív állapotban van!", Colors.Yellow);
                            VoiceHelper.voiceAction_2(gpioController);
                        }
                        else if (Repository.Repository.LoggedInUser.IsActive == -1)
                        {
                            Alert($"Nincs ilyen beregisztrált kód!", Colors.Yellow);
                            VoiceHelper.voiceAction_2(gpioController);
                        }
                        
                        PinText.Password = "";
                        PinText.SelectAll();
                        PinText.Focus(Windows.UI.Xaml.FocusState.Programmatic);
                    }
                    else
                    {
                        Alert("Az adatbázis nem elérhető!", Colors.Red);
                        VoiceHelper.voiceAction_3(gpioController);
                        PinText.Password = "";
                        PinText.SelectAll();
                        PinText.Focus(Windows.UI.Xaml.FocusState.Programmatic);
                    }
                }
                else
                {
                    Alert("Nincs internet elérés !!!", Colors.Red);                  

                    PinText.Password = "";
                    PinText.SelectAll();
                    PinText.Focus(Windows.UI.Xaml.FocusState.Programmatic);
                }

            }
            isEnabled = true;
        }

        private static int i=1;
        private void RfidTimer_Tick(object sender, object e)
        {

            ConnectionProfile InternetConnectionProfile = NetworkInformation.GetInternetConnectionProfile();

            if (InternetConnectionProfile != null)
            {
                if (soon == true) return;

                try
                {
                    lock (_lock2) //  amíg a szál fut, nem hozunk rá újabb szálakat
                    {
                        Alert("Kérem a kártyáját!", Colors.Yellow, false);

                        Task.Run(() => EvaulationOfResults());
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message);
                }
            }
            else
            {
                Alert("Nincs internet elérés!", Colors.Red);
            }
        }

        private void EvaulationOfResults()
        {
            lock (_lock) // amíg van _lock, (_lock != null) addig a blokkba nem lép be
            {
                // ez ha van nfc, ha nincs, beállítjuk. ha van nfc, a timer törli, ha nincs nfc, mi töröljük
                soon = true;
                var event1 = new Data1();
                var event2 = new Data2();

                // ha nincs nfc id (nincs kártya tartva) akkor 9 sec várakozás van - blokkolja a kódot
                // ez az ui-t nem blokkolja, mert külön szálon van
                nfcId = nfcReader.GetNfcId();

                if (!string.IsNullOrEmpty(nfcId))
                {
                    event1.isNfcId = true;

                    event2.message = "Beléptetés...";
                    event2.color = Colors.Yellow;
                    handler2?.Invoke(this, event2);

                    Repository.Repository.LoggedInUser = Repository.Repository.Communicator.loginUserByNFC_Id(nfcId);

                    if (Repository.Repository.LoggedInUser != null)
                    {
                        event1.isUser = true;
                        if (this.LoginAccepted != null) this.LoginAccepted(this, EventArgs.Empty);

                        if (Repository.Repository.LoggedInUser.LoginId == 200 || Repository.Repository.LoggedInUser.LoginId == 201)
                        {
                            event1.message = $"Üdvözöllek {Repository.Repository.LoggedInUser.Name}!";
                            event1.color = Colors.Green;

                            VoiceHelper.voiceAction_1(gpioController);
                        }
                        else if (Repository.Repository.LoggedInUser.LoginId == 300 || Repository.Repository.LoggedInUser.LoginId == 301)
                        {
                            event1.message = $"Viszont látásra {Repository.Repository.LoggedInUser.Name}!";
                            event1.color = Colors.Green;

                            VoiceHelper.voiceAction_1(gpioController);
                        }
                        else if (Repository.Repository.LoggedInUser.IsActive == 0)
                        {
                            event1.message = $"Ön jelenleg inaktív állapotban van!";
                            event1.color = Colors.Yellow;

                            VoiceHelper.voiceAction_2(gpioController);
                        }
                        else if (Repository.Repository.LoggedInUser.IsActive == -1)
                        {
                            event1.message = $"Nincs ilyen beregisztrált NFC kód!";
                            event1.color = Colors.Yellow;

                            VoiceHelper.voiceAction_2(gpioController);
                        }
                    }
                    else
                    {
                        event1.color = Colors.Red;
                        event1.message = $"Az adatbázis nem elérhető!";
                        VoiceHelper.voiceAction_3(gpioController);
                    }

                    // csak akkor sül el, ha van nfc
                    handler1?.Invoke(this, event1);
                }
                else
                {
                    soon = false;
                }
            }
        }

        public void OnResult_Event1(object sender, Data1 e)
        {
            if (!e.isUser)
            {
               // Debug.WriteLine("Nincs Felhasználó!");
                soon = false;
            }
            else 
            {
              _ = Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () => 
              {
                  //Debug.WriteLine("SoonTimer.Start");
                  SoonTimer.Start(); 
              });             
            }

            if (!string.IsNullOrEmpty(e.message))
            {
                Alert(e.message, e.color, true, "Kérem várjon!", Colors.Yellow);               
            }
        }

        public void OnResult_Event2(object sender, Data2 e)
        {
            Alert(e.message, e.color, false);
        }

        private void SoonTimer_Tick(object sender, object e)
        {
            //Debug.WriteLine("SoonTimer_Tick");
            SoonTimer.Stop();
            soon = false;
        }

        private void Alert(string msg, Windows.UI.Color color, bool autoHide = true, string msg2 = null, Windows.UI.Color? color2 = null)
        {
            _ = Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
            {
                Message.Text = msg;
                Message.Foreground = new SolidColorBrush(color);
                if (autoHide)
                {
                    secondMessage = msg2;
                    secondColor = (color2.HasValue) ? color2.Value : Colors.Yellow;
                    AlertTimer.Start();
                }
            });
        }

        private void AlertTimer_Tick(object sender, object e)
        {
            AlertTimer.Stop();
            if (!string.IsNullOrEmpty(secondMessage))
            {
                Message.Text = secondMessage;
                Message.Foreground = new SolidColorBrush(secondColor);
            }
            else
            {
                AlertClr();
            }

        }

        private void AlertClr()
        {
            _ = Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
            {
                Message.Text = "";
            });
        }
    }
}