using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace <# Project.Namespace#>.Service.Features.History.Persistence;

public class HistoryEntryConfiguration : IEntityTypeConfiguration<HistoryModel>
{
	public void Configure(EntityTypeBuilder<HistoryModel> entity)
	{
		entity.HasKey(item => item.Id);
		entity.HasIndex(item => item.EntityId).IsUnique(false);
		entity.HasIndex(item => item.TimeStamp).IsUnique(false);
		entity.Property(item => item.Changes).HasColumnType("longtext");
	}
}
<#cs SetOutputPathAndSkipOtherDefinitions()#>