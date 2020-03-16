using System.Collections.Generic;
using System.ComponentModel;

namespace LaserSettings.Model
{
    public class LaserConfiguration : INotifyPropertyChanged
    {
        public string Comments { get; set; }
        public Material MaterialType { get; set; }
        public float? MaterialThickness { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;

        public void RaiseTablesPropertyChanged()
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Tables)));
        }

        public List<LaserParameterTableBase> Tables { get; set; }
    }
}