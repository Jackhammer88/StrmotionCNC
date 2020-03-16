using Prism.Mvvm;
using Prism.Services.Dialogs;
using System;

namespace CNCDialogService.ViewModels
{
    public abstract class DialogBase : BindableBase, IDialogAware
    {
        public event Action<IDialogResult> RequestClose;

        public virtual bool CanCloseDialog()
        {
            return true;
        }
        public virtual void OnDialogClosed()
        {

        }
        public virtual void OnDialogOpened(IDialogParameters parameters)
        {

        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1030:Используйте события, когда это уместно", Justification = "<Ожидание>")]
        public virtual void RaiseRequestClose(IDialogResult dialogResult)
        {
            RequestClose?.Invoke(dialogResult);
        }

        public string Title { get; set; }
    }
}
