using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace <# Project.Namespace#>.Client.MVVM;

public abstract class AbstractViewModel : INotifyPropertyChanged
{
	public event PropertyChangedEventHandler? PropertyChanged;

	protected void RaisePropertiesChanged(params string[] propertyNames)
	{
		foreach (var propertyName in propertyNames)
		{
			RaisePropertyChanged(propertyName);
		}
	}

	protected void RaisePropertyChanged(string? propertyName)
	{
		PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
	}

	protected virtual bool Set<T>(ref T oldValue, T newValue, [CallerMemberName] string? propertyName = null)
	{
		if (EqualityComparer<T>.Default.Equals(oldValue, newValue))
		{
			return false;
		}

		oldValue = newValue;
		RaisePropertyChanged(propertyName);

		return true;
	}

	protected virtual bool Set<T>(ref T oldValue, T newValue, Action onChanged, [CallerMemberName] string? propertyName = null)
	{
		if (Set(ref oldValue, newValue, propertyName))
		{
			onChanged();
			return true;
		}

		return false;
	}
}<#cs SetOutputPathAndSkipOtherDefinitions()#>