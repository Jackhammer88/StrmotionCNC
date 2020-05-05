using Infrastructure.Abstract;
using Infrastructure.Interfaces.CNCControllerService;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Diag.ViewModels
{
    public class DiagnosticsViewModel : ViewModelBase
    {
        private readonly IController _controller;
        private CancellationTokenSource _cancellationToken;

        public DiagnosticsViewModel(IController controller)
        {
            Title = "DIAG";
            _controller = controller;

            _controller.OnConnect += ControllerConnected;
            _controller.OnDisconnected += ControllerDisconnected;
        }

        private void ControllerConnected(object sender, EventArgs e)
        {
            if (_cancellationToken != null)
                _cancellationToken.Cancel();
            _cancellationToken = new CancellationTokenSource();

            Task.Run(GetDiagData);
        }
        private void ControllerDisconnected(object sender, EventArgs e)
        {
            _cancellationToken.Cancel();
        }


        private void GetDiagData()
        {
            while (!_cancellationToken.IsCancellationRequested)
            {
                
            }
        }

        private int _m910;

        public int M910
        {
            get { return _m910; }
            set { _m910 = value; }
        }
        private int _m911;

        public int M911
        {
            get { return _m911; }
            set { _m911 = value; }
        }
        private int myVar;

        public int MyProperty
        {
            get { return myVar; }
            set { myVar = value; }
        }

    }
}
