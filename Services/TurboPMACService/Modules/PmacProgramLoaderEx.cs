using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Infrastructure.AggregatorEvents;
using Infrastructure.Constants;
using Infrastructure.Enums;
using Infrastructure.Interfaces.CNCControllerService;
using Infrastructure.Interfaces.Logger;
using Infrastructure.Interfaces.UserSettingService;
using Prism.Events;

namespace ControllerService.Modules
{
	public class PmacProgramLoaderEx
	{
		private readonly IController _controller;
		private readonly ILoggerExtended _loggerExtended;
		private readonly IUserSettingsService _settingsService;
		private readonly IEventAggregator _eventAggregator;
		private NCFile _ncFile;
		private ManualResetEvent _programStartHandler;
		private CancellationTokenSource _rotaryBufferCts;

		public PmacProgramLoaderEx(IController controller, ILoggerExtended loggerExtended,
			IUserSettingsService settingsService, IEventAggregator eventAggregator)
		{
			_controller = controller;
			_loggerExtended = loggerExtended;
			_settingsService = settingsService;
			_eventAggregator = eventAggregator;
			_programStartHandler = new ManualResetEvent(false);
		}

		public async Task<IEnumerable<string>> OpenProgramFileAsync(string path, CancellationToken cancellationToken)
		{
			_ncFile?.Dispose();
			_ncFile = new NCFile(path);

			var result = _ncFile.GetSomeString(0, 16)
				.Replace("\r", string.Empty)
					.Split(Environment.NewLine)
						.ToArray();

			await InitNewRotaryBufferAsync();
			await PrepareRotaryBufferAsync();

			return result;
		}

		public async Task PrepareRotaryBufferAsync()
		{
			_rotaryBufferCts?.Cancel();
			_rotaryBufferCts = new CancellationTokenSource();

			_eventAggregator.GetEvent<MachineLockedState>().Publish(true);

			IsProgramRunning = false;

			await Task.Run(() =>
			{
				ClearStates();
				_controller.GetResponse($"{MVariables.ProgramLineNumberFirst}=0", out _);
				_controller.GetResponse($"{MVariables.ProgramLineNumberSecond}=0", out _);
				_settingsService.CSNumber = 0;
				_controller.GetResponse("ABR0", out _);
				_controller.GetResponse("A", out _);

				StartProgramFollowing();
			}).ConfigureAwait(false);
			

			_programStartHandler.WaitOne(); //Ожидание загрузки в буфер
			_programStartHandler.Reset();

			_eventAggregator.GetEvent<MachineLockedState>().Publish(false);
		}

		private void StartProgramFollowing()
		{
			
		}

		private void ClearStates()
		{
			IsProgramPaused = false;
			IsProgramRunning = false;
		}

		private async Task InitNewRotaryBufferAsync()
		{
			_eventAggregator.GetEvent<MachineLockedState>().Publish(true);
			IsProgramRunning = false;

			await Task.Run(() =>
			{
				ClearStates();
				_controller.GetResponse("A", out _);
				_controller.GetResponse($"{MVariables.ProgramLineNumberFirst}=0", out _);
				_controller.GetResponse($"{MVariables.ProgramLineNumberSecond}=0", out _);
				_controller.GetResponse("DELETE LOOKAHEAD", out _);
				_controller.GetResponse("DELETE ROT", out _);
				_controller.RotBufRemove(0);
				_controller.GetResponse("&1 DEFINE ROT 4096", out _);
				_controller.GetResponse("&1 DEFINE LOOKAHEAD 500,20", out _);
				_controller.RotBufClear(0);
				Task.Delay(100).Wait();
				_controller.RotBufSet(false);
				_controller.RotBufSet(true);
				_controller.RotBufInit();
				_settingsService.CSNumber = 0;

			}).ConfigureAwait(false);
			_eventAggregator.GetEvent<MachineLockedState>().Publish(false);
		}

		public bool IsProgramRunning { get; private set; }
		public bool IsProgramPaused { get; private set; }

	}
}