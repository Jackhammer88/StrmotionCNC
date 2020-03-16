using System;
using System.IO;

namespace Infrastructure.AppEventArgs
{
    public class LocationEventArgs : EventArgs
    {
        public LocationEventArgs(DirectoryInfo directory)
        {
            Directory = directory;
        }
        public DirectoryInfo Directory { get; }
    }
}