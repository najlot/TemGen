<#cs Write(AddReadmeSection(
	PreviousContent,
	$"""
## Avalonia client
Requires `Default_Backend`, `Default_DataAccess`, and `Default_MVVM`.

Projects:
- `{Project.Namespace}.Avalonia`
- `{Project.Namespace}.Avalonia.Android`
- `{Project.Namespace}.Avalonia.Browser`
- `{Project.Namespace}.Avalonia.Desktop`
- `{Project.Namespace}.Avalonia.iOS`
"""));
SetOutputPathAndSkipOtherDefinitions(); return SkipRemaining();#>
