using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Todo.Client.MVVM;

public abstract class AbstractViewModel : INotifyPropertyChanged
{
	public event PropertyChangedEventHandler? PropertyChanged;

	public IPropertyChangeVisitor? ChangeVisitor { get; set; }

	protected virtual bool ShouldTrackChange(string? propertyName) => true;

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
		if (EqualityComparer<T>.Default.Equals(oldValue, default) &&
			EqualityComparer<T>.Default.Equals(newValue, default))
		{
			return false;
		}

		if (oldValue?.Equals(newValue) ?? false)
		{
			return false;
		}

		var previous = oldValue;
		oldValue = newValue;
		RaisePropertyChanged(propertyName);

		if (ChangeVisitor is { IsApplyingChange: false } visitor 
			&& !string.IsNullOrEmpty(propertyName)
			&& ShouldTrackChange(propertyName))
		{
			visitor.Visit(this, propertyName, previous, newValue);
		}

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
}