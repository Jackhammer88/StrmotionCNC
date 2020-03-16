namespace Infrastructure.Enums
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Naming", "CA1717:Только перечисления FlagsAttribute должны иметь имена во множественном числе", Justification = "<Ожидание>")]
    public enum Gas : int
    {
        Unknown = 0,
        Air,
        Oxygen,
        Nitrogen
    }
}
