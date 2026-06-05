<#cs Write(AddReadmeSection(
	PreviousContent,
	$"""
## MVVM layer
Requires `Default_Backend` and `Default_DataAccess`.

Projects:
- `{Project.Namespace}.Client.MVVM`
- `{Project.Namespace}.ClientBase`
- `{Project.Namespace}.ClientBase.Test`
"""));
SetOutputPathAndSkipOtherDefinitions(); return SkipRemaining();#>
