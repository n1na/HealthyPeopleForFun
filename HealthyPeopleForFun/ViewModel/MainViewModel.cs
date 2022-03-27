using HealthyPeopleForFun.Classes;
using HealthyPeopleForFun.Resources;
using Microsoft.Win32;
using System.Media;
using System.Threading;
using System.Windows.Input;

namespace HealthyPeopleForFun.ViewModel
{
	public class MainViewModel : BaseViewModel
	{
		#region Fields
		private ICommand _cancelSettingsCommand;
		private ICommand _dismissCommand;
		private ICommand _exitCommand;
		private bool _isMainWindowVisible = true;
		private bool _isSettingsViewVisible = false;
		private bool _isTimerViewVisible = true;
		private ICommand _openSettingsCommand;
		private int _remainingTimeMinutes = 0;
		private int _remainingTimeSeconds = 0;
		private ICommand _saveSettingsCommand;
		private string _sittingInterval = string.Empty;
		private ICommand _showWindowCommand;
		private bool _soundsEnabled = false;
		private string _standingInterval = string.Empty;
		private string _timerDescription = string.Empty;
		private bool _wasMainWindowVisible = true;
		#endregion Fields

		#region Constructor
		/// <summary>
		/// Creates instance of the Main ViewModel
		/// </summary>
		public MainViewModel()
		{
			LoadSettings();
			StartTimer(TimerTypeEnum.Sitting);
		}
		#endregion Constructor

		#region Properties
		/// <summary>
		/// Handles Cancel button click on Settings view
		/// </summary>
		public ICommand CancelSettingsCommand
		{
			get
			{
				return _cancelSettingsCommand ?? (_cancelSettingsCommand = new CommandHandler((param) => {
					IsMainWindowVisible = _wasMainWindowVisible;
					IsTimerViewVisible = true;
				}, () => true));
			}
		}

		/// <summary>
		/// Handles Dismiss button click on Timer view
		/// </summary>
		public ICommand DismissCommand
		{
			get
			{
				return _dismissCommand ?? (_dismissCommand = new CommandHandler((param) => {
					IsMainWindowVisible = false;
				}, () => true));
			}
		}

		/// <summary>
		/// Handles Close button command on window
		/// </summary>
		public ICommand ExitCommand
		{
			get
			{
				return _exitCommand ?? (_exitCommand = new CommandHandler((param) => {
					App.Current.Shutdown();
				}, () => true));
			}
		}

		/// <summary>
		/// Specifies whether the main window is shown or dismissed
		/// </summary>
		public bool IsMainWindowVisible
		{
			get { return _isMainWindowVisible; }
			set { _isMainWindowVisible = value; Notify(nameof(IsMainWindowVisible)); Notify(nameof(IsSettingsViewVisible)); Notify(nameof(IsTimerViewVisible)); }
		}

		/// <summary>
		/// Specifies whether the Settings view is shown or not
		/// </summary>
		public bool IsSettingsViewVisible
		{
			get { return _isMainWindowVisible && _isSettingsViewVisible; }
			set 
			{ 
				_isSettingsViewVisible = value; 
				Notify(nameof(IsSettingsViewVisible));

				if (value) HideOtherViews(TimerTypeEnum.Settings);
			}
		}

		/// <summary>
		/// Specifies whether the Timer view is shown or not
		/// </summary>
		public bool IsTimerViewVisible
		{
			get { return _isMainWindowVisible && _isTimerViewVisible; }
			set 
			{
				_isTimerViewVisible = value; 
				Notify(nameof(IsTimerViewVisible));

				if (value) HideOtherViews(TimerTypeEnum.Sitting);
			}
		}

		/// <summary>
		/// Handles Settings button command on window
		/// </summary>
		public ICommand OpenSettingsCommand
		{
			get
			{
				return _openSettingsCommand ?? (_openSettingsCommand = new CommandHandler((param) => {
					_wasMainWindowVisible = IsMainWindowVisible;
					LoadSettings();
					IsSettingsViewVisible = true;
					IsMainWindowVisible = true;
				}, () => true));
			}
		}

