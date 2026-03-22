namespace Todo.Service.Services;

public interface IPermissionQueryFilter
{
	IQueryable<T> ApplyReadFilter<T>(IQueryable<T> query);
}
