using System;

namespace SDBees.Core.Admin
{
    public class ViewSelectionArgs : EventArgs
    {
        public ViewSelectionArgs(string viewname, Guid identification)
        {
            Name = viewname;
            Identification = identification;
        }

        public string Name { get; }

        public Guid Identification { get; }
    }
}