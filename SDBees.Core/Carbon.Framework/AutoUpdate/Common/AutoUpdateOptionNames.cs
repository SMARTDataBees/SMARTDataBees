using System;
using System.Collections.Generic;
using System.Text;

namespace Carbon.AutoUpdate.Common
{
	/// <summary>
	/// Defines the names of the AutoUpdate options.
	/// </summary>
	public enum AutoUpdateOptionNames
	{
		/// <summary>
		/// Automatically check for updates.
		/// </summary>
		AutomaticallyCheckForUpdates,

		/// <summary>
		/// Automatically download available updates.
		/// </summary>
		AutomaticallyDownloadUpdates,

		/// <summary>
		/// Automatically install available updates.
		/// </summary>
		AutomaticallyInstallUpdates,

		/// <summary>
		/// Automatically switch to new versions after they are installed.
		/// </summary>
		AutomaticallySwitchToNewVersion,

		/// <summary>
		/// Automatically update the alternate path with the update after it is downloaded.
		/// </summary>
		AutomaticallyUpdateAlternatePath,

		/// <summary>
		/// An alternate path that can be used to hold updates, 
		/// and optionally updated with updates that are downloaded.
		/// This path will be checked first for updates if it is filled in, potentially
		/// saving bandwidth by keeping the checks local.
		/// </summary>
		AlternatePath,

		/// <summary>
		/// The url of the web service that will be used to check for updates.
		/// </summary>
		WebServiceUrl
	}
}
