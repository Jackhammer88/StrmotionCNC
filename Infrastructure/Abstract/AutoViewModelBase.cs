using Prism.Regions;
using System.Windows;

namespace Infrastructure.Abstract
{
    public abstract class AutoViewModelBase : ViewModelBase, INavigationAware, IRegionMemberLifetime
    {
        private bool _isBusy;
        public bool IsBusy
        {
            get => _isBusy;
            set => SetProperty(ref _isBusy, value);
        }
        public virtual bool KeepAlive => false;
        private string _fileName;
        public string FileName
        {
            get => _fileName;
            set => SetProperty(ref _fileName, value);
        }
        private Visibility _fileNameVisibility = Visibility.Collapsed;
        public Visibility FileNameVisibility
        {
            get => _fileNameVisibility;
            set => SetProperty(ref _fileNameVisibility, value);
        }

        public virtual void OnNavigatedFrom(NavigationContext navigationContext)
        {
        }
        public virtual void OnNavigatedTo(NavigationContext navigationContext)
        {

        }
        public virtual bool IsNavigationTarget(NavigationContext navigationContext)
        {
            return true;
        }
    }
}
