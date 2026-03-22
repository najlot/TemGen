using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using <# Project.Namespace#>.Service.Model;

namespace <# Project.Namespace#>.Service.Repository.MySqlImpl.Configurations;

public class <# Definition.Name#>ModelConfiguration : IEntityTypeConfiguration<<# Definition.Name#>Model>
{
	public void Configure(EntityTypeBuilder<<# Definition.Name#>Model> entity)
	{
		entity.HasKey(e => e.Id);
		entity.HasIndex(e => e.DeletedAt).IsUnique(false);
<#for e in Entries
#><#if e.IsArray
#>		entity.OwnsMany(e => e.<# e.Field#>, owned =>
		{
			owned.HasKey(e => e.Id);
			owned.ToTable("<# Definition.Name#>_<# e.Field#>");
		});
<#elseif e.IsOwnedType
#>		entity.OwnsOne(e => e.<# e.Field#>).ToTable("<# Definition.Name#>_<# e.Field#>");
<#end#><#end#>	}
}
<#cs SetOutputPath(Definition.IsEnumeration || Definition.IsArray || Definition.IsOwnedType);#>