using Newtonsoft.Json;

namespace LaserSettings.Model
{
    public class Punching : BurningBase
    {
        public Punching()
        {
            TableName = GetType().Name;
        }
    }
}