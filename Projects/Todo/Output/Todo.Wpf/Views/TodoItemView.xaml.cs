using System.Windows.Controls;
using Todo.Wpf.Controls;

namespace Todo.Wpf.Views;

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