using System.Threading.Tasks;

namespace TemGen.Handler;

public abstract class AbstractSectionHandler
{
	protected AbstractSectionHandler Next = null;

	public AbstractSectionHandler SetNext(AbstractSectionHandler next)
	{
		if (Next is null)
		{
			Next = next;
		}
		else
		{
			Next.SetNext(next);
		}

		return this;
	}

	public abstract Task Handle(Globals globals, TemplateSection section);
}