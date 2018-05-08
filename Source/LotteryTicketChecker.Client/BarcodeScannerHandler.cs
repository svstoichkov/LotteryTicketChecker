namespace Client
{
    using System.Text;
    using System.Windows.Input;

    public class BarcodeScannerHandler
    {
        private readonly MainWindow window;
        private readonly StringBuilder barcodeReader;
        private readonly KeyConverter keyConverter;
        private bool isScanning;

        public BarcodeScannerHandler(MainWindow window)
        {
            this.window = window;
            this.barcodeReader = new StringBuilder();
            this.keyConverter = new KeyConverter();

            window.PreviewKeyDown += this.OnPreviewKeyDown;
        }

        private void OnPreviewKeyDown(object sender, KeyEventArgs e)
        {
            var startKey = Key.LeftCtrl;
            var endKey = Key.LeftCtrl;
//#if DEBUG
//            startKey = Key.F11;
//            endKey = Key.F12;
//#endif

            if (e.Key == startKey && !this.isScanning)
            {
                this.isScanning = true;
                e.Handled = true;
            }
            else if (this.isScanning && e.Key == endKey)
            {
                if (this.barcodeReader.Length != 0)
                {
                    this.window.HandleBarcode(this.barcodeReader.ToString().Trim('L'));
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