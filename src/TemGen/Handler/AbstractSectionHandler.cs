using System.Threading.Tasks;
using TemGen.Models;
using TemGen.Services;

namespace TemGen.Handler;

public abstract class AbstractSectionHandler(TemplateHandler handler, TemplateLanguage? language = null)
{
	public async Task<bool> TryHandle(Globals globals, TemplateSection section)
	{
		if (section.Handler == handler && section.Language == language)
		{
			await Handle(globals, section.Content).ConfigureAwait(false);
			return true;
		}

		return false;
	}

	protected abstract Task Handle(Globals globals, string content);
}