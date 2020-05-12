using GeneralComponents.ViewModels.BaseViewModel;
using Infrastructure.Interfaces.CNCControllerService;
using Infrastructure.Interfaces.UserSettingService;
using Infrastructure.Resources.Strings;
using Prism.Commands;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace GeneralComponents.ViewModels
{
    public class ToolOffsetsViewModel : CalculatorBaseViewModel
    {
        private IUserSettingsService _userSettings;

        public ToolOffsetsViewModel(IUserSettingsService userSettings, IProgramLoader programLoader) : base(programLoader)
        {
            Title = GeneralComponentsStrings.ToolOffsets;
            _userSettings = userSettings;
            ChildState = System.Windows.Visibility.Collapsed;

            ChangeOffsetCommand = new DelegateCommand(ExecuteChangeOffset);
            ChangingDoneCommand = new DelegateCommand(ExecuteChangingDone, CanExecuteChangingDone);
            AddNumberCommand = new DelegateCommand<string>(ExecuteAddNumber);
        }

        private void ExecuteAddNumber(string str)
        {
            base.ExecuteAddNumber(str, GetValueByColumnNumber, _currentCollumn);
        }
        private void ExecuteChangeOffset()
        {
            if (CurrentItem != null && CurrentCell.Column != null)
            {
                string message;
                _currentCollumn = CurrentCell.Column.DisplayIndex;
                var format = "F3";
                switch (CurrentCell.Column.DisplayIndex)
                {
                    case 1:
                        message = CurrentItem.ToolLength.ToString(format, CultureInfo.InvariantCulture);
                        break;
                    case 2:
                        message = CurrentItem.ToolDiameter.ToString(format, CultureInfo.InvariantCulture);
                        break;
                    default:
                        return;
                }
                ChildText = message;
                ChildState = System.Windows.Visibility.Visible;
            }
        }
        private void ExecuteChangingDone()
        {
            if (CurrentItem != null)
            {
                if (_currentCollumn == 1)
                    CurrentItem.ToolLength = Convert.ToDouble(CalculateChildString(), CultureInfo.InvariantCulture);
                else if (_currentCollumn == 2)
                    CurrentItem.ToolDiameter = Convert.ToDouble(CalculateChildString(), CultureInfo.InvariantCulture);
            }
            ChildState = System.Windows.Visibility.Collapsed;
        }
        private double GetValueByColumnNumber(int num)
        {
            switch (num)
            {
                case 1:
                    return CurrentItem.ToolLength;
                case 2:
                    return CurrentItem.ToolDiameter;
                default:
                    return 0;
            }
        }

        public IToolOffsetData CurrentItem { get; set; }
        public List<IToolOffsetData> Offsets
        {
            get => _userSettings.ToolOffsets;
        }
    }
}
