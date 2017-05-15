using System;
using System.Windows.Input;

namespace FileEncyptor.Classes
{
	internal class RelayCommand<T> : ICommand
	{
		private readonly Action<T> _execute;
		private readonly Func<T, bool> _canExecute;

		public RelayCommand(Action<T> execute)
			: this(execute, (o) => true)
		{
		}

		public RelayCommand(Action<T> execute, Func<T, bool> canExecute)
		{
			_execute = execute;
			_canExecute = canExecute;
		}

		public bool CanExecute(object parameter)
		{
			return _canExecute((T)parameter);
		}

		public void Execute(object parameter)
		{
			_execute((T)parameter);
		}

		public void RaiseCanExecuteChanged()
		{
			CommandManager.InvalidateRequerySuggested();
		}

		public event EventHandler CanExecuteChanged;
	}
}
