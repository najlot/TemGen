using MongoDB.Bson.Serialization.Attributes;
using Todo.Contracts;
using System;
using System.Collections.Generic;

namespace Todo.Service.Model;

[BsonIgnoreExtraElements]
public class NoteModel
{
	[BsonId]
	public Guid Id { get; set; }
	public string Title { get; set; } = string.Empty;
	public string Content { get; set; } = string.Empty;
	public PredefinedColor Color { get; set; }
}