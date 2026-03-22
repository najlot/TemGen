using MongoDB.Bson.Serialization.Attributes;

namespace <# Project.Namespace#>.Service.Model;

[BsonIgnoreExtraElements]
public class UserModel : IEntityModel
{
	[BsonId]
	public Guid Id { get; set; }
	public string Username { get; set; } = string.Empty;
	public string EMail { get; set; } = string.Empty;
	public byte[] PasswordHash { get; set; } = [];
	public DateTime? DeletedAt { get; set; }
}<#cs SetOutputPathAndSkipOtherDefinitions()#>