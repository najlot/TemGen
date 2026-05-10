using System.Windows.Controls;
using Todo.Wpf.Controls;

namespace Todo.Wpf.Views;

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