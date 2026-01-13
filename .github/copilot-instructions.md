# Copilot Instructions for TemGen

## Project Overview

TemGen is a template-based code generator built with .NET 10.0. It processes templates with embedded scripts (C#, JavaScript, Python, Lua) to generate code based on definitions.

## Repository Structure

- **`./src`** - Source code for the project
  - `TemGen/` - Main CLI application (executable tool)
  - `TemGen.Tests/` - Unit tests
- **`./Templates/Default`** - Default template used for code generation
  - `Template/` - Template files
  - `Scripts/` - Script files (C#, JS, Python, Lua)
  - `Resources.cs` - Resources script
- **`./Projects/Todo`** - Default output project used for generation and testing
  - `Definitions/` - Definition files
  - `Output/` - Generated output from templates
  - `ProjectDefinition` - Project configuration file
  - `Resources/` - Resource files

## How It Works

The project uses the default template (`./Templates/Default`) to generate the output project (`./Projects/Todo`) based on definitions. The Todo project serves as both an example and a test case for the code generation functionality.

## Building and Testing

### Build the Project

```bash
dotnet build src/TemGen.slnx
```

### Run Tests

```bash
dotnet test src/TemGen.Tests/TemGen.Tests.csproj
```

### Run the Tool Locally

```bash
dotnet run --project src/TemGen/TemGen.csproj -- --path ./Projects/Todo
```

## Key Technologies

- **.NET 10.0** - Target framework
- **System.CommandLine** - CLI parsing
- **Microsoft.CodeAnalysis.Scripting** - C# scripting support
- **Jint** - JavaScript execution
- **IronPython** - Python scripting
- **MoonSharp** - Lua scripting
- **Najlot.Log** - Logging framework

## Coding Conventions

- Follow existing code style as defined in `.editorconfig`
- Use latest C# language features (LangVersion: latest)
- Keep namespace and file structure consistent with existing patterns
- Template handlers are extensible via the `ISectionHandler` interface

## When Making Changes

- If modifying template processing logic, test with the Todo project
- Run the code generator after changes to ensure templates still work correctly
- Verify generated output in `./Projects/Todo/Output` matches expectations
- Update tests in `TemGen.Tests` when adding new features
