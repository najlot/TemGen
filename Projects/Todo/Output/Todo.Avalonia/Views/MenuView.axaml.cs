using Avalonia.Controls;
using Avalonia.Input;
using Todo.ClientBase.Shared;

namespace Todo.Avalonia.Views;

public partial class MenuView : UserControl
{
	public MenuView()
	{
		InitializeComponent();
	}

	private void OnOverlayPointerPressed(object? sender, PointerPressedEventArgs e)
	{
		if (DataContext is MenuViewModel viewModel && viewModel.ToggleDrawerCommand.CanExecute(null))
		{
			viewModel.ToggleDrawerCommand.Execute(null);
		}
	}
}
