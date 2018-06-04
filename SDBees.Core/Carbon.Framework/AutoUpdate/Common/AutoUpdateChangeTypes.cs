using System;
using System.Collections.Generic;
using System.Text;

namespace Carbon.AutoUpdate.Common
{
	/// <summary>
	/// Defines the types of events or changes that occurred during an AutoUpdate cycle.
	/// </summary>
	public enum AutoUpdateChangeTypes
	{
		/// <summary>
		/// Defines an event that resulted in a correction or adjustment being made to existing code.
		/// </summary>
		Correction,

		/// <summary>
		/// Defines an event that resulted in an addition to the code or product.
		/// </summary>
		Addition,

		/// <summary>
		/// Defines an event in which existing features, source code, or bugs were reviewed.
		/// </summary>
		Review,

		/// <summary>
		/// Defines an event in which source code was completely rewritten for a feature.
		/// </summary>
		Rewrite
	}
}
