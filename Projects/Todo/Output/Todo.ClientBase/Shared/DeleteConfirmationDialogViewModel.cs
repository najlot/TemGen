using System.Threading.Tasks;
using Todo.Client.Localisation;
using Todo.Client.MVVM;

namespace Todo.ClientBase.Shared;

public enum DeleteDialogResult
{
	Delete,
	Cancel
}

public sealed class DeleteConfirmationDialogViewModel : AbstractViewModel
{
	public string Title { get; set => Set(ref field, value); } = CommonLoc.DeleteConfirmationTitle;
	public string Description { get; set => Set(ref field, value); } = CommonLoc.DeleteConfirmationDescription;

	public bool IsVisible { get; private set => Set(ref field, value); }

	private TaskCompletionSource<DeleteDialogResult> _deleteDialogCompletion = new();

	public async Task<DeleteDialogResult> ShouldDelete()
	{
		_deleteDialogCompletion = new();

		IsVisible = true;

		try
		{
			return await _deleteDialogCompletion.Task.ConfigureAwait(false);
		}
		finally
		{
			IsVisible = false;
		}
	}

	public DeleteConfirmationDialogViewModel()
	{
		DeleteCommand = new RelayCommand(Delete);
		CancelCommand = new RelayCommand(Cancel);
	}

	public RelayCommand DeleteCommand { get; }
	private void Delete() => _deleteDialogCompletion.SetResult(DeleteDialogResult.Delete);

	public RelayCommand CancelCommand { get; }
	private void Cancel() => _deleteDialogCompletion.SetResult(DeleteDialogResult.Cancel);
}
