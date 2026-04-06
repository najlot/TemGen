using System;
using System.Linq;
using System.Threading.Tasks;
using Todo.Service.Infrastructure.Persistence;

namespace Todo.Service.Features.Notes;

public interface INoteRepository : IEntityRepository<NoteModel>
{
}