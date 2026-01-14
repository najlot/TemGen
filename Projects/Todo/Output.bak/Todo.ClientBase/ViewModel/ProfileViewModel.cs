using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Todo.Client.MVVM;
using Todo.Client.MVVM.Services;
using Todo.Client.MVVM.ViewModel;
using Todo.Client.Localisation;
using Todo.ClientBase.Models;

namespace Todo.ClientBase.ViewModel;

public class ProfileViewModel : AbstractViewModel
{
	private ProfileBase profile;
	private readonly IErrorService _errorService;
	private readonly INavigationService _navigationService;

	public ProfileBase Profile { get => profile; set => Set(ref profile, value); }

	public Source Source
	{
		get => profile.Source;
		set
		{
			if (profile.Source != value)
			{
				ProfileBase newProfile = value switch
				{
					Source.Local => new LocalProfile(),
					Source.RMQ => new RmqProfile(),
					Source.REST => new RestProfile(),
					_ => throw new NotImplementedException(value.ToString()),
				};

				newProfile.Id = Profile.Id;
				newProfile.Name = Profile.Name;
				newProfile.Source = value;

				Profile = newProfile;
			}

			profile.Source = value;
		}
	}

	public List<Source> PossibleSources { get; } = new(Enum.GetValues(typeof(Source)) as Source[]);

	public ProfileViewModel(IErrorService errorService, INavigationService navigationService)
	{
		_errorService = errorService;
		_navigationService = navigationService;

		SaveCommand = new AsyncCommand(RequestSave, DisplayError);

		var id = Guid.NewGuid();

		Profile = new LocalProfile
		{
			Id = id,
			Name = ProfileLoc.NewProfile,
			FolderName = id.ToString()
		};
	}

	private async Task DisplayError(Task task)
	{
		await _errorService.ShowAlertAsync(CommonLoc.Error, task.Exception);
	}

	private readonly List<Func<ProfileViewModel, Task>> _onSaveRequested = [];
	public void OnSaveRequested(Func<ProfileViewModel, Task> func) => _onSaveRequested.Add(func);

	public AsyncCommand SaveCommand { get; }
	private async Task RequestSave()
	{
		foreach (var func in _onSaveRequested)
		{
			await func(this);
		}

		await _navigationService.NavigateBack();
	}
}