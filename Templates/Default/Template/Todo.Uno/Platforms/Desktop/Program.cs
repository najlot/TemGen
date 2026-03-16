using Uno.UI.Hosting;

namespace <#cs Write(Project.Namespace)#>.Uno;

internal class Program
{
    [STAThread]
    public static void Main(string[] args)
    {
        var host = UnoPlatformHostBuilder.Create()
            .App(() => new App())
            .UseX11()
            .UseLinuxFrameBuffer()
            .UseMacOS()
            .UseWin32()
            .Build();

        host.Run();
    }
}
<#cs SetOutputPathAndSkipOtherDefinitions()#>
