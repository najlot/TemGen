using Avalonia.Controls;
using Todo.Avalonia.Controls;

namespace Todo.Avalonia.Views;

public partial class TodoItemView : UserControl
{
	public TodoItemView()
	{
		InitializeComponent();
	}

	public TodoItemView(ToggleFavoriteButton toggleFavoriteButton) : this()
	{
		ToggleFavoriteButtonHost.Content = toggleFavoriteButton;
	}
}
