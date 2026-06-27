using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;
using Palisades.ViewModel;

namespace Palisades.View
{
    public partial class Palisade : Window
    {
        [DllImport("user32.dll", SetLastError = true)]
        private static extern int GetWindowLong(IntPtr hWnd, int nIndex);

        [DllImport("user32.dll")]
        private static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern IntPtr SetParent(IntPtr hWndChild, IntPtr hWndNewParent);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

        private const int GWL_EXSTYLE = -20;
        private const int WS_EX_TOOLWINDOW = 0x00000080;
        private const int WS_EX_NOACTIVATE = 0x08000000;

        private readonly PalisadeViewModel viewModel;

        public Palisade(PalisadeViewModel defaultModel)
        {
            InitializeComponent();
            DataContext = defaultModel;
            viewModel = defaultModel;
            
            // Safe Fix: Wait until the app window is 100% rendered on screen before anchoring
            this.Loaded += Palisade_Loaded;
        }

        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);
            
            var helper = new WindowInteropHelper(this);
            IntPtr hwnd = helper.Handle;

            // Apply style layout flags immediately on load
            int exStyle = GetWindowLong(hwnd, GWL_EXSTYLE);
            SetWindowLong(hwnd, GWL_EXSTYLE, exStyle | WS_EX_NOACTIVATE | WS_EX_TOOLWINDOW);
        }

        private void Palisade_Loaded(object sender, RoutedEventArgs e)
        {
            var helper = new WindowInteropHelper(this);
            IntPtr hwnd = helper.Handle;

            // Search for the desktop manager windows safely
            IntPtr desktopHandle = FindWindow("Progman", null);
            if (desktopHandle == IntPtr.Zero)
            {
                desktopHandle = FindWindow("WorkerW", null); // Windows 11 Fallback layer
            }

            // Only anchor if handles are explicitly verified, preventing silent application crashes
            if (hwnd != IntPtr.Zero && desktopHandle != IntPtr.Zero)
            {
                SetParent(hwnd, desktopHandle);
            }
        }

        private void Header_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonDown(e);
            DragMove();
        }
    }
}
