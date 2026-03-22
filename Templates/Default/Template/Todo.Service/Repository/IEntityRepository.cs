using System;
using System.Linq;
using System.Threading.Tasks;
using <# Project.Namespace#>.Service.Model;

namespace <# Project.Namespace#>.Service.Repository;

public interface IEntityRepository<TModel> where TModel : class, IEntityModel
{
	IQueryable<TModel> GetAllQueryable();

	Task<TModel?> Get(Guid id);

	Task Insert(TModel model);

	Task Update(TModel model);

	Task Delete(Guid id);
}<#cs SetOutputPathAndSkipOtherDefinitions()#>