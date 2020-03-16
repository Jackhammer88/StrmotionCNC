using Infrastructure.Enums;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Infrastructure.SharedClasses
{
    public class MachineStatuses
    {
        public MachineStatuses(GlobalStatusXs xs, GlobalStatusYs ys, IEnumerable<Tuple<MotorXStatuses, MotorYStatuses>> motors)
        {
            GlobalX = xs;
            GlobalY = ys;
            NumberOfMotors = motors.Count();
            MotorsStatuses = new List<Tuple<MotorXStatuses, MotorYStatuses>>(motors);
        }
        public GlobalStatusXs GlobalX { get; }
        public GlobalStatusYs GlobalY { get; }
        public int NumberOfMotors { get; }
        public List<Tuple<MotorXStatuses, MotorYStatuses>> MotorsStatuses { get; }
    }
}