		/// <summary>
		/// Specifies the amount of minutes left for sitting/standing
		/// </summary>
		public int RemainingTimeMinutes
		{
			get { return _remainingTimeMinutes; }
			set { _remainingTimeMinutes = value; Notify(nameof(RemainingTimeMinutes)); }
		}

		/// <summary>
		/// Specifies the amount of seconds left for sitting/standing
		/// </summary>
		public int RemainingTimeSeconds
		{
			get { return _remainingTimeSeconds; }
			set { _remainingTimeSeconds = value; Notify(nameof(RemainingTimeSeconds)); }
		}

		/// <summary>
		/// Handles Save button click on Settings view
		/// </summary>
		public ICommand SaveSettingsCommand
		{
			get
			{
				return _saveSettingsCommand ?? (_saveSettingsCommand = new CommandHandler((param) => {
					SaveSettings();
				}, () => true));
			}
		}

		/// <summary>
		/// Handles Open button click in context menu
		/// </summary>
		public ICommand ShowWindowCommand
		{
			get
			{
				return _showWindowCommand ?? (_showWindowCommand = new CommandHandler((param) => {
					IsMainWindowVisible = true;
				}, () => true));
			}
		}

		/// <summary>
		/// Specifies the amount of minutes, a user has chosen as the sitting interval
		/// </summary>
		public string SittingInterval
		{
			get { return _sittingInterval; }
			set
			{
				if (!string.IsNullOrEmpty(value))
				{
					// simple TextBox is used on UI - input is validated manually
					int intVal;
					if (!int.TryParse(value, out intVal))
					{
						return;
					}
					if (intVal > 300) // max number of minutes for an interval is 300 - if more: set to max
					{
						intVal = 300;
						value = Strings.MaxIntervalMinutes;
					}
					SittingIntervalValue = intVal;
				}
				_sittingInterval = value;
				Notify(nameof(SittingInterval));
			}
		}

		/// <summary>
		/// Specifies the sitting interval length
		/// </summary>
		public int SittingIntervalValue { get; set; }

		/// <summary>
		/// Specifies whether playing notification sound is enabled in settings
		/// </summary>
		public bool SoundsEnabled
		{
			get { return _soundsEnabled; }
			set { _soundsEnabled = value; Notify(nameof(SoundsEnabled)); }
		}

		/// <summary>
		/// Specifies the amount of minutes, a user has chosen as the standing interval
		/// </summary>
		public string StandingInterval
		{
			get { return _standingInterval; }
			set
			{
				if (!string.IsNullOrEmpty(value))
				{
					// simple TextBox is used on UI - input is validated manually
					int intVal;
					if (!int.TryParse(value, out intVal))
					{
						return;
					}
					if (intVal > 300) // max number of minutes for an interval is 300 - if more: set to max
					{
						intVal = 300;
						value = Strings.MaxIntervalMinutes;
					}
					StandingIntervalValue = intVal;
				}
				_standingInterval = value;
				Notify(nameof(StandingInterval));
			}
		}

		/// <summary>
		/// Specifies the standing interval length
		/// </summary>
		public int StandingIntervalValue { get; set; }

		/// <summary>
		/// Specifies the text to be shown along with countdown
		/// </summary>
		public string TimerDescription
		{
			get { return _timerDescription; }
			set { _timerDescription = value; Notify(nameof(TimerDescription)); }
		}

		/// <summary>
		/// Specifies the Timer used for countdown
		/// </summary>
		public Timer Timer { get; set; }

		/// <summary>
		/// Specifies elapsed timer time (in seconds)
		/// </summary>
		public int TimerRuntime { get; set; } = 0;
		#endregion Properties

