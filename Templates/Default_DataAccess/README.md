<#cs Write(AddReadmeSection(
	PreviousContent,
	$"""
## Data access layer
Requires `Default_Backend`.

Projects:
- `{Project.Namespace}.Client.Data`
- `{Project.Namespace}.Client.Data.Test`
- `{Project.Namespace}.Client.Localisation`
"""));
SetOutputPathAndSkipOtherDefinitions(); return SkipRemaining();#>
