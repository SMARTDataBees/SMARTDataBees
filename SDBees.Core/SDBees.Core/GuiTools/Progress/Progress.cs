
using System;

namespace SDBees.GuiTools
{
    public class Progress
    {
        public static iProgress GetProgress(IntPtr windowHandle)
        {
            return new AdnRme.ProgressForm(windowHandle);
        }
    }
}
