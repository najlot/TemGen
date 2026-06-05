using System;
using System.Linq;
using System.Threading.Tasks;
using <# Project.Namespace#>.Service.Infrastructure.Persistence;

namespace <# Project.Namespace#>.Service.Features.<# Definition.Name#>s;

public interface I<# Definition.Name#>Repository : IEntityRepository<<# Definition.Name#>Model>
{
}<#cs SetOutputPath(Definition.IsOwnedType || Definition.IsEnumeration || Definition.IsArray)#>