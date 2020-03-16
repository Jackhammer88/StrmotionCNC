using Infrastructure.AppEventArgs;
using System;

namespace Infrastructure.Interfaces.CNCControllerService
{
    public interface IControllerConfigurator
    {
        event EventHandler<ControllerConfiguratorInvalidFormatEventArgs> OnInvalidInputFormat;

        bool GetVariable(string varName, out double result);
        bool GetVariable(string variableName, out int result);
        bool SetVariable(string varName, double value);
        bool SetVariable(string varName, int value);
    }
}