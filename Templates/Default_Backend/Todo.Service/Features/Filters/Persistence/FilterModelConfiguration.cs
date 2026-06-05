using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace <# Project.Namespace#>.Service.Features.Filters.Persistence;

public sealed class FilterModelConfiguration : IEntityTypeConfiguration<FilterModel>
{
	public void Configure(EntityTypeBuilder<FilterModel> entity)
	{
		entity.HasKey(e => e.Id);
		entity.Property(e => e.Name).HasMaxLength(255);
		entity.HasIndex(e => e.UserId).IsUnique(false);
		entity.HasIndex(e => e.TargetType).IsUnique(false);

		entity.OwnsMany(e => e.Conditions, owned =>
		{
			owned.WithOwner().HasForeignKey("FilterId");
			owned.Property<int>("Id");
			owned.HasKey("Id");
			owned.Property(condition => condition.Field).HasMaxLength(255);
			owned.Property(condition => condition.Value).HasColumnType("longtext");
			owned.ToTable("FilterConditions");
		});
	}
}
<#cs SetOutputPathAndSkipOtherDefinitions()#>