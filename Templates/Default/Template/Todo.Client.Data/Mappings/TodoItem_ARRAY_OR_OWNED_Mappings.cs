using Najlot.Map;
using Najlot.Map.Attributes;
using <#cs Write(Project.Namespace)#>.Client.Data.Models;
using <#cs Write(Project.Namespace)#>.Contracts;

namespace <#cs Write(Project.Namespace)#>.Client.Data.Mappings;

[Mapping]
internal sealed partial class <#cs Write(Definition.Name)#>Mappings
{
	[MapIgnoreProperty(nameof(to.Id))] // Do not map the Id property as it makes problems with entity tracking in EF Core
	public static partial void Map(IMap map, <#cs Write(Definition.Name)#> from, <#cs Write(Definition.Name)#> to);
}<#cs
SetOutputPath(!(Definition.IsOwnedType || Definition.IsArray));
RelativePath = RelativePath.Replace("_ARRAY_OR_OWNED_", "");
#>