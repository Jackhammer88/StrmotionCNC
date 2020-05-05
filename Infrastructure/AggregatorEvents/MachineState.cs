using Prism.Events;

namespace Infrastructure.AggregatorEvents
{
    /// <summary>
    /// Событие смены состояния станка (AUTO - 1; MDI - 2; MANUAL - 3)
    /// </summary>
    public class MachineState : PubSubEvent<MachineStateType> { }
    /// Cостояние станка (AUTO - 1; MDI - 2; MANUAL - 3)
    public enum MachineStateType
    {
        Unknown = 0,
        Auto = 1,
        Manual = 2,
        MDI = 3,
        Homing = 4,
        Test = 6
    }
    /*
     *                              //MODE_AUTO				1
									//MODE_MANUAL			2
									//MODE_MDI				3
									//MODE_HOME				4
									//MODE_CAL				5
									//MODE_TEST				6
     */
}
