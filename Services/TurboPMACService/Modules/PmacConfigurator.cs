using Infrastructure.AppEventArgs;
using Infrastructure.Interfaces.CNCControllerService;
using Prism.Logging;
using System;
using System.Globalization;
using System.Text.RegularExpressions;

namespace ControllerService.Modules
{
    public class PmacConfigurator : IControllerConfigurator
    {
        readonly IController _controller;
        readonly ILoggerFacade _logger;
        public PmacConfigurator(IController controller, ILoggerFacade logger)
        {
            _logger = logger;
            _controller = controller;
            OnInvalidInputFormat += (s, e) => _logger.Log(e.Message, Category.Warn, Priority.Medium);
        }
        private bool Connected => _controller.Connected;
        /// <summary>
        /// Устанавливает значение типа int для заданной переменной.
        /// </summary>
        /// <param name="varName">Имя заданной переменной.</param>
        /// <param name="value">Значение для заданной переменной.</param>
        /// <returns>Успешность операции.</returns>
        public bool SetVariable(string varName, int value)
        {
            if (!Connected)
                return false;
            if (Regex.Match(varName, @"^[IMPimp]\d+$").Success)
            {
                var result = _controller.GetResponse($"{varName}={value}", out string response);
                if (result && response.Length == 0)
                    return true;
            }
            OnInvalidInputFormat(this, new ControllerConfiguratorInvalidFormatEventArgs($"Неверный формат запроса при установке переменной: {varName}"));
            return false;
        }
        /// <summary>
        /// Устанавливает значение типа double для заданной переменной.
        /// </summary>
        /// <param name="varName">Имя заданной переменной.</param>
        /// <param name="value">Значение для заданной переменной.</param>
        /// <returns>Успешность операции.</returns>
        public bool SetVariable(string varName, double value)
        {
            if (!Connected)
                return false;
            if (Regex.Match(varName, @"^[IMPimp]\d+$").Success)
            {
                var result = _controller.GetResponse($"{varName}={value}", out string response);
                if (result && response.Length == 0)
                    return true;
            }
            OnInvalidInputFormat(this, new ControllerConfiguratorInvalidFormatEventArgs($"Неверный формат запроса при установке переменной: {varName}"));
            return false;
        }
        /// <summary>
        /// Возвращает значение типа int указанной переменной.
        /// </summary>
        /// <param name="varName">Имя переменной.</param>
        /// <param name="result">Значение переменной.</param>
        /// <returns>Успешность операции.</returns>
        public bool GetVariable(string variableName, out int result)
        {
            result = -1;
            if (!Connected)
                return false;
            bool parseSuccess = false;
            if (Regex.Match(variableName, @"^[IMPimp]\d+$").Success && _controller.GetResponse($"{variableName}", out string response))
            {
                if (response.Contains("$"))
                    parseSuccess = int.TryParse(response.Trim('$'), NumberStyles.HexNumber, CultureInfo.InvariantCulture, out result);
                else
                    parseSuccess = int.TryParse(response, NumberStyles.Any, CultureInfo.InvariantCulture, out result);

                return parseSuccess;
            }
            OnInvalidInputFormat(this, new ControllerConfiguratorInvalidFormatEventArgs($"Неверный формат запроса при получении переменной: {variableName}"));
            return parseSuccess;
        }
        /// <summary>
        /// Возвращает значение типа double указанной переменной.
        /// </summary>
        /// <param name="varName">Имя переменной.</param>
        /// <param name="result">Результат операции.</param>
        /// <returns>Успешность операции.</returns>
        public bool GetVariable(string varName, out double result)
        {
            result = -1;
            if (!Connected)
                return false;
            bool parseSuccess = false;
            if (Regex.Match(varName, @"^[IMPimp]\d+$").Success && _controller.GetResponse($"{varName}", out string response) && !string.IsNullOrEmpty(response))
            {
                if (response[0] == '$')
                    parseSuccess = (double.TryParse(response.Trim('$'), NumberStyles.HexNumber, CultureInfo.InvariantCulture, out result));
                else
                    parseSuccess = double.TryParse(response, NumberStyles.Any, CultureInfo.InvariantCulture, out result);
                return parseSuccess;
            }
            OnInvalidInputFormat(this, new ControllerConfiguratorInvalidFormatEventArgs($"Неверный формат запроса при получении переменной: {varName}"));
            return parseSuccess;
        }

        public event EventHandler<ControllerConfiguratorInvalidFormatEventArgs> OnInvalidInputFormat = delegate { };
    }
}
