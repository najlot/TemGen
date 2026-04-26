using MongoDB.Bson.Serialization.Attributes;
using <# Project.Namespace#>.Service.Infrastructure.Persistence;

namespace <# Project.Namespace#>.Service.Features.Users;

[BsonIgnoreExtraElements]
public class UserModel : IEntityModel
{
	[BsonId]
	public Guid Id { get; set; }
	public string Username { get; set; } = string.Empty;
	public string EMail { get; set; } = string.Empty;
	public byte[] PasswordHash { get; set; } = [];
	public byte[]? PasswordResetCodeHash { get; set; }
	public DateTime? PasswordResetCodeExpiresAt { get; set; }
	public DateTime? DeletedAt { get; set; }
}<#cs SetOutputPathAndSkipOtherDefinitions()#>