		#region Functions
		/// <summary>
		/// Hides Settings view if timer is visible and vice-versa
		/// </summary>
		/// <param name="viewType">Type of view that's shown</param>
		public void HideOtherViews(TimerTypeEnum viewType)
		{
			switch (viewType)
			{
				case TimerTypeEnum.Settings:
					IsTimerViewVisible = false;
					break;
				default:
					IsSettingsViewVisible = false;
					break;
			}
		}

		/// <summary>
		/// Loads settings and sets values
		/// </summary>
		public void LoadSettings()
		{
			SittingInterval = Properties.Settings.Default.SittingInterval.ToString();
			StandingInterval = Properties.Settings.Default.StandingInterval.ToString();
			SoundsEnabled = Properties.Settings.Default.PlaySound;
		}

		/// <summary>
		/// Plays notification sound if the setting is enabled
		/// </summary>
		public void PlaySound()
		{
			if (SoundsEnabled)
			{
				const string key = @"AppEvents\Schemes\Apps\.Default\Notification.Default\.Default";
				using (var reg = Registry.CurrentUser.OpenSubKey(key))
				using (var player = new SoundPlayer((string)reg.GetValue(string.Empty)))
				{
					player.Play();
				}
			}
		}

		/// <summary>
		/// Saves Settings
		/// </summary>
		public void SaveSettings()
		{
			bool settingsHaveChanged = Properties.Settings.Default.SittingInterval != SittingIntervalValue || Properties.Settings.Default.StandingInterval != StandingIntervalValue;
			
			Properties.Settings.Default.SittingInterval = SittingIntervalValue;
			Properties.Settings.Default.StandingInterval = StandingIntervalValue;
			Properties.Settings.Default.PlaySound = SoundsEnabled;

			Properties.Settings.Default.Save();

			LoadSettings();

			IsMainWindowVisible = _wasMainWindowVisible; // show main window only if it was shown before accessing Settings view

			if (settingsHaveChanged) // if sitting or standing interval has changes - restart timer
			{
				Timer.Dispose();
				Timer = null;
				IsMainWindowVisible = true;
				StartTimer(TimerTypeEnum.Sitting);
			}

			IsTimerViewVisible = true;
		}

		/// <summary>
		/// Sets up sitting or standing timer
		/// </summary>
		/// <param name="timerType">Type of timer to start</param>
		public void StartTimer(TimerTypeEnum timerType)
		{
			switch (timerType)
			{
				case TimerTypeEnum.Sitting:
					TimerDescription = Strings.SittingDescription;
					RemainingTimeMinutes = SittingIntervalValue;
					RemainingTimeSeconds = 0;
					TimerRuntime = SittingIntervalValue * 60;
					Timer = new Timer(Tick, TimerTypeEnum.Standing, 0, 1000);
					break;
				case TimerTypeEnum.Standing:
					TimerDescription = Strings.StandFor;
					RemainingTimeMinutes = StandingIntervalValue;
					RemainingTimeSeconds = 0;
					TimerRuntime = StandingIntervalValue * 60;
					Timer = new Timer(Tick, TimerTypeEnum.Sitting, 0, 1000);
					break;
				default:
					break;
			}
		}

		/// <summary>
		/// Decreases the amount of minutes and seconds shown to the user on 1 second interval
		/// </summary>
		/// <param name="timerType">Type of timer that triggered the event</param>
		private void Tick(object timerType)
		{
			if (TimerRuntime > 1) // timer runtime is set to interval - decrease it each second until it reaches 1
			{
				TimerRuntime--;
				RemainingTimeMinutes = TimerRuntime / 60;
				RemainingTimeSeconds = TimerRuntime % 60;
			}
			else
			{
				// killing timer
				Timer.Dispose();
				Timer = null;

				// restart timer with different type
				IsMainWindowVisible = true;
				StartTimer((TimerTypeEnum)timerType);
				PlaySound();
			}
		}
		#endregion Functions

	}
}
