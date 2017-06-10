using System;
using System.Windows.Input;

namespace BookCollector.Framework
{
    public class BusyCursor : IDisposable
    {
        private Cursor previous_cursor;

        public BusyCursor()
        {
            previous_cursor = Mouse.OverrideCursor;

            Mouse.OverrideCursor = Cursors.Wait;
        }

        public void Dispose()
        {
            Mouse.OverrideCursor = previous_cursor;
        }
    }
}
