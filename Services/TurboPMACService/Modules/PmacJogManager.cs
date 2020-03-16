using Infrastructure.Constants;
using Infrastructure.Interfaces.CNCControllerService;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ControllerService.Modules
{
    public class PmacJogManager : IManualMachineControl
    {
        readonly IController _controller;
        readonly IControllerInformation _controllerInformation;
        readonly IControllerConfigurator _controllerConfigurator;
        private Dictionary<int, int> _oldFeedrateVelocity = new Dictionary<int, int>();
        private string _lastJogCommand;

        public PmacJogManager(IController controller, IControllerInformation controllerInformation, IControllerConfigurator controllerConfigurator)
        {
            _controllerConfigurator = controllerConfigurator;
            _controllerInformation = controllerInformation;
            _controller = controller;
        }

        public void Homing(string axis)
        {
            //var motors = _controllerInformation.Motors.Where(m => m.Activated && string.Compare(m.Letter, axis) == 0);
            //string query = string.Empty;
            //foreach (var motor in motors)
            //{
            //    query += $"#{_controllerInformation.Motors.IndexOf(motor) + 1}HOMEZ";
            //}
            //_controller.GetResponse(query, out _);
            _controllerConfigurator.SetVariable(PVariables.Homing, 1);
        }

        public void JogIncrementally(string axis, int stepLength, bool negativeDirection, int velocityFactor = 1)
        {
            var motors = _controllerInformation.Motors.Where(m => m.Activated && string.Equals(m.Letter, axis, StringComparison.Ordinal));
            string direction = negativeDirection ? "-" : "+";
            string command = string.Empty;
            foreach (var motor in motors)
            {
                command += $"#{_controllerInformation.Motors.IndexOf(motor) + 1}J:{direction}{stepLength} ";
            }
            _controller.GetResponse(command, out _);
        }
        public void StopJog()
        {
            if (!string.IsNullOrEmpty(_lastJogCommand))
            {
                _controller.GetResponse(_lastJogCommand.Replace("+", "/").Replace("-", "/"), out _);
                //_controller.GetResponse("J/", out _);
                RestoreOldFeedrateVelocity();
            }
            else
                _controller.GetResponse("J/", out _);
        }
        public void TryJog(string axis, bool negativeDirection, int velocityFactor = 1)
        {
            var motors = _controllerInformation.Motors.Where(m => m.Activated && string.Equals(m.Letter, axis, StringComparison.Ordinal));
            string direction = negativeDirection ? "-" : "+";
            string command = string.Empty;
            foreach (var motor in motors)
            {
                command += $"#{_controllerInformation.Motors.IndexOf(motor) + 1}J{direction} ";
                RememberOldFeedrateVelocity(_controllerInformation.Motors.IndexOf(motor) + 1);
                _controllerConfigurator.SetVariable($"I{_controllerInformation.Motors.IndexOf(motor) + 1}22", 20 * velocityFactor);
            }
            _controller.GetResponse(command, out _);
            _lastJogCommand = command;
        }
        private void RememberOldFeedrateVelocity(int motorNumber)
        {
            _controllerConfigurator.GetVariable($"I{motorNumber}22", out int oldVelocity);
            if (_oldFeedrateVelocity.ContainsKey(motorNumber))
                _oldFeedrateVelocity[motorNumber] = oldVelocity;
            else
                _oldFeedrateVelocity.Add(motorNumber, oldVelocity);

        }
        private void RestoreOldFeedrateVelocity()
        {
            foreach (var motor in _oldFeedrateVelocity)
            {
                _controllerConfigurator.SetVariable($"I{motor.Key}22", motor.Value);
            }
            _oldFeedrateVelocity.Clear();
        }
    }
}
