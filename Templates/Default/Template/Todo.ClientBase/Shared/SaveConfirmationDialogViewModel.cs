using System.Threading.Tasks;
using <# Project.Namespace#>.Client.Localisation;
using <# Project.Namespace#>.Client.MVVM;

namespace <# Project.Namespace#>.ClientBase.Shared;

public enum SaveDialogResult
{
	Save,
	Discard,
	Cancel
}

public sealed class SaveConfirmationDialogViewModel : AbstractViewModel
{
	public string Title { get; set => Set(ref field, value); } = CommonLoc.SaveConfirmationTitle;
	public string Description { get; set => Set(ref field, value); } = CommonLoc.SaveConfirmationDescription;

	public bool IsVisible { get; private set => Set(ref field, value); }
	public bool CanSave { get; set => Set(ref field, value, SaveCommand.RaiseCanExecuteChanged); }

	private TaskCompletionSource<SaveDialogResult> _saveDialogCompletion = new();

	public async Task<SaveDialogResult> ShouldSave()
	{
		_saveDialogCompletion = new();

		IsVisible = true;

		try
		{
			return await _saveDialogCompletion.Task.ConfigureAwait(false);
		}
		finally
		{
			IsVisible = false;
		}
	}

	public SaveConfirmationDialogViewModel()
	{
		SaveCommand = new RelayCommand(Save, () => CanSave);
		DiscardCommand = new RelayCommand(Discard);
		CancelCommand = new RelayCommand(Cancel);
	}

	public RelayCommand SaveCommand { get; }
	private void Save() => _saveDialogCompletion.SetResult(SaveDialogResult.Save);

	public RelayCommand DiscardCommand { get; }
	private void Discard() => _saveDialogCompletion.SetResult(SaveDialogResult.Discard);

	public RelayCommand CancelCommand { get; }
	private void Cancel() => _saveDialogCompletion.SetResult(SaveDialogResult.Cancel);
}<#cs SetOutputPathAndSkipOtherDefinitions()#>