using Infrastructure.Abstract;
using Infrastructure.Interfaces.CNCControllerService;
using Prism.Commands;
using System;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;

namespace GeneralComponents.ViewModels.BaseViewModel
{
    public abstract class CalculatorBaseViewModel : ViewModelBase
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1051:Не объявляйте видимые поля экземпляров", Justification = "<Ожидание>")]
        protected int _currentCollumn;
        private Visibility _childState;
        private string _childText;
        private bool _isAvailable = true;
        readonly IProgramLoader _programLoader;
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1051:Не объявляйте видимые поля экземпляров", Justification = "<Ожидание>")]
        protected readonly string DecimalSeparator = CultureInfo.InvariantCulture.NumberFormat.NumberDecimalSeparator;

        public CalculatorBaseViewModel(IProgramLoader programLoader)
        {
            _programLoader = programLoader ?? throw new NullReferenceException();

            ChangingCancelCommand = new DelegateCommand(ExecuteChangingCancel);

            _programLoader.PropertyChanged += OnProgrammIsRunned;
        }

        private void ExecuteChangingCancel()
        {
            ChildState = Visibility.Collapsed;
        }
        protected virtual void ExecuteAddNumber(string addableString, Func<int, double> getValueByColumnNumber, int currentColumnNumber)
        {
            switch (addableString)
            {
                case ".":
                    if (DotCanBeAdded()) ChildText += addableString;
                    break;
                case "+":
                case "-":
                    if (MinusOrPlusCanBeAdded()) ChildText += addableString;
                    break;
                case "C":
                    ChildText = string.Empty;
                    break;
                case "Сброс":
                    ChildText = (getValueByColumnNumber ?? throw new NullReferenceException())(currentColumnNumber).ToString("F3", CultureInfo.InvariantCulture);
                    break;
                default:
                    ChildText += addableString;
                    break;
            }

            ChangingDoneCommand?.RaiseCanExecuteChanged();
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Стиль", "IDE0060:Удалите неиспользуемый параметр", Justification = "<Ожидание>")]
        private bool DotCanBeAdded()
        {
            var words = ChildText.Contains("+") ? ChildText.Split('+') : ChildText.Split('-');
            var lastWord = words.Last();
            return (lastWord.Length > 0 && !lastWord.Contains(DecimalSeparator));
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Globalization", "CA1307:Укажите StringComparison", Justification = "<Ожидание>")]
        private bool MinusOrPlusCanBeAdded()
        {
            if (ChildText.Equals("-", StringComparison.Ordinal) || ChildText.Equals("+", StringComparison.Ordinal)) return false;
            var validString = new string(ChildText.Skip(1).ToArray());
            var condition = validString.Length == 0 ? true : validString.Last().ToString(CultureInfo.InvariantCulture).CompareTo(DecimalSeparator) != 0;
            return (!validString.Contains("+") && !validString.Contains("-") && condition);
        }
        private void OnProgrammIsRunned(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName.Equals(nameof(_programLoader.IsProgramRunning), StringComparison.Ordinal))
                IsAvailable = !_programLoader.IsProgramRunning;
        }
        protected double CalculateChildString()
        {
            if (ChildText.Remove(0, 1).Contains("+"))
                return ChildText.Split('+').Select(s => Convert.ToDouble(s, CultureInfo.InvariantCulture)).Aggregate((d1, d2) => d1 + d2);
            if (ChildText.Remove(0, 1).Contains("-"))
            {
                if (ChildText[0] == '-')
                {
                    var result = ChildText.Split('-').Skip(1).Select(s => Convert.ToDouble(s, CultureInfo.InvariantCulture)).Aggregate((d1, d2) => d1 + d2);
                    return -result;
                }
                return ChildText.Split('-').Select(s => Convert.ToDouble(s, CultureInfo.InvariantCulture)).Aggregate((d1, d2) => d1 - d2);
            }
            return Convert.ToDouble(ChildText, CultureInfo.InvariantCulture);
        }
        protected virtual bool CanExecuteChangingDone()
        {
            if (string.IsNullOrEmpty(ChildText))
                return false;
            if (ChildText.Last() == '+' || ChildText.Last() == '-')
                return false;
            Regex reg = new Regex($"^[+-]?[0-9]*[{DecimalSeparator}]?[0-9]*[+-]?[0-9]*[{DecimalSeparator}]?[0-9]+|$");
            if (!reg.IsMatch(ChildText))
                return false;
            if (ChildText.Last().ToString(CultureInfo.InvariantCulture).Equals(DecimalSeparator, StringComparison.Ordinal))
                return false;
            return true;
        }

        public Visibility ChildState
        {
            get => _childState;
            set => SetProperty(ref _childState, value);
        }
        public DataGridCellInfo CurrentCell { get; set; }
        public string ChildText
        {
            get => _childText;
            set => SetProperty(ref _childText, value);
        }
        public bool IsAvailable
        {
            get { return _isAvailable; }
            set { SetProperty(ref _isAvailable, value); }
        }

        public DelegateCommand ChangingCancelCommand { get; private set; }
        public DelegateCommand<string> AddNumberCommand { get; protected set; }
        public DelegateCommand ChangeOffsetCommand { get; protected set; }
        public DelegateCommand ChangingDoneCommand { get; protected set; }
    }
}
