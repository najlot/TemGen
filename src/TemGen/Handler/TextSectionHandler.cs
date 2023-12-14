using System.Threading.Tasks;

namespace TemGen.Handler;

public sealed class TextSectionHandler : AbstractSectionHandler
{
	public override async Task Handle(Globals globals, TemplateSection section)
	{
		if (section.Handler != TemplateHandler.Text)
		{
			await Next.Handle(globals, section).ConfigureAwait(false);
			return;
		}

		globals.Write(section.Content);
	}
}