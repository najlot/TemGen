using System.Windows.Controls;
using <# Project.Namespace#>.Wpf.Controls;

namespace <# Project.Namespace#>.Wpf.Views;

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
}<#cs SetOutputPath(Definition.IsEnumeration || Definition.IsOwnedType || Definition.IsArray)#>