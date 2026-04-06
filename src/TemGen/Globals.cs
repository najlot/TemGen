using System;
using System.Collections.Generic;
using System.Linq;
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
	public Encoding Encoding { get; set; } = Encoding.UTF8;

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

	private readonly List<Dictionary<string, object>> _variableScopes = [[]];

	public void SetVariable(string name, object value)
	{
		_variableScopes[^1][name] = value;
	}

	public object GetVariable(string name)
	{
		for (var index = _variableScopes.Count - 1; index >= 0; index--)
		{
			if (_variableScopes[index].TryGetValue(name, out var value))
			{
				return value;
			}
		}

		return null;
	}

	public IReadOnlyList<string> GetVisibleVariableNames()
	{
		var names = new HashSet<string>(StringComparer.Ordinal);

		foreach (var scope in _variableScopes)
		{
			foreach (var key in scope.Keys)
			{
				names.Add(key);
			}
		}

		return names.OrderBy(name => name, StringComparer.Ordinal).ToArray();
	}

	public void PushVariableScope()
	{
		_variableScopes.Add([]);
	}

	public void PopVariableScope()
	{
		if (_variableScopes.Count == 1)
		{
			throw new InvalidOperationException("Cannot remove the root variable scope.");
		}

		_variableScopes.RemoveAt(_variableScopes.Count - 1);
	}

	internal void ReplaceInResult(string from, string to)
	{
		_resultSb.Replace(from, to);
	}
}