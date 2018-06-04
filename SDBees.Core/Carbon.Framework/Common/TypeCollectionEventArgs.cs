using System;
using System.Collections.Generic;
using System.Text;

namespace Carbon.Common
{
	/// <summary>
	/// Defines an EventArgs 
	/// </summary>
	public sealed class TypeCollectionEventArgs : ObjectEventArgs 
	{
		public TypeCollectionEventArgs(Type context, ObjectActions action) :
			base(context, action)
		{

		}

		public new Type Context
		{
			get
			{
				return (Type)base.Context;
			}
		}
	}
}
