using System.Threading.Tasks;

namespace TemGen.Handler;

public sealed class TextSectionHandler() : AbstractSectionHandler(TemplateHandler.Text)
{
	protected override Task Handle(Globals globals, string content)
	{
		globals.Write(content);
		return Task.CompletedTask;
	}
}