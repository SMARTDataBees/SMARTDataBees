using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace Carbon.Common
{
	public static class WindowUtilities
	{
		private delegate IntPtr GetWindowHandleDelegate(Form window);

		[DllImport("User32")]
		public static extern int LockWindowUpdate(IntPtr windowHandle);

		/// <summary>
		/// Safely retrieves the window's handle. The events the InstanceManager are on a separate thread
		/// forcing us to marshall the calls back to the window's thread.
		/// </summary>
		/// <param name="window">The window to retrieve the handle for.</param>
		/// <returns></returns>
		public static IntPtr SafeGetWindowHandle(Form window)
		{
			if (window == null)
			{
				return IntPtr.Zero;
			}

			if (window.InvokeRequired)
			{
				return (IntPtr)window.Invoke(new GetWindowHandleDelegate(SafeGetWindowHandle), window);
			}

			return window.Handle;
		}
	}
}
