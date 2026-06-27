using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;
using Palisades.ViewModel;

namespace Palisades.View
{
    public partial class Palisade : Window
    {
        // Safe Additions: Core Windows API functions needed to force the desktop layer placement
        [DllImport("user32.dll", SetLastError = true)]
        private static extern int GetWindowLong(IntPtr hWnd, int nIndex);

        [DllImport("user32.dll")]
        private static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);

        private const int GWL_EXSTYLE = -20;
        private const int WS_EX_TOOLWINDOW = 0x00000080;   // Hides it cleanly from Alt+Tab
        private const int WS_EX_NOACTIVATE = 0x08000000;   // Prevents it from taking window focus

        private readonly PalisadeViewModel viewModel;
        
        public Palisade(PalisadeViewModel defaultModel)
        {
            InitializeComponent();
            DataContext = defaultModel;
            viewModel = defaultModel;
            Show();
        }

        // Safe Addition: This runs automatically to pin the container to your desktop background layer
        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);
            
            var helper = new WindowInteropHelper(this);
            IntPtr hwnd = helper.Handle;

            int exStyle = GetWindowLong(hwnd, GWL_EXSTYLE);
            SetWindowLong(hwnd, GWL_EXSTYLE, exStyle | WS_EX_NOACTIVATE | WS_EX_TOOLWINDOW);
        }

        private void Header_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonDown(e);
            DragMove();
        }
    }
}
