using System;
using System.ComponentModel;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows;

namespace Akaha_Gesture {
    /// <summary>
    /// MainWindow.xaml の相互作用ロジック
    /// </summary>

    public partial class MainWindow : Window {
        public MainWindow()
        {
            InitializeComponent();
        }

        protected override void OnClosing(CancelEventArgs e) {
            base.OnClosing(e);
            var placement = this.GetPlacement();
            byte[] buffer = new byte[Marshal.SizeOf(placement)];
            using (BinaryWriter writer = new BinaryWriter(new MemoryStream(buffer))) {
                writer.Write(placement.length);
                writer.Write(placement.flags);
                writer.Write(placement.showCmd);
                writer.Write(placement.minPosition.X);
                writer.Write(placement.minPosition.Y);
                writer.Write(placement.maxPosition.Y);
                writer.Write(placement.maxPosition.Y);
                writer.Write(placement.normalPosition.Left);
                writer.Write(placement.normalPosition.Top);
                writer.Write(placement.normalPosition.Right);
                writer.Write(placement.normalPosition.Bottom);
            }
            var brandKey = Microsoft.Win32.Registry.CurrentUser.CreateSubKey(@"SOFTWARE\AkahaSoftware");
            var key = brandKey.CreateSubKey("AkahaGesture");
            key.SetValue("WindowPlacement", buffer, Microsoft.Win32.RegistryValueKind.Binary);
            key.Close();
            brandKey.Close();
        }

        protected override void OnSourceInitialized(EventArgs e) {
            base.OnSourceInitialized(e);
            var key = Microsoft.Win32.Registry.CurrentUser.OpenSubKey(@"SOFTWARE\AkahaSoftware\AkahaGesture");
            if (key != null) {
                var currentPlacement = this.GetPlacement();
                var placementBytes = (byte[])key.GetValue("WindowPlacement");
                WINDOWPLACEMENT placement;
                using (BinaryReader reader = new BinaryReader(new MemoryStream(placementBytes))) {
                    try {
                        placement.length = reader.ReadInt32();
                        placement.flags = reader.ReadInt32();
                        placement.showCmd = reader.ReadInt32();
                        placement.minPosition.X = reader.ReadInt32();
                        placement.minPosition.Y = reader.ReadInt32();
                        placement.maxPosition.X = reader.ReadInt32();
                        placement.maxPosition.Y = reader.ReadInt32();
                        placement.normalPosition.Left = reader.ReadInt32();
                        placement.normalPosition.Top = reader.ReadInt32();
                        placement.normalPosition.Right = reader.ReadInt32();
                        placement.normalPosition.Bottom = reader.ReadInt32();
                    } catch(IOException) {
                        // silently ignore decoding errors, and let the window open normally
                        return;
                    }
                }
                if (currentPlacement.showCmd != 1 /* SW_SHOWNORMAL */) {
                    placement.showCmd = currentPlacement.showCmd;
                }
                this.SetPlacement(placement);
            }
        }

    }
}
