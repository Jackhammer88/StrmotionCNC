using System;

namespace Infrastructure.AppEventArgs
{
    public class ControlPanelDataRecievedEventArgs : EventArgs
    {
        public ControlPanelDataRecievedEventArgs(int arg1, int arg2)
        {
            Data1 = arg1;
            Data2 = arg2;
        }
        public int Data1 { get; }
        public int Data2 { get; }
    }
}