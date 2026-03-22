using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Todo.Service.Model;

namespace Todo.Service.Repository.MySqlImpl.Configurations;

public class TodoItemModelConfiguration : IEntityTypeConfiguration<TodoItemModel>
{
	public void Configure(EntityTypeBuilder<TodoItemModel> entity)
	{
		entity.HasKey(e => e.Id);
		entity.HasIndex(e => e.DeletedAt).IsUnique(false);
		entity.OwnsMany(e => e.Checklist, owned =>
		{
			owned.HasKey(e => e.Id);
			owned.ToTable("TodoItem_Checklist");
		});
	}
}
