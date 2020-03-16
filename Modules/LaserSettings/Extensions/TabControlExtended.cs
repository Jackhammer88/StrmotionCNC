using LaserSettings.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace LaserSettings.Extensions
{
    public class TabControlExtended : TabControl
    {
        public TabControlExtended()
        {
            
        }
        public void OrderCollection()
        {
            var testList = ItemsSource.Cast<LaserParameterTableBase>().OrderBy(t => t.IsFavourite).ToList();
            ItemsSource = testList;
        }
    }
}
