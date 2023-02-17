using System;
using System.Collections.Generic;
using System.Text;

namespace TemGen;

public class Globals
{
	private readonly StringBuilder _resultSb = new();

	public string Result
	{
		get => _resultSb.ToString();
		set
		{
			_resultSb.Clear();
			_resultSb.Append(value);
		}
	}

	public string RelativePath { get; set; }
	public Definition Definition { get; set; }
	public bool SkipOtherDefinitions { get; set; }
	public List<Definition> Definitions { get; set; }
	public DefinitionEntry DefinitionEntry { get; set; }
	public List<DefinitionEntry> Entries { get; set; }
	public Project Project { get; set; }
	public bool RepeatForEachDefinitionEntry { get; set; }

	public void Write(object obj)
	{
		if (obj is null)
		{
			return;
		}

		_resultSb.Append(obj.ToString());
	}

	public void WriteLine(object obj)
	{
		Write(obj);
		Write(Environment.NewLine);
	}
}