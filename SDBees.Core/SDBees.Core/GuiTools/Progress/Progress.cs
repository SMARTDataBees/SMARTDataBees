
using System;
using AdnRme;

namespace SDBees.GuiTools
{
    public class Progress
    {
        public static iProgress GetProgress(IntPtr windowHandle)
        {
            return new ProgressForm(windowHandle);
        }
    }
}
