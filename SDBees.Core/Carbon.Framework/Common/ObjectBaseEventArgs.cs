using System.Diagnostics;

namespace Carbon.Common
{
	[DebuggerStepThrough]
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
