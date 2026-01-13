using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Todo.Service.Model;

namespace Todo.Service.Repository;

public interface ITodoItemRepository
{
	IAsyncEnumerable<TodoItemModel> GetAll();

	IQueryable<TodoItemModel> GetAllQueryable();

	Task<TodoItemModel?> Get(Guid id);

	Task Insert(TodoItemModel model);

	Task Update(TodoItemModel model);

	Task Delete(Guid id);
}