using System.Collections.Generic;

namespace LaserSettings.Model
{
    public class BurningCycle
    {
        public BurningCycle()
        {
            CycleArray = new List<BurningBase>();
        }
        public int CyclesCount { get; set; }
        public List<BurningBase> CycleArray { get; }
    }
}