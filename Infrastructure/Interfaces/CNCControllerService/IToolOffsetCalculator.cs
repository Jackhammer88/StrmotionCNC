namespace Infrastructure.Interfaces.CNCControllerService
{
    public interface IToolOffsetCalculator
    {
        string ApplyOffsets(string programString, IToolOffsetData toolOffset);
    }
}
