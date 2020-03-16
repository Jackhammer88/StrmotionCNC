namespace Infrastructure.Interfaces.CNCControllerService
{
    public interface IManualMachineControl
    {
        void StopJog();
        void TryJog(string axis, bool negativeDirection, int velocityFactor = 1);
        void JogIncrementally(string axis, int stepLength, bool negativeDirection, int velocityFactor = 1);
        void Homing(string v);
    }
}
