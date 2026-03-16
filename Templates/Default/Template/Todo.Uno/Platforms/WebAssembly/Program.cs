using Uno.UI.Hosting;

namespace <#cs Write(Project.Namespace)#>.Uno;

public class Program
{
    public static async Task Main(string[] args)
    {
        var host = UnoPlatformHostBuilder.Create()
            .App(() => new App())
            .UseWebAssembly()
            .Build();

        await host.RunAsync();
    }
}
<#cs SetOutputPathAndSkipOtherDefinitions()#>
