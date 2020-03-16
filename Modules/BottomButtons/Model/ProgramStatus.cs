using Infrastructure.ApplicationStates;

namespace BottomButtons.Model
{
    public class ProgramStatus : ProgramStateMachine
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1030:Используйте события, когда это уместно", Justification = "<Ожидание>")]
        public void RaiseProgramIsRunning() => CurrentState = ProgramState.ProgramIsRunning;

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1030:Используйте события, когда это уместно", Justification = "<Ожидание>")]
        public void RaiseProgramIsPaused() => CurrentState = ProgramState.ProgramPaused;

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1030:Используйте события, когда это уместно", Justification = "<Ожидание>")]
        public void RaiseProgramIsAborted() => CurrentState = ProgramState.ProgramStopped;
    }
}
