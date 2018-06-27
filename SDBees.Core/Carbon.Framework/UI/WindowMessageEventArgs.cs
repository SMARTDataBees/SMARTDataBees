using System;
using System.Windows.Forms;

namespace Carbon.UI
{
	/// <summary>
	/// Defines an EventArgs class that contains a Window Message as the context of the event.
	/// </summary>
	public sealed class WindowMessageEventArgs : EventArgs
	{
		private Message _message;

		/// <summary>
		/// Initializes a new instance of the WindowMessageEventArgs class.
		/// </summary>
		/// <param name="m">A reference to the window message being processed.</param>
		public WindowMessageEventArgs(ref Message m)
		{
			_message = m;
		}

		/// <summary>
		/// Returns a reference to the window message being processed.
		/// </summary>
		public Message Message
		{
			get
			{
				return _message;
			}
		}
	}
}
