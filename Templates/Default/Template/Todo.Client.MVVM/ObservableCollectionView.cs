using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;

namespace <# Project.Namespace#>.Client.MVVM;

public class ObservableCollectionView<T> : IReadOnlyList<T>, IList, INotifyCollectionChanged
{
	public event NotifyCollectionChangedEventHandler? CollectionChanged;
	public ObservableCollection<T> SourceCollection { get; private set; }
	private readonly List<T> _filteredItems;
	private readonly Func<T, bool> _filter;
	private readonly Func<T, object>? _order;
	private readonly bool _descending;
	private bool _isEnabled = true;

	public ObservableCollectionView(ObservableCollection<T> sourceCollection, Func<T, bool> filter, Func<T, object>? order = null, bool descending = false)
	{
		SourceCollection = sourceCollection;
		_filteredItems = [];

		_filter = filter;
		_order = order;
		_descending = descending;

		SourceCollection.CollectionChanged += SourceCollectionChanged;
		Refresh();
	}

	public int Count => _filteredItems.Count;

	public T this[int index] => _filteredItems[index];

	bool IList.IsFixedSize => true;
	bool IList.IsReadOnly => true;
	bool ICollection.IsSynchronized => false;
	object ICollection.SyncRoot => this;

	object? IList.this[int index]
	{
		get => _filteredItems[index];
		set => throw new NotSupportedException("Collection is read-only.");
	}

	private void SourceCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
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

	int IList.Add(object? value)
	{
		throw new NotSupportedException("Collection is read-only.");
	}

	void IList.Clear()
	{
		throw new NotSupportedException("Collection is read-only.");
	}

	bool IList.Contains(object? value)
	{
		return value is T item && _filteredItems.Contains(item);
	}

	int IList.IndexOf(object? value)
	{
		return value is T item ? _filteredItems.IndexOf(item) : -1;
	}

	void IList.Insert(int index, object? value)
	{
		throw new NotSupportedException("Collection is read-only.");
	}

	void IList.Remove(object? value)
	{
		throw new NotSupportedException("Collection is read-only.");
	}

	void IList.RemoveAt(int index)
	{
		throw new NotSupportedException("Collection is read-only.");
	}

	void ICollection.CopyTo(Array array, int index)
	{
		((ICollection)_filteredItems).CopyTo(array, index);
	}
}<#cs SetOutputPathAndSkipOtherDefinitions()#>