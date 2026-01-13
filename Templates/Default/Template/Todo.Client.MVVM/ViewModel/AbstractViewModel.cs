using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace <#cs Write(Project.Namespace)#>.Client.MVVM.ViewModel;

public abstract class AbstractViewModel : INotifyPropertyChanged
{
	public event PropertyChangedEventHandler PropertyChanged;

	protected void RaisePropertyChanged(string propertyName)
	{
		PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
	}

	protected virtual bool Set<T>(ref T oldValue, T newValue, [CallerMemberName] string propertyName = null)
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

		oldValue = newValue;
		RaisePropertyChanged(propertyName);

		return true;
	}

	protected virtual bool Set<T>(string propertyName, ref T oldValue, T newValue)
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

		oldValue = newValue;
		RaisePropertyChanged(propertyName);

		return true;
	}
}<#cs SetOutputPathAndSkipOtherDefinitions()#>