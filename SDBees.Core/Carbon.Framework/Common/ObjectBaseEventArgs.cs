using System;
using System.Collections.Generic;
using System.Text;

namespace Carbon.Common
{
	[System.Diagnostics.DebuggerStepThrough()]
	public class ObjectBaseEventArgs : ObjectEventArgs 
	{
		public ObjectBaseEventArgs(ObjectBase context, ObjectActions action)
			: base(context, action)
		{

		}

		public new ObjectBase Context
		{
			get
			{
				return base.Context as ObjectBase;
			}
		}
	}
}
