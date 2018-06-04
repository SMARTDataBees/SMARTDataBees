
using System;

namespace SDBees.GuiTools
{
    public interface iProgress : IDisposable
    {
        void Show();

        void Hide();

        void Set(string caption, int max, string staticText, string dynamicText);

        void SetMax(int max);

        void Increment(int value = 1);

        void IncrementTo(int value);
    }
}
