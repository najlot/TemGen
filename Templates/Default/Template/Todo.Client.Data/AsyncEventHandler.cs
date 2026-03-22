using System.Threading.Tasks;

namespace <# Project.Namespace#>.Client.Data;

public delegate Task AsyncEventHandler<T>(object sender, T args);
<#cs SetOutputPathAndSkipOtherDefinitions()#>