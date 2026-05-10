using System;
using System.Threading.Tasks;
using Todo.Client.Data.Users;
using Todo.Client.Localisation;

namespace Todo.Client.Data.History;

internal sealed class TodoItemHistoryLocalizer(
	IUserService userService) : EntityHistoryLocalizerBase
{
	public override bool CanLocalize(string entityName)
		=> string.Equals(entityName, "TodoItem", StringComparison.OrdinalIgnoreCase);

	protected override string[] LocalizePropertyParts(string[] parts)
	{
		if (parts.Length == 1)
		{
			return parts[0] switch
			{
				"Title" => [TodoItemLoc.Title],
				"Content" => [TodoItemLoc.Content],
				"CreatedAt" => [TodoItemLoc.CreatedAt],
				"CreatedBy" => [TodoItemLoc.CreatedBy],
				"AssignedToId" => [TodoItemLoc.AssignedTo],
				"Status" => [TodoItemLoc.Status],
				"ChangedAt" => [TodoItemLoc.ChangedAt],
				"ChangedBy" => [TodoItemLoc.ChangedBy],
				"Priority" => [TodoItemLoc.Priority],
				"Checklist" => [TodoItemLoc.Checklist],
				"IsDeleted" => [CommonLoc.Deleted],
				_ => parts,
			};
		}
		else if (parts.Length == 2 && parts[0].Equals("Checklist", StringComparison.OrdinalIgnoreCase))
		{
			return parts[1] switch
			{
				"IsDone" => [TodoItemLoc.Checklist, ChecklistTaskLoc.IsDone],
				"Description" => [TodoItemLoc.Checklist, ChecklistTaskLoc.Description],
				_ => parts,
			};
		}

		return parts;
	}

	protected override async Task<string> LocalizePropertyValue(string[] parts, string value)
	{
		if (parts.Length == 1)
		{
			return parts[0] switch
			{
				"AssignedToId" => await LocalizeUserId(value).ConfigureAwait(false),
				"Status" => LocalizeTodoItemStatus(value),
				"IsDeleted" => LocalizeBoolean(value),
				_ => value,
			};
		}
		else if (parts.Length == 2 && parts[0].Equals("Checklist", StringComparison.OrdinalIgnoreCase))
		{
			return parts[1] switch
			{
				"IsDone" => LocalizeBoolean(value),
				_ => value,
			};
		}

		return value;
	}

	private async Task<string> LocalizeUserId(string value)
	{
		if (!Guid.TryParse(value, out var id))
		{
			return value;
		}
		else if (id == Guid.Empty)
		{
			return string.Empty;
		}

		try
		{
			var user = await userService.GetItemAsync(id).ConfigureAwait(false);
			return user?.Username ?? value;
		}
		catch (Exception)
		{
			return value;
		}
	}

	private static string LocalizeTodoItemStatus(string value)
		=> value.ToLowerInvariant() switch
		{
			"todo" => TodoItemStatusLoc.Todo,
			"inprogress" => TodoItemStatusLoc.InProgress,
			"done" => TodoItemStatusLoc.Done,
			_ => value,
		};
}
