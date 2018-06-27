using System;

namespace Carbon.AutoUpdate.Common
{
	/// <summary>
	/// Defines an EventArgs class that contains AutoUpdateOptions as the context.
	/// </summary>
	public sealed class AutoUpdateOptionsEventArgs : EventArgs 
	{
		private readonly AutoUpdateOptions _options;
		private readonly AutoUpdateOptionNames _optionThatChanged;

		/// <summary>
		/// Initializes a new instance of the AutoUpdateOptionsEventArgs class.
		/// </summary>
		/// <param name="options">The options that contain the changes.</param>
		/// <param name="optionThatChanged">The name of the option that changed.</param>
		public AutoUpdateOptionsEventArgs(AutoUpdateOptions options, AutoUpdateOptionNames optionThatChanged)
		{
			_options = options;
			_optionThatChanged = optionThatChanged;
		}

		/// <summary>
		/// Returns the options that contain the changes.
		/// </summary>
		public AutoUpdateOptions Options
		{
			get
			{
				return _options;
			}
		}

		/// <summary>
		/// Returns the name of the option that changed.
		/// </summary>
		public AutoUpdateOptionNames OptionThatChanged
		{
			get
			{
				return _optionThatChanged;
			}
		}
	}
}
