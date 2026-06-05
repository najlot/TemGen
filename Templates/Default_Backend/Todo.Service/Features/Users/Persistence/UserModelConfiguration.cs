using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using <# Project.Namespace#>.Service.Features.Users;

namespace <# Project.Namespace#>.Service.Features.Users.Persistence;

public class UserModelConfiguration : IEntityTypeConfiguration<UserModel>
{
	public void Configure(EntityTypeBuilder<UserModel> entity)
	{
		entity.HasKey(e => e.Id);
		entity.HasIndex(e => e.Username).IsUnique(false);
		entity.HasIndex(e => e.EMail).IsUnique(false);
		entity.HasIndex(e => e.DeletedAt).IsUnique(false);
	}
}
<#cs SetOutputPathAndSkipOtherDefinitions()#>