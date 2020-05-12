using GeneralComponents.ViewModels.BaseViewModel;
using HelixToolkit.SharpDX.Core.Model.Scene2D;
using Infrastructure.DialogService;
using Infrastructure.Interfaces.CNCControllerService;
using Infrastructure.Interfaces.UserSettingService;
using Infrastructure.Resources.Strings;
using Prism.Commands;
using Prism.Services.Dialogs;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Windows;

namespace GeneralComponents.ViewModels
{
    using Visibility = System.Windows.Visibility;
    public class OffsetViewModel : CalculatorBaseViewModel
    {
        private IUserSettingsService _userSettings;
        private IOffsetData _currentItem;
        private readonly IControllerInformation _controllerInformation;
        private readonly IDialogService _dialogService;

        public OffsetViewModel(IUserSettingsService userSettings, IProgramLoader programLoader, IControllerInformation controllerInformation, IDialogService dialogService) : base(programLoader)
        {
            _userSettings = userSettings;
            _controllerInformation = controllerInformation;
            _dialogService = dialogService;
            Title = GeneralComponentsStrings.Offsets;
            ChildState = Visibility.Collapsed;

            ResetAllCommand = new DelegateCommand(ResetAllExecute, ResetAllCanExecute);
            SetAllCommand = new DelegateCommand(SetAllExecute, SetAllCanExecute);
            ChangeOffsetCommand = new DelegateCommand(ExecuteChangeOffset);
            ChangingDoneCommand = new DelegateCommand(ExecuteChangingDone, CanExecuteChangingDone);
            AddNumberCommand = new DelegateCommand<string>(ExecuteAddNumber);
            SetOffsetCommand = new DelegateCommand(ExecuteSetOffset);
        }

        private bool SetAllCanExecute()
        {
            return CurrentItem != null;
        }

        private bool ResetAllCanExecute()
        {
            return CurrentItem != null;
        }

        private void ResetAllExecute()
        {
            ShowConfirmationDialog(GeneralComponentsStrings.ResetAllOffsetsTitle, GeneralComponentsStrings.ResetAllOffsetsMessage, () =>
            {
                foreach (var prop in CurrentItem.GetType().GetProperties().Where(p => p.Name.Length == 1 && !p.Name.Contains("I") && !p.Name.Contains("J") && !p.Name.Contains("K")))
                {
                    prop.SetValue(CurrentItem, 0);
                }
            });
        }

        private void ShowConfirmationDialog(string title, string message, Action action)
        {
            var parameters = new DialogParameters
            {
                { "title", title },
                { "message", message }
            };
            _dialogService.ShowDialog(DialogNames.Confirmation, parameters, r =>
            {
                if (r.Result == ButtonResult.Yes)
                    action.Invoke();
            });
        }

        private void SetAllExecute()
        {
            ShowConfirmationDialog(GeneralComponentsStrings.SetAllOffsetsTitle, GeneralComponentsStrings.SetAllOffsetsMessage, () =>
            {
                foreach (var prop in CurrentItem.GetType().GetProperties().Where(p => p.Name.Length == 1 && !p.Name.Contains("I") && !p.Name.Contains("J") && !p.Name.Contains("K")))
                {
                    var motor = _controllerInformation.Motors.FirstOrDefault(m => m.Letter.Equals(prop.Name, StringComparison.Ordinal));
                    var motorOffset = (double)prop.GetValue(CurrentItem);
                    if (motor != null)
                    {
                        var purePosition = motor.RawPosition;
                        prop.SetValue(CurrentItem, purePosition);
                    }
                }
            });
        }

        private void ExecuteSetOffset()
        {
            if (CurrentItem != null)
            {
                var property = GetCurrentItemPropertyByNumber(_currentCollumn);
                var motor = _controllerInformation.Motors.FirstOrDefault(m => m.Letter.Equals(property.Name, StringComparison.Ordinal));
                //var motorOffset = (double)property.GetValue(CurrentItem);
                if (motor != null)
                {
                    ExecuteAddNumber("C");
                    var purePosition = motor.RawPosition; //Убран знак минуса
                    foreach (var l in purePosition.ToString("F3", CultureInfo.InvariantCulture))
                    {
                        ExecuteAddNumber(l.ToString(CultureInfo.InvariantCulture));
                    }
                }
            }
        }

        private void ExecuteAddNumber(string str)
        {
            ExecuteAddNumber(str, GetValueByColumnNumber, _currentCollumn);
        }
        private void ExecuteChangeOffset()
        {
            if (CurrentItem != null && CurrentCell.Column != null)
            {
                var format = "F3";
                _currentCollumn = CurrentCell.Column.DisplayIndex;
                if (_currentCollumn == 0) return;
                var property = GetCurrentItemPropertyByNumber(_currentCollumn);
                var value = (double)property.GetValue(CurrentItem);
                ChildText = value.ToString(format, CultureInfo.InvariantCulture);
                ChildState = Visibility.Visible;
            }
        }
        private void ExecuteChangingDone()
        {
            if (CurrentItem != null)
            {
                var property = GetCurrentItemPropertyByNumber(_currentCollumn);
                property.SetValue(CurrentItem, Convert.ToDouble(CalculateChildString(), CultureInfo.InvariantCulture));
            }
            ChildState = Visibility.Collapsed;
        }
        private PropertyInfo GetCurrentItemPropertyByNumber(int currentCollumn)
        {
            var letters = "XYZABCUVW";
            var property = CurrentItem.GetType().GetProperty(letters[currentCollumn - 1].ToString(CultureInfo.InvariantCulture));
            return property;
        }
        private double GetValueByColumnNumber(int num)
        {
            var property = GetCurrentItemPropertyByNumber(num);
            return (double)property.GetValue(CurrentItem);
        }

        public IOffsetData CurrentItem
        {
            get => _currentItem;
            set
            {
                SetProperty(ref _currentItem, value);
                ResetAllCommand.RaiseCanExecuteChanged();
                SetAllCommand.RaiseCanExecuteChanged();
            }
        }
        public List<IOffsetData> Offsets
        {
            get => _userSettings.Offsets;
        }
        public DelegateCommand SetOffsetCommand { get; }
        public DelegateCommand ResetAllCommand { get; }
        public DelegateCommand SetAllCommand { get; }
    }
}
