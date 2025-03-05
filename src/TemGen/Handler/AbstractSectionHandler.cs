using System.Threading.Tasks;

namespace TemGen.Handler;

public abstract class AbstractSectionHandler(TemplateHandler handler)
{
	public async Task<bool> TryHandle(Globals globals, TemplateSection section)
	{
		if (section.Handler == handler)
		{
			await Handle(globals, section.Content).ConfigureAwait(false);
			return true;
		}

		return false;
	}

	protected abstract Task Handle(Globals globals, string content);
}