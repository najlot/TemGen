namespace Todo.Client.MVVM;

public interface IPropertyChangeVisitor
{
	bool IsApplyingChange { get; }
	void Visit<T>(object target, string propertyName, T oldValue, T newValue);
}
