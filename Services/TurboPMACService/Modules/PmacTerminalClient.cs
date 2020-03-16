using Infrastructure.Interfaces.CNCControllerService;

namespace ControllerService.Modules
{
    public class PmacTerminalClient : ITerminalClient
    {
        readonly IController _controller;
        public PmacTerminalClient(IController controller)
        {
            _controller = controller;
        }
        public bool TerminalResponse(string query, out string answer)
        {
            answer = "Not connected.";
            if (!_controller.Connected)
                return false;
            return _controller.GetResponse(query, out answer);
        }
    }
}
