using Infrastructure.AppEventArgs;
using Infrastructure.Resources.Strings;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace CNCDialogService.Models
{
    public class Location
    {
        public Location()
        {
            Filter = FileExtension.Default;
            string defaultPath = @"C:\";
            var lastPath = Registry.GetValue(@"HKEY_CURRENT_USER\Software\Strmotion", "LastDirectoryPath", null);
            if (lastPath is string && new DirectoryInfo((string)lastPath).Exists)
                OpenDirectory((string)lastPath);
            else
                OpenDirectory(defaultPath);
        }
        public Location(string path, FileExtension filter = null, bool showHidden = false)
        {
            Filter = filter ?? FileExtension.Default;
            OpenDirectory(path, showHidden);
        }
        public void OpenDirectory(string path, bool showHidden = false)
        {
            if (!Directory.Exists(path))
            {
                Error(this, new LocationErrorEventArgs(CNCDialogServiceStrings.DirectoryNotFound));
                return;
            }
            OpenDirectory(new DirectoryInfo(path), showHidden);
        }
        public void OpenDirectory(DirectoryInfo directoryInfo, bool showHidden = false)
        {
            if (directoryInfo is null)
                return;
            SubDirectories.Clear();
            Files.Clear();
            CurrentDirectory = directoryInfo;
            foreach (var file in directoryInfo.GetFiles().Where(f => Filter.CompareExtensions(f.Extension)))
            {
                Files.Add(file);
            }
            IEnumerable<DirectoryInfo> directories;
            if (showHidden)
                directories = directoryInfo.GetDirectories();
            else
                directories = directoryInfo.GetDirectories().Where(d => !d.Attributes.HasFlag(FileAttributes.Hidden));
            foreach (var directory in directories)
            {
                SubDirectories.Add(directory);
            }
            DirectoryChanged(this, new LocationEventArgs(CurrentDirectory));
        }
        public void UpToDir()
        {
            if (CurrentDirectory.Parent != null)
                OpenDirectory(CurrentDirectory.Parent);
        }

        public List<DirectoryInfo> SubDirectories { get; } = new List<DirectoryInfo>();
        public List<FileInfo> Files { get; } = new List<FileInfo>();
        public DirectoryInfo CurrentDirectory { get; private set; }
        private FileExtension _filter;
        public FileExtension Filter
        {
            get => _filter;
            set
            {
                _filter = value;
                OpenDirectory(CurrentDirectory);
            }
        }
        public event EventHandler<LocationEventArgs> DirectoryChanged = delegate { };
        public event EventHandler<LocationErrorEventArgs> Error = delegate { };
    }
}
