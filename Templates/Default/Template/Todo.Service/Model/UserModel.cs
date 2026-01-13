using MongoDB.Bson.Serialization.Attributes;
using System;

namespace <#cs Write(Project.Namespace)#>.Service.Model;

[BsonIgnoreExtraElements]
public class UserModel
{
	[BsonId]
	public Guid Id { get; set; }
	public string Username { get; set; } = string.Empty;
	public string EMail { get; set; } = string.Empty;
	public byte[] PasswordHash { get; set; } = [];
	public bool IsActive { get; set; }
}<#cs SetOutputPathAndSkipOtherDefinitions()#>