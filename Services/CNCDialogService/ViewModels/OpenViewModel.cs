using CNCDialogService.Models;
using Infrastructure.AppEventArgs;
using Infrastructure.Resources.Strings;
using Microsoft.Win32;
using Prism.Commands;
using Prism.Services.Dialogs;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows;

namespace CNCDialogService.ViewModels
{
    public class OpenViewModel : DialogBase
    {
        readonly Location _location;
        private ObservableCollection<FileSystemInfo> _objects = new ObservableCollection<FileSystemInfo>();
        private FileExtension _selectedExtension;

        public OpenViewModel(Location location)
        {
            Title = CNCDialogServiceStrings.OpeningTitle;

            _location = location ?? throw new ArgumentNullException(nameof(location));
            _location.DirectoryChanged += OnDirectoryChanged;
            _location.Error += OnLocationError;
            SetCommands();
            AddDefaultExtensions();
        }

        public override void OnDialogOpened(IDialogParameters parameters)
        {
            base.OnDialogOpened(parameters);
            if (parameters != null && parameters.ContainsKey("extensions"))
            {
                Extensions.Clear();
                var extensions = parameters.GetValue<IEnumerable<Tuple<string, string>>>("extensions").Select(t => new FileExtension { Description = t.Item1, Extension = t.Item2 });
                if (extensions.Any())
                    Extensions.AddRange(extensions);
                else
                    AddDefaultExtensions();
            }
            else
                AddDefaultExtensions();
            SelectedExtension = Extensions.First();
        }

        private void OnLocationError(object _, LocationErrorEventArgs args)
        {
            MessageBox.Show(args.Message, CNCDialogServiceStrings.Error, MessageBoxButton.OK, MessageBoxImage.Error);
        }
        private void AddDefaultExtensions()
        {
            if (Extensions == null)
                Extensions = new List<FileExtension>();
            else
                Extensions.Clear();
            Extensions.AddRange(new List<FileExtension> { new FileExtension { Description = CNCDialogServiceStrings.CncFiles, Extension = ".cnc"},
             new FileExtension { Description = CNCDialogServiceStrings.NcFiles, Extension = ".nc" },
             new FileExtension { Description = CNCDialogServiceStrings.TxtFiles, Extension = ".txt" },
             new FileExtension { Description = CNCDialogServiceStrings.AllFiles, Extension = string.Empty } });
        }
        private void OnDirectoryChanged(object sender, LocationEventArgs _)
        {
            OpenDirectory();
            RaisePropertyChanged(nameof(CurrentDirectory));
        }
        private void SetCommands()
        {
            SelectCommand = new DelegateCommand<object>(ExecuteSelectCommand);
            UpCommand = new DelegateCommand(ExecuteUpCommand, CanExecuteUpCommand);
            OpenDriveCommand = new DelegateCommand<string>(ExecuteOpenDriveCommand);
            CloseDialogCommand = new DelegateCommand(ExecuteCloseDialogCommand);
        }
        private void OpenDirectory()
        {
            _objects.Clear();
            foreach (var directory in _location.SubDirectories)
            {
                Objects.Add(directory);
            }
            foreach (var file in _location.Files)
            {
                Objects.Add(file);
            }
        }
        public void ExecuteUpCommand()
        {
            _location.UpToDir();
            OpenDirectory();
            UpCommand.RaiseCanExecuteChanged();
        }
        private bool CanExecuteUpCommand()
        {
            return _location.CurrentDirectory.Parent != null;
        }
        private void ExecuteOpenDriveCommand(string drive)
        {
            _location.OpenDirectory(drive);
        }
        private void ExecuteCloseDialogCommand()
        {
            var dialogResult = new DialogResult(ButtonResult.Cancel);
            RaiseRequestClose(dialogResult);
        }
        private async void ExecuteSelectCommand(object item)
        {
            await Application.Current.Dispatcher.BeginInvoke(new Action(() =>
            {
                //Thread.Sleep(150);
                if (item is DirectoryInfo directory)
                {
                    _location.OpenDirectory(directory);
                    OpenDirectory();
                    UpCommand.RaiseCanExecuteChanged();
                }
                if (item is FileInfo file)
                {
                    Registry.SetValue(@"HKEY_CURRENT_USER\Software\Strmotion", "LastDirectoryPath", file.Directory);
                    var dialogResult = new DialogResult(ButtonResult.OK, new DialogParameters($"path={file.FullName}"));
                    RaiseRequestClose(dialogResult);
                }
            }));
        }

        public string CurrentDirectory
        {
            get => _location.CurrentDirectory.FullName;
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "CA2227:Свойства коллекций должны быть доступны только для чтения", Justification = "<Ожидание>")]
        public ObservableCollection<FileSystemInfo> Objects
        {
            get { return _objects; }
            set { SetProperty(ref _objects, value); }
        }
        public List<FileExtension> Extensions { get; private set; }
        public FileExtension SelectedExtension
        {
            get => _selectedExtension;
            set
            {
                if (value != null)
                {
                    SetProperty(ref _selectedExtension, value);
                    _location.Filter = _selectedExtension;
                }
            }
        }
        public static IEnumerable<string> Drives => Directory.GetLogicalDrives();

        public DelegateCommand UpCommand { get; private set; }
        public DelegateCommand<object> SelectCommand { get; private set; }
        public DelegateCommand<string> OpenDriveCommand { get; private set; }
        public DelegateCommand CloseDialogCommand { get; private set; }
    }
}
