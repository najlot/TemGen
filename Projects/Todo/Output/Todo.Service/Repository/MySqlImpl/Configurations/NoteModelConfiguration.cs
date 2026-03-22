using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Todo.Service.Model;

namespace Todo.Service.Repository.MySqlImpl.Configurations;

public class NoteModelConfiguration : IEntityTypeConfiguration<NoteModel>
{
	public void Configure(EntityTypeBuilder<NoteModel> entity)
	{
		entity.HasKey(e => e.Id);
		entity.HasIndex(e => e.DeletedAt).IsUnique(false);
	}
}
