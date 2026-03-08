using System.Threading.Tasks;

namespace <#cs Write(Project.Namespace)#>.Client.Data;

public delegate Task AsyncEventHandler<T>(object sender, T args);
<#cs SetOutputPathAndSkipOtherDefinitions()#>