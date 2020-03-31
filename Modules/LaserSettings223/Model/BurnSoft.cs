using Newtonsoft.Json;

namespace LaserSettings.Model
{
    public class BurnSoft : BurningBase
    {
        public BurnSoft()
        {
            TableName = this.GetType().Name;
        }
    }
}