using System;

namespace AntSimComplexUI.Utilities
{
    public class StringEventArgs : EventArgs
    {
        public string DirPath { get; private set; }

        public StringEventArgs(string dirPath)
        {
            DirPath = dirPath;
        }
    }
}
