using System;
using System.Windows.Input;

namespace HealthyPeopleForFun.Classes
{
	public class CommandHandler : ICommand
	{
		#region Fields
		private Action<object> _action;
		private Func<bool> _canExecute;
		#endregion Fields

		#region Constructor
		/// <summary>
		/// Creates instance of the command handler
		/// </summary>
		/// <param name="action">Action to be executed by the command</param>
		/// <param name="canExecute">A boolean property to containing current permissions to execute the command</param>
		public CommandHandler(Action<object> action, Func<bool> canExecute)
		{
			_action = action;
			_canExecute = canExecute;
		}
		#endregion Constructor

		#region Properties
		/// <summary>
		/// Wires CanExecuteChanged event 
		/// </summary>
		public event EventHandler CanExecuteChanged
		{
			add { CommandManager.RequerySuggested += value; }
			remove { CommandManager.RequerySuggested -= value; }
		}
		#endregion Properties

		#region Functions
		/// <summary>
		/// Forces checking if execute is allowed
		/// </summary>
		/// <param name="parameter">Parameter forwarded to Command</param>
		/// <returns>Bool value that indicates success status</returns>
		public bool CanExecute(object parameter)
		{
			return _canExecute.Invoke();
		}

		/// <summary>
		/// Executes the command
		/// </summary>
		/// <param name="parameter">Parameter forwarded to Command</param>
		public void Execute(object parameter)
		{
			_action(parameter);
		}
		#endregion Functions
	}
}
