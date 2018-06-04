using System;
using System.Collections.Generic;
using System.Text;

namespace Carbon.Common
{
	[System.Diagnostics.DebuggerStepThrough()]
	public class ObjectBaseCancelEventArgs : ObjectCancelEventArgs
	{
		public ObjectBaseCancelEventArgs(ObjectBase context, ObjectActions action, bool cancel)
			: base(context, action, cancel)
		{

		}

		public new ObjectBase Context
		{
			get
			{
				return (ObjectBase)base.Context;
			}
		}
	}
}
