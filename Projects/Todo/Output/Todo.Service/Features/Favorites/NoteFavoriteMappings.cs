using Najlot.Map;
using Najlot.Map.Attributes;
using System.Linq.Expressions;
using Todo.Contracts.Shared;
using Todo.Service.Features.Notes;

namespace Todo.Service.Features.Favorites;

[Mapping]
internal partial class NoteFavoriteMappings
{
	public static Expression<Func<NoteModel, FavoriteModel>> GetFavoriteModelExpression()
	{
		return from => new FavoriteModel
		{
			TargetType = ItemType.Note,
			ItemId = from.Id,
			Title = from.Title,
			Content = from.Content,
		};
	}

	[MapIgnoreProperty(nameof(to.Id))]
	[MapIgnoreProperty(nameof(to.UserId))]
	public static void MapToFavoriteModel(IMap map, NoteModel from, FavoriteModel to)
	{
		to.TargetType = ItemType.Note;
		to.ItemId = from.Id;
		to.Title = from.Title;
		to.Content = from.Content;
	}
}
