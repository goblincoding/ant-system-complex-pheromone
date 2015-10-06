using System;

namespace AntSimComplexUI.Utilities
{
    public class DirPathEventArgs : EventArgs
    {
        public string DirPath { get; private set; }

        public DirPathEventArgs(string dirPath)
        {
            DirPath = dirPath;
        }
    }
}