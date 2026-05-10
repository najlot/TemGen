using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace <# Project.Namespace#>.Service.Features.Favorites.Persistence;

public sealed class FavoriteModelConfiguration : IEntityTypeConfiguration<FavoriteModel>
{
	public void Configure(EntityTypeBuilder<FavoriteModel> entity)
	{
		entity.HasKey(e => e.Id);
		entity.Property(e => e.Title).HasMaxLength(255);
		entity.Property(e => e.Content).HasColumnType("longtext");
		entity.HasIndex(e => e.UserId).IsUnique(false);
		entity.HasIndex(e => e.TargetType).IsUnique(false);
		entity.HasIndex(e => e.ItemId).IsUnique(false);
		entity.HasIndex(e => new { e.UserId, e.TargetType, e.ItemId }).IsUnique();
	}
}
<#cs SetOutputPathAndSkipOtherDefinitions()#>