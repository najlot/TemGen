using Avalonia.Controls;
using <# Project.Namespace#>.Avalonia.Controls;

namespace <# Project.Namespace#>.Avalonia.Views;

public partial class <# Definition.Name#>View : UserControl
{
	public <# Definition.Name#>View()
	{
		InitializeComponent();
	}

	public <# Definition.Name#>View(ToggleFavoriteButton toggleFavoriteButton) : this()
	{
		ToggleFavoriteButtonHost.Content = toggleFavoriteButton;
	}
}
<#cs SetOutputPath(Definition.IsEnumeration || Definition.IsOwnedType || Definition.IsArray)#>