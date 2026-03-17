using System;
using System.Threading.Tasks;

namespace <# Project.Namespace#>.Client.Data.Identity;

public interface IRegistrationService
{
	Task<RegistrationResult> Register(Guid id, string username, string email, string password);
}<#cs SetOutputPathAndSkipOtherDefinitions()#>