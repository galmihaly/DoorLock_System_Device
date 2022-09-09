using FirstUwp.Classes;
using FirstUwp.Helpers;
using System;
using System.Device.Gpio;
using System.Diagnostics;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace FirstUwp.UserControls
{
    public sealed partial class LoginControl : UserControl
    {
        /// <summary>
        /// Események a login kezeléséhez
        /// </summary>
        public event EventHandler LoginAccepted;
        public event EventHandler LoginCancelled;
        public event EventHandler LoginUserInteraction;

        string nfcId;
        int ledPinGreen = 18;
        int ledPinRed = 10;

        private GpioController gpioController = null;
        private NfcReader nfcReader = null;


        /// <summary>
        /// Azt állítja be, hogy a 'Mégsem" gomb alklamzható-e
        /// </summary>
        public bool CanCancel { get; set; } = false;

        private DispatcherTimer _timer = new DispatcherTimer();
        private DispatcherTimer RfidTimer = new DispatcherTimer();

        public LoginControl()
        {
            this.Loaded += LoginControl_Loaded;
            _timer.Interval = TimeSpan.FromSeconds(1);
            _timer.Tick += _timer_Tick;

            RfidTimer.Interval = TimeSpan.FromSeconds(1);
            RfidTimer.Tick += RfidTimer_Tick;

            this.InitializeComponent();

            //PinPanel.Width = 360;
            //PinPanel.Height = 720;
            ErrorMessage.Text = "";

            Translate();
            if (!CanCancel)
            {
                PinCancel.Visibility = Visibility.Collapsed;
                //bnAccountCancel.Visibility = Visibility.Collapsed;
                //BarcodeCancel.Visibility = Visibility.Collapsed;
            }

            this.CharacterReceived += LoginControl_CharacterReceived;
            this.PivotControl.SelectionChanged += PivotControl_SelectionChanged;

            int Index = -1;
            if (LocalSettingsHelper.Get("LoginIndex", ref Index))
            {
                PivotControl.SelectedIndex = Index;
            }
        }

        private void LoginControl_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                nfcReader = new NfcReader();
                gpioController = new GpioController();

                gpioController.OpenPin(ledPinGreen, PinMode.Output);
                gpioController.Write(ledPinGreen, PinValue.Low);

                gpioController.OpenPin(ledPinRed, PinMode.Output);
                gpioController.Write(ledPinRed, PinValue.Low);
            }
            catch (Exception en)
            {
                Debug.WriteLine(en.Message);
            }
        }

        private void RfidTimer_Tick(object sender, object e)
        {
            RfidTimer.Stop();
            try
            {
                nfcId = nfcReader.GetNfcId();

                if (!string.IsNullOrEmpty(nfcId))
                {

                    gpioController.Write(ledPinGreen, PinValue.High);
                    gpioController.Write(ledPinRed, PinValue.Low);

                    Repository.Repository.LoggedInUser = Repository.Repository.Communicator.loginUserByNFC_Id(nfcId);
                    if (Repository.Repository.LoggedInUser != null)
                    {
                        var eventHandler = this.LoginAccepted;
                        if (eventHandler != null)
                        {
                            eventHandler(this, EventArgs.Empty);
                        }
                    }
                    else
                    {
                        //ErrorMessage.Text = Repository.Translate("WC_MeasuringStation.Login.Rfid.Error");
                        ErrorMessage.Text = "A beolvasott RFID kártya nem tartozik egyetlen aktív felhasználóhoz sem!";
                    }


                    if (Repository.Repository.LoggedInUser != null)
                    {
                        
                        if (Repository.Repository.LoggedInUser.LoginId == 200)
                        {
                            Debug.WriteLine($"a(z) {Repository.Repository.LoggedInUser.Id} azonosítójú felhasználó belépett!");
                        }
                        else if (Repository.Repository.LoggedInUser.LoginId == 300)
                        {
                            Debug.WriteLine($"a(z) {Repository.Repository.LoggedInUser.LoginId} azonosítójú felhasználó kilépett!");
                        }
                        else
                        {
                            Debug.WriteLine($"a beolvasott kártyával nem lehet belépni!");
                        }
                    }
                    else
                    {
                        Debug.WriteLine("ön még nem használta a rendszerünket!");
                    }

                    gpioController.Write(ledPinGreen, PinValue.Low);
                }
                else
                {

                    Debug.WriteLine("Nincs beolvasott NFC kód!");
                    gpioController.Write(ledPinRed, PinValue.High);

                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
            finally
            {
                RfidTimer.Start();
            }
        }

        private void _timer_Tick(object sender, object e)
        {
            _timer.Stop();

            switch (PivotControl.SelectedIndex)
            {
                //case 0:
                //    LoginAccountTextBox.SelectAll();
                //    LoginAccountTextBox.Focus(Windows.UI.Xaml.FocusState.Programmatic);
                //    break;

                case 0:
                    PinText.SelectAll();
                    PinText.Focus(Windows.UI.Xaml.FocusState.Programmatic);
                    break;

                case 1:
                    RfidTimer.Start();
                    break;

                    //case 3:
                    //    BarcodeTextBox.SelectAll();
                    //    BarcodeTextBox.Focus(Windows.UI.Xaml.FocusState.Programmatic);
                    //    break;
            }

        }

        private void PivotControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            RfidTimer.Stop();
            _timer.Start();
            LocalSettingsHelper.Set("LoginIndex", PivotControl.SelectedIndex);
        }

        private void LoginControl_CharacterReceived(UIElement sender, CharacterReceivedRoutedEventArgs args)
        {
            if (args.Character == 13) // Enter
            {
                switch (PivotControl.SelectedIndex)
                {
                    //case 0:
                    //    AccountLogin_Click(sender, null);
                    //    break;

                    case 0:
                        PinOk_Click(sender, null);
                        break;
                    case 1:
                        break;

                        //case 3:
                        //    BarcodeOk_Click(sender, null);
                        //    break;
                }
            }

            if (args.Character == 27) //27=[ESC]
            {
                bnCancel_Click(sender, null);
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

        private void Translate()
        {
            //bnAccountLogin.Content = Repository.Translate("WC_MeasuringStation.Login");
            //bnAccountCancel.Content = Repository.Translate("WC_MeasuringStation.Cancel");
            //AccountSubtitle.Text = Repository.Translate("WC_MeasuringStation.Login.Account.Head");
            //AccountName.Text = Repository.Translate("WC_MeasuringStation.Username");
            //AccountPassword.Text = Repository.Translate("WC_MeasuringStation.Password");
            //RfidSubtitle.Text = Repository.Translate("WC_MeasuringStation.Login.Rfid.Head");
            //BarcodeSubtitle.Text = Repository.Translate("WC_MeasuringStation.Login.Barcode.Head");
        }

        //private void LanguageButton_Click(object sender, RoutedEventArgs e)
        //{
        //    Button button = sender as Button;
        //    int iLanguage = System.Convert.ToInt32(button.Tag);
        //    Repository.SetLanguage((Language)iLanguage);
        //    Translate();
        //}

        private void AccountLogin_Click(object sender, RoutedEventArgs e)
        {
            //UserInteraction();
            //bool bResult = Task.Run(async () => { return await Repository.LoginByAccount(LoginAccountTextBox.Text, LoginPasswordTextBox.Password); }).GetAwaiter().GetResult();
            //if (bResult)
            //{
            //    var eventHandler = this.LoginAccepted;
            //    if (eventHandler != null)
            //    {
            //        eventHandler(this, EventArgs.Empty);
            //    }
            //}
            //else
            //{
            //    ErrorMessage.Text = Repository.Translate("WC_MeasuringStation.Login.Account.Error");
            //    LoginPasswordTextBox.SelectAll();
            //    LoginPasswordTextBox.Focus(Windows.UI.Xaml.FocusState.Programmatic);
            //}
        }

        private void bnCancel_Click(object sender, RoutedEventArgs e)
        {
            UserInteraction();

            if (CanCancel)
            {
                var eventHandler = this.LoginCancelled;

                if (eventHandler != null)
                {
                    eventHandler(this, EventArgs.Empty);
                }
            }
        }

        #region Név és jelszó függvényei
        private void LoginAccountTextBox_KeyDown(object sender, KeyRoutedEventArgs e)
        {
            //UserInteraction();
            //if (e.Key == Windows.System.VirtualKey.Enter)
            //{
            //    AccountLogin_Click(sender, null);
            //}
        }

        private void LoginPasswordTextBox_KeyDown(object sender, KeyRoutedEventArgs e)
        {
            //UserInteraction();
            //if (e.Key == Windows.System.VirtualKey.Enter)
            //{
            //    bnCancel_Click(sender, null);
            //}
        }
        #endregion 

        #region Pin kód függvényei
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
            UserInteraction();

            //bool bResult = Task.Run(async () => { return await Repository.LoginByPin(PinText.Password); }).GetAwaiter().GetResult();
            Repository.Repository.LoggedInUser = Repository.Repository.Communicator.loginUserByCode(PinText.Password);
            if (Repository.Repository.LoggedInUser != null)
            {
                var eventHandler = this.LoginAccepted;
                if (eventHandler != null)
                {
                    eventHandler(this, EventArgs.Empty);
                }
            }
            else
            {
                //ErrorMessage.Text = Repository.Translate("WC_MeasuringStation.Login.Pincode.Error");
                ErrorMessage.Text = "A megadott pin kóddal a felhaznál nem léptethető be!";
                PinText.SelectAll();
                PinText.Focus(Windows.UI.Xaml.FocusState.Programmatic);
            }
        }
        #endregion

        #region Vonalkódkezelés
        private async void BarcodeOk_Click(object sender, RoutedEventArgs e)
        {
            //UserInteraction();

            //if (await Repository.LoginByBarcode(BarcodeTextBox.Text))
            //{
            //    var eventHandler = this.LoginAccepted;
            //    if (eventHandler != null)
            //    {
            //        eventHandler(this, EventArgs.Empty);
            //    }
            //}
            //else
            //{
            //    ErrorMessage.Text = Repository.Translate("WC_MeasuringStation.Login.Barcode.Error");
            //    BarcodeTextBox.SelectAll();
            //    BarcodeTextBox.Focus(Windows.UI.Xaml.FocusState.Programmatic);
            //}
        }
        #endregion

        #region RFID kártya kezelése
        private async Task LoginWithRfid(string rfid)
        {
            UserInteraction();

            Repository.Repository.LoggedInUser = Repository.Repository.Communicator.loginUserByNFC_Id(rfid);
            if (Repository.Repository.LoggedInUser != null)
            {
                var eventHandler = this.LoginAccepted;
                if (eventHandler != null)
                {
                    eventHandler(this, EventArgs.Empty);
                }
            }
            else
            {
                //ErrorMessage.Text = Repository.Translate("WC_MeasuringStation.Login.Rfid.Error");
                ErrorMessage.Text = "A beolvasott RFID kártya nem tartozik egyetlen aktív felhasználóhoz sem!";
            }
        }
        #endregion

        private async void ShutdownButton_Click(object sender, RoutedEventArgs e)
        {
            //ContentDialog dlg = new ContentDialog();
            //dlg.Title = Repository.Translate("WC_MeasuringStation.Shutdown");
            //dlg.PrimaryButtonText = Repository.Translate("WC_MeasuringStation.Page.Settings.Restart.Yes");
            //dlg.SecondaryButtonText = Repository.Translate("WC_MeasuringStation.Page.Settings.Restart.No");
            ////dlg.CloseButtonText = Repository.Translate("WC_MeasuringStation.Close");
            //dlg.Content = Repository.Translate("WC_MeasuringStation.Page.Settings.Restart.Text");

            ////todo 
            ////var a = dlg.ShowAsync().AsTask();
            ////a.RunSynchronously();
            ////ContentDialogResult result = a.Result;

            //ContentDialogResult result = await dlg.ShowAsync();
            //if (result == ContentDialogResult.Primary)
            //{
            //    NetworkHelper.ShutdownComputer();
            //}

        }
    }
}
