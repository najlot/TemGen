using System.Threading.Tasks;

namespace TemGen.Handler;

public sealed class TextSectionHandler : AbstractSectionHandler
{
	public override async Task<HandlingResult> Handle(TemplateSection section, Definition definition, string relativePath, DefinitionEntry definitionEntry)
	{
		if (section.Handler != TemplateHandler.Text)
		{
			return await Next.Handle(section, definition, relativePath, definitionEntry).ConfigureAwait(false);
		}

		return new HandlingResult()
		{
			RelativePath = relativePath,
			Content = section.Content,
		};
	}
}