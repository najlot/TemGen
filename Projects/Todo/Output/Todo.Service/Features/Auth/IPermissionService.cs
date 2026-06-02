namespace Todo.Service.Features.Auth;

public interface IPermissionService
{
	IQueryable<T> ApplyReadFilter<T>(IQueryable<T> query);
	bool CanAccess<T>(T item);
}
