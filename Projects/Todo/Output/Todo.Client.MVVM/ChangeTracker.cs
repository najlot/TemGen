using System;
using System.Collections.Generic;

namespace Todo.Client.MVVM;

public record PropertyChange(string PropertyName, Action Undo, Action Redo);

public class ChangeTracker : IPropertyChangeVisitor
{
	private readonly Stack<PropertyChange> _undoStack = new();
	private readonly Stack<PropertyChange> _redoStack = new();

	public bool IsApplyingChange { get; private set; }
	public bool CanUndo => _undoStack.Count > 0;
	public bool CanRedo => _redoStack.Count > 0;

	public event Action? StateChanged;

	public void Visit<T>(object target, string propertyName, T oldValue, T newValue)
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

		_undoStack.Push(new PropertyChange(
			propertyName,
			() => property.SetValue(target, oldValue),
			() => property.SetValue(target, newValue)));

		_redoStack.Clear();
		StateChanged?.Invoke();
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

		_undoStack.Push(new PropertyChange(description, undo, redo));
		_redoStack.Clear();
		StateChanged?.Invoke();
	}

	public void Clear()
	{
		_undoStack.Clear();
		_redoStack.Clear();
		StateChanged?.Invoke();
	}
}
