using Infrastructure.Enums;
using Newtonsoft.Json;

namespace LaserSettings.Model
{
    public class LargeContour : CuttingBase
    {
        public LargeContour()
        {
            TableName = this.GetType().Name;
        }
    }
}