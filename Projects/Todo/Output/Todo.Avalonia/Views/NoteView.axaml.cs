using Avalonia.Controls;
using Todo.Avalonia.Controls;

namespace Todo.Avalonia.Views;

public partial class NoteView : UserControl
{
	public NoteView()
	{
		InitializeComponent();
	}

	public NoteView(ToggleFavoriteButton toggleFavoriteButton) : this()
	{
		ToggleFavoriteButtonHost.Content = toggleFavoriteButton;
	}
}
