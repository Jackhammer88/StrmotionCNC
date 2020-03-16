namespace LaserSettings.Model
{
    public class Engraving : CuttingBase
    {
        public Engraving()
        {
            TableName = this.GetType().Name;
        }
    }
}