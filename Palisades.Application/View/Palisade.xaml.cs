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

        // Required API Hook: Safely staples our window handle into the native desktop backdrop shell
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
            Show();
        }

        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);
            
            var helper = new WindowInteropHelper(this);
            IntPtr hwnd = helper.Handle;

            // Step 1: Apply our invisible, non-minimizing toolwindow logic
            int exStyle = GetWindowLong(hwnd, GWL_EXSTYLE);
            SetWindowLong(hwnd, GWL_EXSTYLE, exStyle | WS_EX_NOACTIVATE | WS_EX_TOOLWINDOW);

            // Step 2: Bind the app directly to the physical Windows Desktop background manager window handle
            IntPtr desktopHandle = FindWindow("Progman", null);
            if (desktopHandle != IntPtr.Zero)
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
