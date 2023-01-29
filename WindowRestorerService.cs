using System;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows;
using System.Windows.Interop;
using System.Xml.Serialization;

namespace Akaha_Gesture {
    // RECT structure required by WINDOWPLACEMENT structure
    [Serializable]
    [StructLayout(LayoutKind.Sequential)]
    public struct RECT {
        public int Left;
        public int Top;
        public int Right;
        public int Bottom;

        public RECT(int left, int top, int right, int bottom)
        {
            Left = left;
            Top = top;
            Right = right;
            Bottom = bottom;
        }
    }

    // POINT structure required by WINDOWPLACEMENT structure
    [Serializable]
    [StructLayout(LayoutKind.Sequential)]
    public struct POINT {
        public int X;
        public int Y;

        public POINT(int x, int y)
        {
            X = x;
            Y = y;
        }
    }

    // WINDOWPLACEMENT stores the position, size, and state of a window
    [Serializable]
    [StructLayout(LayoutKind.Sequential)]
    public struct WINDOWPLACEMENT {
        public int length;
        public int flags;
        public int showCmd;
        public POINT minPosition;
        public POINT maxPosition;
        public RECT normalPosition;
    }

    static class WindowRestorerService {
        private const int SW_SHOWNORMAL = 1;
        private const int SW_SHOWMINIMIZED = 2;

        private static Encoding encoding = new UTF8Encoding();
        private static XmlSerializer serializer = new XmlSerializer(typeof(WINDOWPLACEMENT));

        [DllImport("user32.dll")]
        private static extern bool SetWindowPlacement(IntPtr hWnd, [In] ref WINDOWPLACEMENT lpwndpl);

        [DllImport("user32.dll")]
        private static extern bool GetWindowPlacement(IntPtr hWnd, out WINDOWPLACEMENT lpwndpl);

        public static void SetPlacement(this Window window, WINDOWPLACEMENT placement)
        {
            WindowRestorerService.SetPlacement(new WindowInteropHelper(window).Handle, placement);
        }

        /// <summary>
        /// Set the window to the relevent position using the placement information. 
        /// </summary>
        /// <param name="windowHandle">The window handle of the target information.</param>
        /// <param name="placementXml">The placement XML.</param>
        public static void SetPlacement(IntPtr windowHandle, WINDOWPLACEMENT placement)
        {
            SetWindowPlacement(windowHandle, ref placement);
        }

        public static WINDOWPLACEMENT GetPlacement(this Window window)
        {
            return WindowRestorerService.GetPlacement(new WindowInteropHelper(window).Handle);
        }

        /// <summary>
        /// Retruns the serialize XML of the placement information for the 
        /// target window.
        /// </summary>
        /// <param name="windowHandle">The handle of the target window.</param>
        public static WINDOWPLACEMENT GetPlacement(IntPtr windowHandle)
        {
            WINDOWPLACEMENT placement = new WINDOWPLACEMENT();
            GetWindowPlacement(windowHandle, out placement);
            return placement;
        }
    }

}
