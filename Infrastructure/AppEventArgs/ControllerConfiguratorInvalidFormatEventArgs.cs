using System;

namespace Infrastructure.AppEventArgs
{
    public class ControllerConfiguratorInvalidFormatEventArgs : EventArgs
    {
        public ControllerConfiguratorInvalidFormatEventArgs(string message)
        {
            Message = message;
        }

        public string Message { get; }
    }
}
