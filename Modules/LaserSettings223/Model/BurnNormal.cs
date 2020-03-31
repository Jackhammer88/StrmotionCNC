using Newtonsoft.Json;

namespace LaserSettings.Model
{
    public class BurnNormal : BurningBase
    {
        public BurnNormal()
        {
            TableName = this.GetType().Name;
        }
    }
}