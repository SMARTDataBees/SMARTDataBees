using System;
using System.Windows.Forms;

using Carbon.Common;

namespace Carbon.UI
{	
	/// <summary>
	/// Provides a means of subclassing a window and raising events when menu messages are received as events
	/// </summary>
	public sealed class MenuLoopListener : NativeWindow
	{			
		private const int WM_ENTERMENULOOP = 0x0211;
		private const int WM_EXITMENULOOP = 0x0212;
		
		/// <summary>
		/// Occurs when the window enters a menu loop
		/// </summary>
		public event EventHandler<WindowMessageEventArgs> EnterMenuLoop;

		/// <summary>
		/// Occurs when the window exits a menu loop
		/// </summary>
		public event EventHandler<WindowMessageEventArgs> ExitMenuLoop;

		/// <summary>
		/// Initializes a new instance of the MenuLoopListener class
		/// </summary>
		/// <param name="hWnd">A handle to the window to watch for menu messages</param>
		public MenuLoopListener(IntPtr hWnd)
		{			
			base.AssignHandle(hWnd);	
		}
	
		#region My Overrides

		protected override void WndProc(ref Message m)
		{
			switch(m.Msg)
			{
			case WM_ENTERMENULOOP:
				EventManager.Raise<WindowMessageEventArgs>(this.EnterMenuLoop, this, new WindowMessageEventArgs(ref m));
				break;
			case WM_EXITMENULOOP:
				EventManager.Raise<WindowMessageEventArgs>(this.ExitMenuLoop, this, new WindowMessageEventArgs(ref m));
				break;
			};

			base.WndProc (ref m);
		}

		#endregion
	}
}
