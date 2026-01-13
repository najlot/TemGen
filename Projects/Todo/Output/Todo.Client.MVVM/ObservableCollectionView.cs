using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Todo.Client.MVVM;

public class ObservableCollectionView<T> : IEnumerable<T>, INotifyCollectionChanged
{
	public ObservableCollection<T> SourceCollection { get; private set; }
	private readonly List<T> _filteredItems;
	private readonly Func<T, bool> _filter;
	private readonly Func<T, object> _order;
	private readonly bool _descending;
	private bool _isEnabled = true;

	public ObservableCollectionView(ObservableCollection<T> sourceCollection, Func<T, bool> filter, Func<T, object> order = null, bool descending = false)
	{
		SourceCollection = sourceCollection;
		_filteredItems = new List<T>(sourceCollection);

		_filter = filter;
		_order = order;
		_descending = descending;

		SourceCollection.CollectionChanged += SourceCollectionChanged;
	}

	private void SourceCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
	{
		Refresh();
	}

	public void Refresh()
	{
		if (!_isEnabled)
		{
			return;
		}

		var enumerator = SourceCollection
				.Where(_filter);

		if (_order != null)
		{
			if (_descending)
			{
				enumerator = enumerator.OrderByDescending(_order);
			}
			else
			{
				enumerator = enumerator.OrderBy(_order);
			}
		}

		_filteredItems.Clear();
		_filteredItems.AddRange(enumerator);

		CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
	}

	public void Enable()
	{
		if (_isEnabled)
		{
			return;
		}

		_isEnabled = true;
		Refresh();
	}

	public void Disable()
	{
		_isEnabled = false;
	}

	public IEnumerator<T> GetEnumerator()
	{
		return _filteredItems.GetEnumerator();
	}

	IEnumerator IEnumerable.GetEnumerator()
	{
		return _filteredItems.GetEnumerator();
	}

	public event NotifyCollectionChangedEventHandler CollectionChanged;
}