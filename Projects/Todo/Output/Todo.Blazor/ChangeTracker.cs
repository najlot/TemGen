using System;
using System.Collections.Generic;

namespace Todo.Blazor;

public record PropertyChange(string PropertyName, Action Undo, Action Redo);

public class ChangeTracker
{
	private readonly Stack<PropertyChange> _undoStack = new();
	private readonly Stack<PropertyChange> _redoStack = new();

	public bool IsApplyingChange { get; private set; }
	public bool CanUndo => _undoStack.Count > 0;
	public bool CanRedo => _redoStack.Count > 0;
	public bool HasChanges => _undoStack.Count > 0;

	public event Action? StateChanged;

	public void TrackChange<T>(object target, string propertyName, T oldValue, T newValue)
	{
		if (IsApplyingChange)
		{
			return;
		}

		var property = target.GetType().GetProperty(propertyName);

		if (property is null)
		{
			return;
		}

		var hadAnyRedo = _redoStack.Count > 0;
		var hadAnyUndo = _undoStack.Count > 0;

		property.SetValue(target, newValue);

		_undoStack.Push(new PropertyChange(
			propertyName,
			() => property.SetValue(target, oldValue),
			() => property.SetValue(target, newValue)));

		_redoStack.Clear();

		if (hadAnyRedo || !hadAnyUndo)
		{
			// Only trigger state changed if we went from no changes to having changes,
			// or if we had redo actions that are now cleared.
			StateChanged?.Invoke();
		}
	}

	public void Undo()
	{
		if (_undoStack.Count == 0)
		{
			return;
		}

		var change = _undoStack.Pop();
		IsApplyingChange = true;

		try
		{
			change.Undo();
		}
		finally
		{
			IsApplyingChange = false;
		}

		_redoStack.Push(change);
		StateChanged?.Invoke();
	}

	public void Redo()
	{
		if (_redoStack.Count == 0)
		{
			return;
		}

		var change = _redoStack.Pop();
		IsApplyingChange = true;

		try
		{
			change.Redo();
		}
		finally
		{
			IsApplyingChange = false;
		}

		_undoStack.Push(change);
		StateChanged?.Invoke();
	}

	public void Track(string description, Action undo, Action redo)
	{
		if (IsApplyingChange)
		{
			return;
		}

		var hadAnyRedo = _redoStack.Count > 0;
		var hadAnyUndo = _undoStack.Count > 0;

		_undoStack.Push(new PropertyChange(description, undo, redo));
		_redoStack.Clear();

		if (hadAnyRedo || !hadAnyUndo)
		{
			// Only trigger state changed if we went from no changes to having changes,
			// or if we had redo actions that are now cleared.
			StateChanged?.Invoke();
		}
	}

	public void Clear()
	{
		_undoStack.Clear();
		_redoStack.Clear();
		StateChanged?.Invoke();
	}
}
