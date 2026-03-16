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
		switch (obj)
		{
			case string str:
				_resultSb.Append(str);
				break;
			case not null:
				_resultSb.Append(obj.ToString());
				break;
		}
	}

	public void WriteLine(object obj)
	{
		switch (obj)
		{
			case string str:
				_resultSb.AppendLine(str);
				break;
			case null:
				_resultSb.AppendLine();
				break;
			default:
				_resultSb.AppendLine(obj.ToString());
				break;
		}
	}

	private readonly Dictionary<string, object> _variables = [];

	public void SetVariable(string name, object value)
	{
		_variables[name] = value;
	}

	public object GetVariable(string name)
	{
		_variables.TryGetValue(name, out var value);
		return value;
	}

	internal void ReplaceInResult(string from, string to)
	{
		_resultSb.Replace(from, to);
	}
}