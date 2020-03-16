using Prism.Logging;
using System;
using System.Windows.Controls;

namespace GeneralComponents.Views
{
    /// <summary>
    /// Логика взаимодействия для Plot.xaml
    /// </summary>
    public partial class Plot : UserControl
    {
        private readonly ILoggerFacade _loggerFacade;

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Не перехватывать исключения общих типов", Justification = "<Ожидание>")]
        public Plot(ILoggerFacade loggerFacade)
        {
            _loggerFacade = loggerFacade ?? throw new ArgumentNullException(nameof(loggerFacade));
            try
            {
                InitializeComponent();
            }
            catch (Exception ex)
            {
                _loggerFacade.Log(ex.Message, Category.Exception, Priority.High);
            }

        }
    }
}
