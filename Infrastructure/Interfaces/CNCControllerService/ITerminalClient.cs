namespace Infrastructure.Interfaces.CNCControllerService
{
    public interface ITerminalClient
    {
        bool TerminalResponse(string query, out string answer);
    }
}
