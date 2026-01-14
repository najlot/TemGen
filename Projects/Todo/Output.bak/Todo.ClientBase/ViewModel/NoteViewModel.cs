using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Najlot.Map;
using Todo.Client.Data.Models;
using Todo.Client.Data.Services;
using Todo.Client.MVVM;
using Todo.Client.MVVM.Services;
using Todo.Client.MVVM.Validation;
using Todo.Client.MVVM.ViewModel;
using Todo.ClientBase.Validation;
using Todo.Contracts;
using Todo.Contracts.Events;

namespace Todo.ClientBase.ViewModel;

public class NoteViewModel : AbstractValidationViewModel, IDisposable
{
	private readonly IErrorService _errorService;
	private readonly INavigationService _navigationService;
	private readonly INoteService _noteService;
	private readonly IMessenger _messenger;
	private readonly IMap _map;

	public List<PredefinedColor> AvailablePredefinedColors { get; } = new(Enum.GetValues(typeof(PredefinedColor)) as PredefinedColor[]);

	public Guid Id { get => field; set => Set(ref field, value); }
	public string Title { get => field; set => Set(ref field, value); }
	public string Content { get => field; set => Set(ref field, value); }
	public PredefinedColor Color { get => field; set => Set(ref field, value); }

	public bool IsBusy { get => field; private set => Set(ref field, value); }

	public bool IsNew { get; set; }

	public NoteViewModel(
		IErrorService errorService,
		INavigationService navigationService,
		INoteService noteService,
		IMessenger messenger,
		IMap map)
	{
		_errorService = errorService;
		_navigationService = navigationService;
		_noteService = noteService;
		_messenger = messenger;
		_map = map;

		SaveCommand = new AsyncCommand(SaveAsync, DisplayError);
		DeleteCommand = new AsyncCommand(DeleteAsync, DisplayError);

		SetValidation(new NoteValidationList());

		_messenger.Register<NoteUpdated>(Handle);
	}

	private async Task DisplayError(Task task)
	{
		await _errorService.ShowAlertAsync("Error...", task.Exception);
	}

	public void Handle(NoteUpdated obj)
	{
		if (Id != obj.Id)
		{
			return;
		}

		_map.From(obj).To(this);
	}

	public AsyncCommand SaveCommand { get; }
	public async Task SaveAsync()
	{
		if (IsBusy)
		{
			return;
		}

		try
		{
			IsBusy = true;

			var errors = Errors
				.Where(err => err.Severity > ValidationSeverity.Info)
				.Select(e => e.Text);

			if (errors.Any())
			{
				var message = "There are some validation errors:";
				message += Environment.NewLine + Environment.NewLine;
				message += string.Join(Environment.NewLine, errors);
				message += Environment.NewLine + Environment.NewLine;
				message += "Do you want to continue?";

				var vm = new YesNoPageViewModel()
				{
					Title = "Validation",
					Message = message
				};

				var selection = await _navigationService.RequestInputAsync(vm);

				if (!selection)
				{
					return;
				}
			}

			await _navigationService.NavigateBack();

			var model = _map.From(this).To<NoteModel>();

			if (IsNew)
			{
				await _noteService.AddItemAsync(model);
				IsNew = false;
			}
			else
			{
				await _noteService.UpdateItemAsync(model);
			}
		}
		catch (Exception ex)
		{
			await _errorService.ShowAlertAsync("Error saving...", ex);
		}
		finally
		{
			IsBusy = false;
		}
	}

	public AsyncCommand DeleteCommand { get; }
	public async Task DeleteAsync()
	{
		if (IsBusy)
		{
			return;
		}

		try
		{
			IsBusy = true;

			var vm = new YesNoPageViewModel()
			{
				Title = "Delete?",
				Message = "Should the item be deleted?"
			};

			var selection = await _navigationService.RequestInputAsync(vm);

			if (selection)
			{
				await _navigationService.NavigateBack();
				await _noteService.DeleteItemAsync(Id);
			}
		}
		catch (Exception ex)
		{
			await _errorService.ShowAlertAsync("Error deleting...", ex);
		}
		finally
		{
			IsBusy = false;
		}
	}

	public void Dispose() => _messenger.Unregister(this);
}