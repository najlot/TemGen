namespace TemGen.Services;

public enum TemplateHandler
{
	Text,
	Script,
	For,
	If,
	ElseIf,
	Else,
	End,
	Reflection,
	JavaScript,
	Lua,
	Python,
}

public enum TemplateLanguage
{
	CSharp,
	JavaScript,
	Lua,
	Python,
}