namespace Infrastructure.Interfaces.CNCControllerService
{
    public interface IProgramControl
    {
        void AbortProgram();
        void PauseProgram();
        void ResumeProgram();
        void ResetProgram();
        void StartProgram(int line = 0);
    }
}
