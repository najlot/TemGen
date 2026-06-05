# TemGen Project Instructions

## Project Overview

TemGen is a template-based code generator built with .NET 10.0. It processes templates with embedded scripts (C#, JavaScript, Python, Lua) to generate code based on definitions.

## Repository Structure

- **`./src`** - Source code for the project
  - `TemGen/` - Main CLI application (executable tool)
  - `TemGen.Tests/` - Unit tests
- **`./Templates/Default_*`** - Split default template layers used for code generation
  - `Default_Backend/` - Contracts, service, and service tests
  - `Default_DataAccess/` - Client data services, localisation, and client data tests
  - `Default_MVVM/` - Shared MVVM helpers and view models
  - `Default_WPF/` - WPF desktop client
  - `Default_Avalonia/` - Avalonia shared client plus platform heads
  - `Default_Blazor/` - Server-side Blazor client
  - `Default_HTMX/` - Razor Pages/HTMX client
  - `Default_Scripts/` - Shared C# helper scripts for the default layers
- **`./Projects/Todo`** - Default output project used for generation and testing
  - `Definitions/` - Definition files
  - `Output/` - Generated output from templates
  - `ProjectDefinition.json` - Project configuration file
  - `Resources/` - Resource files

## How It Works

The Todo project composes the split default template layers to generate `./Projects/Todo/Output` from its definitions. `Default_Backend` is required by every default application combination; client-facing layers depend on `Default_DataAccess`, and WPF/Avalonia also depend on `Default_MVVM`. The Todo project serves as both an example and a test case for the code generation functionality.

## Quick Reference

```bash
# Build
dotnet build src/TemGen.slnx

# Run tests
dotnet test src/TemGen.Tests/TemGen.Tests.csproj

# Run code generation for the Todo project
dotnet run --project src/TemGen/TemGen.csproj -- --path ./Projects/Todo

# Run the Tool Locally
dotnet run --project src/TemGen/TemGen.csproj -- --path ./Projects/Todo

# Verify templates produce no output changes
dotnet run --project src/TemGen/TemGen.csproj -- --path ./Projects/Todo && git diff --stat Projects/Todo/Output
```

## Working with Templates

### How TemGen Processes Templates

Each template file in the selected `Templates/Default_*` layer directories is processed **once per definition** (from `Projects/Todo/Definitions/`). `Projects/Todo/ProjectDefinition.json` lists layers in dependency order:

```text
Default_Backend;Default_DataAccess;Default_MVVM;Default_WPF;Default_Avalonia;Default_Blazor;Default_HTMX
```

Templates with the same relative output path can build on earlier layers through `PreviousContent`. The default root files `Directory.Packages.props`, `README.md`, and `Todo.slnx` use this multilayer behavior so each layer contributes only its own package versions, README section, and solution projects. The template controls its output via:

- **`RelativePath`**: Set to empty string to skip output for a definition. `SetOutputPath(bool skip)` helper: if skip is true, sets `RelativePath = ""` (no output); otherwise replaces `TodoItem` with `Definition.Name` and `Todo` with `Project.Namespace` in the path.
- **`SkipOtherDefinitions`**: Set to `true` to stop processing further definitions. `SetOutputPathAndSkipOtherDefinitions()` sets the output path and stops after the first definition (used for files generated once, like converters, shared base classes).
- **`SetOutputPath(bool skip)`**: Common helper. Called at the END of per-definition templates (e.g., `<#cs SetOutputPath(Definition.IsOwnedType || Definition.IsEnumeration || Definition.IsArray)#>`).

### Template Code Blocks

- `<#cs ... #>` - Embedded C# code block (executes, text output via `Write()`/`WriteLine()`)
- `<#cs Write(expr) #>` - Inline expression output
- `Result` - The accumulated text output; can be modified (e.g., `Result = Result.TrimEnd()`)

### Definition Properties

Each **Definition** has:
- `Name` / `NameLow` - Definition name (e.g., "TodoItem" / "todoItem")
- `IsOwnedType` - Directly embedded child type (not referenced by ID)
- `IsEnumeration` - Enum type (e.g., PredefinedColor, TodoItemStatus)
- `IsArray` - Referenced as an array in another type (e.g., ChecklistTask is `IsArray=true` because TodoItem has `ChecklistTask[]`)
- `Entries` - List of fields/properties

Each **DefinitionEntry** has:
- `EntryType` / `Field` - Type and name
- `IsOwnedType`, `IsKey`, `IsArray`, `IsReference`, `IsEnumeration`, `IsNullable`
- `ReferenceType` - For references, the target definition name

### Todo Project Definitions

| Definition | IsArray | IsOwnedType | IsEnumeration | Notes |
|---|---|---|---|---|
| TodoItem | false | false | false | Main entity, has ChecklistTask[] array |
| Note | false | false | false | Simple entity with PredefinedColor enum |
| ChecklistTask | true | false | false | Array child of TodoItem |
| User | false | false | false | Referenced by TodoItem |
| PredefinedColor | false | false | true | Enum used by Note |
| TodoItemStatus | false | false | true | Enum used by TodoItem |

### Common Template Patterns

**Skip conditions** for per-definition templates:
- `SetOutputPath(Definition.IsOwnedType || Definition.IsEnumeration || Definition.IsArray)` - Skip owned types, enums, and array types
- `SetOutputPath(Definition.IsEnumeration || Definition.IsOwnedType)` - Skip enums and owned types (generates for array types too)

**Conditional code based on entries:**
- `Entries.Where(e => e.IsArray).Any()` - True when definition has array children (e.g., TodoItem has Checklist)
- `Entries.Where(e => e.IsReference)` - Reference entries (generate ComboBox, service dependencies)
- `Entries.Where(e => e.IsEnumeration)` - Enum entries (generate enum ComboBox with converter)

### Fixing Templates Workflow

1. Run temgen: `dotnet run --project src/TemGen/TemGen.csproj -- --path ./Projects/Todo`
2. Check diff: `git diff Projects/Todo/Output`
3. For each changed output file, compare template vs expected output
4. Fix the template in the relevant `Templates/Default_*` layer to produce the exact expected output
5. Re-run temgen and verify `git diff --stat Projects/Todo/Output` shows no changes

### Key Files

- `Templates/Default_Scripts/Common.cs` - Helper functions (`SetOutputPath`, `SetOutputPathAndSkipOtherDefinitions`, multilayer merge helpers, mapping helpers, etc.)
- `Projects/Todo/ProjectDefinition.json` - Defines namespace (`Todo`), paths to definitions/templates/output
- `src/TemGen/Definition.cs` - Definition class model
- `src/TemGen/DefinitionEntry.cs` - Entry class model
- `src/TemGen/TemplateProcessor.cs` - Core processing logic (skips empty RelativePath, breaks on SkipOtherDefinitions)

## Coding Conventions

- Follow existing code style as defined in `.editorconfig`
- Use latest C# language features (LangVersion: latest)
- Keep namespace and file structure consistent with existing patterns

## When Making Changes

- If modifying template processing logic, test with the Todo project
- Run the code generator after changes to ensure templates still work correctly
- Verify generated output in `./Projects/Todo/Output` matches expectations
- Update tests in `TemGen.Tests` when adding new features
