using Infrastructure.Enums;

namespace LaserSettings.Model
{
    public interface ILaserTableBase
    {
        string TableName { get; set; }
        float? LaserPower { get; set; }
        float? MinLaserPower { get; set; }
        float? Gap { get; set; }
        float? LaserFocus { get; set; }
        Gas GasType { get; set; }
        float? GasPressure { get; set; }
        float? ImpFrequeency { get; set; }
        float? ImpDutyCycle { get; set; }
        float? SensorSensitivityI1 { get; set; }
    }
}