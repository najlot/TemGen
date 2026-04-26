using MongoDB.Bson.Serialization.Attributes;
using Todo.Service.Infrastructure.Persistence;

namespace Todo.Service.Features.Users;

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
}