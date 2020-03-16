using Infrastructure.Enums;
using Newtonsoft.Json;

namespace LaserSettings.Model
{
    public abstract class CuttingBase : LaserParameterTableBase, ILaserTableBase
    {
        public string TableName { get; set; }
        public float? NozzleDiameter { get; set; }
        public float? Acceleration { get; set; }
        public float? BeamDiameter { get; set; }
        public bool? RoundedOn { get; set; }
        public float? LaserPower { get; set; }
        public float? MinLaserPower { get; set; }
        public float? Gap { get; set; }
        public float? LaserFocus { get; set; }
        public Gas GasType { get; set; }
        public float? GasPressure { get; set; }
        public float? ImpFrequeency { get; set; }
        public float? ImpDutyCycle { get; set; }
        public float? SensorSensitivityI1 { get; set; }
    }
}