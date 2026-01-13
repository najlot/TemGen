using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Todo.Service.Model;

namespace Todo.Service.Repository;

public interface INoteRepository
{
	IAsyncEnumerable<NoteModel> GetAll();

	IQueryable<NoteModel> GetAllQueryable();

	Task<NoteModel?> Get(Guid id);

	Task Insert(NoteModel model);

	Task Update(NoteModel model);

	Task Delete(Guid id);
}