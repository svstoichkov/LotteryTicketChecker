namespace LotteryTicketChecker.Client
{
    using System.Text;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Input;
    using System.Windows.Interactivity;

    using MaterialDesignThemes.Wpf;

    public class BarcodeScannerHandler : Behavior<Window>
    {
        private readonly StringBuilder barcodeReader;
        private readonly KeyConverter keyConverter;
        private bool isScanning;

        public BarcodeScannerHandler()
        {
            this.barcodeReader = new StringBuilder();
            this.keyConverter = new KeyConverter();
        }

        protected override void OnAttached()
        {
            base.OnAttached();
            this.AssociatedObject.PreviewKeyDown += this.OnPreviewKeyDown;
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();
            this.AssociatedObject.PreviewKeyDown -= this.OnPreviewKeyDown;
        }

        private void OnPreviewKeyDown(object sender, KeyEventArgs e)
        {
            var startKey = Key.LeftCtrl;
            var endKey = Key.LeftCtrl;
#if DEBUG
            startKey = Key.F11;
            endKey = Key.F12;
#endif

            if (e.Key == startKey && !this.isScanning)
            {
                    this.isScanning = true;
                    e.Handled = true;
            }
            else if (this.isScanning && e.Key == endKey)
            {
                if (this.barcodeReader.Length != 0)
                {
                    ((MainWindow)this.AssociatedObject).HandleBarcode(this.barcodeReader.ToString());
                    this.barcodeReader.Clear();
                }

                this.isScanning = false;
                e.Handled = true;
            }
            else if (this.isScanning)
            {
                e.Handled = true;
                var xChar = this.keyConverter.ConvertToString(e.Key);
                this.barcodeReader.Append(xChar);
            }
        }
    }
}