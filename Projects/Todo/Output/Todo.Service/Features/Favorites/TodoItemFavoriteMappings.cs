using Najlot.Map;
using Najlot.Map.Attributes;
using System.Linq.Expressions;
using Todo.Contracts.Shared;
using Todo.Service.Features.TodoItems;

namespace Todo.Service.Features.Favorites;

[Mapping]
internal partial class TodoItemFavoriteMappings
{
	public static Expression<Func<TodoItemModel, FavoriteModel>> GetFavoriteModelExpression()
	{
		return from => new FavoriteModel
		{
			TargetType = ItemType.TodoItem,
			ItemId = from.Id,
			Title = from.Title,
			Content = from.Content,
		};
	}

	[MapIgnoreProperty(nameof(to.Id))]
	[MapIgnoreProperty(nameof(to.UserId))]
	public static void MapToFavoriteModel(IMap map, TodoItemModel from, FavoriteModel to)
	{
		to.TargetType = ItemType.TodoItem;
		to.ItemId = from.Id;
		to.Title = from.Title;
		to.Content = from.Content;
	}
}
