using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using <#cs Write(Project.Namespace)#>.ClientBase.ProfileHandler;
using <#cs Write(Project.Namespace)#>.ClientBase.Services;
using System.Threading.Tasks;
using <#cs Write(Project.Namespace)#>.Client.Localisation;
using <#cs Write(Project.Namespace)#>.Client.MVVM;
using <#cs Write(Project.Namespace)#>.Client.MVVM.ViewModel;
using <#cs Write(Project.Namespace)#>.Client.MVVM.Services;

namespace <#cs Write(Project.Namespace)#>.ClientBase.ViewModel;

public class LoginViewModel : AbstractViewModel
{
	private readonly IErrorService _errorService;
	private readonly INavigationService _navigationService;
	private readonly IProfilesService _profilesService;
	private readonly IProfileHandler _profileHandler;
	private readonly IServiceProvider _serviceProvider;
	private readonly Func<ProfileViewModel> _createProfileViewModel;
	private readonly Func<LoginProfileViewModel> _createLoginProfileViewModel;
	private IServiceScope _serviceScope;
	private ObservableCollection<LoginProfileViewModel> _loginProfiles = [];

	public ObservableCollection<LoginProfileViewModel> LoginProfiles
	{
		get => _loginProfiles;
		private set => Set(nameof(LoginProfiles), ref _loginProfiles, value);
	}

	public LoginViewModel(IErrorService errorService,
		INavigationService navigationService,
		IProfilesService profilesService,
		IProfileHandler profileHandler,
		IServiceProvider serviceProvider,
		Func<ProfileViewModel> createProfileViewModel,
		Func<LoginProfileViewModel> createLoginProfileViewModel)
	{
		_errorService = errorService;
		_navigationService = navigationService;
		_profilesService = profilesService;
		_profileHandler = profileHandler;
		_serviceProvider = serviceProvider;
		_createProfileViewModel = createProfileViewModel;
		_createLoginProfileViewModel = createLoginProfileViewModel;

		CreateProfileCommand = new AsyncCommand(CreateProfileAsync, DisplayError);
		EditProfileCommand = new AsyncCommand<LoginProfileViewModel>(EditProfileAsync, DisplayError);
		DeleteProfileCommand = new AsyncCommand<LoginProfileViewModel>(DeleteProfileAsync, DisplayError);
		LoginProfileCommand = new AsyncCommand<LoginProfileViewModel>(LoginProfileAsync, DisplayError);

		LoadProfilesAsync().ContinueWith(DisplayError, TaskContinuationOptions.OnlyOnFaulted);
	}

	private async Task DisplayError(Task task)
	{
		await _errorService.ShowAlertAsync(CommonLoc.Error, task.Exception);
	}

	public AsyncCommand CreateProfileCommand { get; }

	private async Task CreateProfileAsync()
	{
		var viewModel = _createProfileViewModel();
		viewModel.OnSaveRequested(HandleSaveAsync);
		await _navigationService.NavigateForward(viewModel);
	}

	private async Task HandleSaveAsync(ProfileViewModel obj)
	{
		var profile = obj.Profile;
		var profileVm = LoginProfiles.FirstOrDefault(vm => vm.Profile.Id == profile.Id);

		if (profileVm != null)
		{
			LoginProfiles.Remove(profileVm);
		}

		profileVm = _createLoginProfileViewModel();
		profileVm.Profile = profile;

		LoginProfiles.Add(profileVm);

		await _profilesService.SaveAsync(LoginProfiles.Select(vm => vm.Profile).ToList());
	}

	private async Task LoadProfilesAsync()
	{
		var profiles = await _profilesService.LoadAsync();
		var loginProfiles = new ObservableCollection<LoginProfileViewModel>(
			profiles.Select(profile =>
			{
				var vm = _createLoginProfileViewModel();
				vm.Profile = profile;

				return vm;
			}));

		LoginProfiles = loginProfiles;
	}

	public AsyncCommand<LoginProfileViewModel> DeleteProfileCommand { get; }
	private async Task DeleteProfileAsync(LoginProfileViewModel obj)
	{
		var profile = obj.Profile;
		var profileVm = LoginProfiles.FirstOrDefault(viewModel => viewModel.Profile.Id == profile.Id);

		if (profileVm == null)
		{
			return;
		}

		var message = string.Format(ProfileLoc.DeleteProfile, profileVm.Profile.Name);

		var vm = new YesNoPageViewModel()
		{
			Title = ProfileLoc.DeleteQ,
			Message = message
		};

		var selection = await _navigationService.RequestInputAsync(vm);

		if (selection)
		{
			LoginProfiles.Remove(profileVm);
			await _profilesService.RemoveAsync(profile);
		}
	}

	public AsyncCommand<LoginProfileViewModel> EditProfileCommand { get; }
	private async Task EditProfileAsync(LoginProfileViewModel obj)
	{
		var profile = obj.Profile;
		var viewModel = _createProfileViewModel();
		viewModel.Profile = profile.Clone();
		viewModel.OnSaveRequested(HandleSaveAsync);
		await _navigationService.NavigateForward(viewModel);
	}

	public AsyncCommand<LoginProfileViewModel> LoginProfileCommand { get; }
	private async Task LoginProfileAsync(LoginProfileViewModel obj)
	{
		try
		{
			_serviceScope?.Dispose();
			_serviceScope = _serviceProvider.CreateScope();

			await _profileHandler.SetProfile(obj.Profile);
			var viewModel = _serviceScope.ServiceProvider.GetRequiredService<MenuViewModel>();
			await _navigationService.NavigateForward(viewModel);
		}
		catch (Exception ex)
		{
			await _errorService.ShowAlertAsync(ProfileLoc.CouldNotLogin, ex);
		}
	}
}<#cs SetOutputPathAndSkipOtherDefinitions()#>