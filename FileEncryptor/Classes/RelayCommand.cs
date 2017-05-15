using System;
using System.Windows.Input;

namespace FileEncyptor.Classes
{
	internal class RelayCommand : ICommand
	{
		private readonly Action _execute;
		private readonly Func<bool> _canExecute;

		public RelayCommand(Action execute)
			: this(execute, () => true)
		{
		}

		public RelayCommand(Action execute, Func<bool> canExecute)
		{
			_execute = execute;
			_canExecute = canExecute;
		}

		public bool CanExecute(object parameter)
		{
			return _canExecute();
		}

		public void Execute(object parameter)
		{
			_execute();
		}

		public void RaiseCanExecuteChanged()
		{
			CommandManager.InvalidateRequerySuggested();
		}

		public event EventHandler CanExecuteChanged;
	}
}
