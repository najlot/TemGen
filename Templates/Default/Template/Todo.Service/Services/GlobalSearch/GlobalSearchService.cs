using <# Project.Namespace#>.Contracts;

namespace <# Project.Namespace#>.Service.Services.GlobalSearch;

public class GlobalSearchService(IEnumerable<IGlobalSearchSource> sources)
{
	public async Task<GlobalSearchItem[]> SearchAsync(string text)
	{
		if (string.IsNullOrWhiteSpace(text))
		{
			return [];
		}

		var trimmedText = text.Trim();
		var items = new List<GlobalSearchItem>();

		foreach (var source in sources)
		{
			var sourceItems = await source.SearchAsync(trimmedText).ToListAsync().ConfigureAwait(false);
			items.AddRange(sourceItems);
		}

		if (items.Count > 100)
		{
			// We limit to top 100 results based on relevance score to avoid overwhelming the user and to improve performance
			return items
				.OrderBy(r => GetRelevanceScore(r, trimmedText))
				.Take(100)
				.ToArray();
		}

		return items
			.OrderBy(r => GetRelevanceScore(r, trimmedText))
			.ToArray();
	}

	private static int GetRelevanceScore(GlobalSearchItem item, string searchText)
	{
		// Exact title match = highest relevance (0)
		if (item.Title.Equals(searchText, StringComparison.OrdinalIgnoreCase))
		{
			return 0;
		}

		// Title starts with search text = high relevance (1)
		if (item.Title.StartsWith(searchText, StringComparison.OrdinalIgnoreCase))
		{
			return 1;
		}

		// Title contains search text = medium relevance (2)
		if (item.Title.Contains(searchText, StringComparison.OrdinalIgnoreCase))
		{
			return 2;
		}

		// Content contains search text = lower relevance (3)
		if (!string.IsNullOrEmpty(item.Content) && item.Content.Contains(searchText, StringComparison.OrdinalIgnoreCase))
		{
			return 3;
		}

		// Something else contains search text = lowest relevance (4)
		return 4;
	}
}
<#cs SetOutputPathAndSkipOtherDefinitions()#>