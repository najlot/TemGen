using System;
using System.Windows.Input;

namespace <#cs Write(Project.Namespace)#>.Client.MVVM;

public class RelayCommand : RelayCommand<object>
{
	public RelayCommand(Action action)
		: base(_ => action())
	{
	}

	public RelayCommand(Action action, Func<bool> canExecute)
		: base(_ => action(), _ => canExecute())
	{
	}
}

public class RelayCommand<T> : ICommand
{
	private readonly Action<T> _action;
	private readonly Func<T, bool> _canExecute;

	public event EventHandler CanExecuteChanged;

	public RelayCommand(Action<T> action, Func<T, bool> canExecute = null)
	{
		_action = action;

		if (canExecute != null)
			_canExecute = canExecute;
		else
			_canExecute = ReturnTrue;
	}

	private static bool ReturnTrue(T param) => true;

	public void RaiseCanExecuteChanged()
	{
		CanExecuteChanged?.Invoke(this, EventArgs.Empty);
	}

	public bool CanExecute(object parameter)
	{
		if (parameter == null)
		{
			return _canExecute(default);
		}

		if (!typeof(T).IsAssignableFrom(parameter.GetType()))
		{
			var typeConverter = System.ComponentModel.TypeDescriptor.GetConverter(typeof(T));
			parameter = typeConverter.ConvertFrom(parameter);
		}

		return _canExecute((T)parameter);
	}

	public void Execute(object parameter)
	{
		if (CanExecute(parameter))
		{
			if (parameter == null)
			{
				_action(default);
				return;
			}

			if (!typeof(T).IsAssignableFrom(parameter.GetType()))
			{
				var typeConverter = System.ComponentModel.TypeDescriptor.GetConverter(typeof(T));
				parameter = typeConverter.ConvertFrom(parameter);
			}

			_action((T)parameter);
		}
	}
}<#cs SetOutputPathAndSkipOtherDefinitions()#>