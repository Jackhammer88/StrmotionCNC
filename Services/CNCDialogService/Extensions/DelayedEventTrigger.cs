using System;
using System.Threading.Tasks;
using System.Windows;
using EventTrigger = Microsoft.Xaml.Behaviors.EventTrigger;

namespace CNCDialogService.Extensions
{
    public class DelayedEventTrigger : EventTrigger
    {
        bool _delay;

        public int Delay
        {
            get { return (int)GetValue(DelayProperty); }
            set { SetValue(DelayProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Delay.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty DelayProperty =
            DependencyProperty.Register("Delay", typeof(int), typeof(DelayedEventTrigger), new PropertyMetadata(100));



        protected override void OnEvent(EventArgs eventArgs)
        {
            if (_delay)
                return;
            base.OnEvent(eventArgs);
            _delay = true;
            Task.Delay(Delay).Wait();
            _delay = false;
        }
    }
}
