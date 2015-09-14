using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.Devices.PointOfService;
using Windows.System.Threading;
using Windows.UI.Xaml.Media.Animation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=391641

namespace BarcodeSample1
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page,IDisposable
    {
        private TransitionCollection transitions;
        BarcodeScanner scanner;
        ClaimedBarcodeScanner claimedScanner;

        public MainPage()
        {
            this.InitializeComponent();

            this.NavigationCacheMode = NavigationCacheMode.Required;
        }

        public void Dispose()
        {
            DisableScanner();
        }

        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.
        /// This parameter is typically used to configure the page.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            // TODO: Prepare page for display here.

            // TODO: If your application contains multiple pages, ensure that you are
            // handling the hardware Back button by registering for the
            // Windows.Phone.UI.Input.HardwareButtons.BackPressed event.
            // If you are using the NavigationHelper provided by some templates,
            // this event is handled for you.
            
        }
        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            DisableScanner();
            base.OnNavigatedFrom(e);
        }

        private async void button_Click(object sender, RoutedEventArgs e)
        {
            if (claimedScanner != null)
                await claimedScanner.StartSoftwareTriggerAsync();
        }

        private async void buttonEnable_Click(object sender, RoutedEventArgs e)
        {
            scanner = await BarcodeScanner.GetDefaultAsync();
            if (claimedScanner != null)
                claimedScanner.Dispose();
            claimedScanner = await scanner.ClaimScannerAsync();
            claimedScanner.DataReceived += ClamedScanner_DataReceived;
            await claimedScanner.EnableAsync();
            claimedScanner.IsDecodeDataEnabled = true;
            
            await claimedScanner.SetActiveSymbologiesAsync(new uint[] {
                BarcodeSymbologies.Aztec,
                BarcodeSymbologies.AusPost,
                BarcodeSymbologies.CanPost,
                BarcodeSymbologies.Codabar,
                BarcodeSymbologies.Codablock128,
                BarcodeSymbologies.Code11,
                BarcodeSymbologies.Code128,
                BarcodeSymbologies.Code39,
                BarcodeSymbologies.Code39Ex,
                BarcodeSymbologies.DataMatrix,
                BarcodeSymbologies.Ean13,
                BarcodeSymbologies.Ean8,
                BarcodeSymbologies.Gs1128,
                BarcodeSymbologies.Gs1DatabarType1 ,
                BarcodeSymbologies.Gs1DatabarType2,
                BarcodeSymbologies.Gs1DatabarType3,
                BarcodeSymbologies.Pdf417,
                BarcodeSymbologies.Qr,
                BarcodeSymbologies.OcrB,
                BarcodeSymbologies.UccEan128,
                BarcodeSymbologies.Upca,
                BarcodeSymbologies.Upce,
                BarcodeSymbologies.UsPostNet,
            });
            changeButtonEnable(true);
        }

        void changeButtonEnable(bool bEnableScan)
        {
            button_Enable.IsEnabled = !bEnableScan;
            button_Disable.IsEnabled = bEnableScan;
            button_DoScan.IsEnabled = bEnableScan;
        }

        async private void ClamedScanner_DataReceived(ClaimedBarcodeScanner sender, BarcodeScannerDataReceivedEventArgs args)
        {
            UInt32 symbology;
            string label;
            string data;
            symbology = args.Report.ScanDataType;

            using(var datareader = Windows.Storage.Streams.DataReader.FromBuffer(args.Report.ScanDataLabel))
            {
                label = datareader.ReadString(args.Report.ScanDataLabel.Length);
            }
            using (var datareader = Windows.Storage.Streams.DataReader.FromBuffer(args.Report.ScanData))
            {
                data = datareader.ReadString(args.Report.ScanData.Length);
            }
            
            await this.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
            {
                textBoxSymbology.Text = BarcodeSymbologies.GetName(symbology); //Send the data to the UI through the dispatcher. tbData is a textbox in the UI
                textBoxData.Text = label; //Send the data to the UI through the dispatcher. tbLabel is a textbox in the UI
            });
        }

        private void button_Disable_Click(object sender, RoutedEventArgs e)
        {
            DisableScanner();
        }

        private void DisableScanner()
        {
            if (claimedScanner != null)
            {
                claimedScanner.Dispose();
                claimedScanner = null;
            }
            changeButtonEnable(false);
        }
    }
}
