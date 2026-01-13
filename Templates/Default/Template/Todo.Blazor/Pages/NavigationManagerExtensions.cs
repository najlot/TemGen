using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.WebUtilities;

namespace <#cs Write(Project.Namespace)#>.Blazor.Pages;

public static class NavigationManagerExtensions
{
	public static bool TryGetQueryString<T>(this NavigationManager navManager, string key, out T? value)
	{
		var uri = navManager.ToAbsoluteUri(navManager.Uri);

		if (QueryHelpers.ParseQuery(uri.Query).TryGetValue(key, out var valueFromQueryString))
		{
			if (typeof(T) == typeof(int) && int.TryParse(valueFromQueryString, out var valueAsInt))
			{
				value = (T)(object)valueAsInt;
				return true;
			}

			if (typeof(T) == typeof(string))
			{
				value = (T)(object)valueFromQueryString.ToString();
				return true;
			}

			if (typeof(T) == typeof(decimal) && decimal.TryParse(valueFromQueryString, out var valueAsDecimal))
			{
				value = (T)(object)valueAsDecimal;
				return true;
			}
		}

		value = default;
		return false;
	}

	public static bool TryGetReturnUrl(this NavigationManager navManager, out string? value)
	{
		return navManager.TryGetQueryString("ReturnUrl", out value);
	}

	public static string BuildReturnUrl(this NavigationManager navManager, string relativeTo)
	{
		var uri = navManager.ToBaseRelativePath(navManager.Uri);
		return QueryHelpers.AddQueryString(relativeTo, "ReturnUrl", uri);
	}
}<#cs SetOutputPathAndSkipOtherDefinitions()#>