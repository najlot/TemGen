# TemGen

[![.NET](https://img.shields.io/badge/.NET-10.0-512BD4)](https://dotnet.microsoft.com/)
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](LICENSE)

TemGen is a powerful template-based code generator that processes templates with embedded scripts to generate complete application code from simple definitions. It supports multiple scripting languages (C#, JavaScript, Python, Lua) and can generate entire project structures including backend services, WPF clients, and Blazor applications.

## Table of Contents

- [Features](#features)
- [Installation](#installation)
- [Quick Start](#quick-start)
- [Project Structure](#project-structure)
- [How It Works](#how-it-works)
- [Definition Files](#definition-files)
- [Template Syntax](#template-syntax)
- [CLI Options](#cli-options)
- [Example Project](#example-project)
- [License](#license)

## Features

- **Multi-language scripting support**: Embed C#, JavaScript, Python, or Lua scripts in templates
- **Definition-based generation**: Define your data models in simple text files
- **Full project generation**: Generate complete application structures including:
  - Service backends
  - WPF desktop clients
  - Blazor web applications
  - Contracts, events, and DTOs
  - Tests and localization
- **Customizable templates**: Create your own templates or use the included default template
- **Resource management**: Copy and process resource files alongside generated code
- **Watch mode**: Run in loop mode to continuously regenerate on demand

## Installation

### As a .NET Global Tool

```bash
dotnet tool install -g TemGen
```

### From Source

```bash
git clone https://github.com/Najlot/TemGen.git
cd TemGen
dotnet build src/TemGen.slnx
```

## Quick Start

1. **Create a project structure**:

```
MyProject/
├── ProjectDefinition          # Project configuration
├── Definitions/               # Your data model definitions
│   └── User                   # Example: User entity
└── Output/                    # Generated code will go here
```

2. **Define your project** (`ProjectDefinition`):

```
#Global
Namespace:MyApp

#Paths
DefinitionsPath:./Definitions
TemplatesPath:../../Templates/Default/Template
OutputPath:./Output
ResourcesPath:./Resources
ResourcesScriptPath:../../Templates/Default/Resources.cs
ScriptsPath:../../Templates/Default/Scripts
```

3. **Create a definition** (`Definitions/User`):

```
string Username
string Email
DateTime CreatedAt
```

4. **Generate code**:

```bash
temgen --path ./MyProject
```

Or if running from source:

```bash
dotnet run --project src/TemGen/TemGen.csproj -- --path ./MyProject
```

## Project Structure

A TemGen project consists of:

- **ProjectDefinition**: Configuration file specifying paths and global settings
- **Definitions/**: Directory containing entity/model definitions
- **Templates/**: Template files with embedded scripts
- **Scripts/**: Reusable script files
- **Resources/**: Static files to copy to output
- **Output/**: Generated code destination

### Repository Structure

```
TemGen/
├── src/
│   ├── TemGen/              # Main CLI application
│   └── TemGen.Tests/        # Unit tests
├── Templates/
│   └── Default/             # Default C# template
│       ├── Template/        # Template files
│       ├── Scripts/         # Script files
│       └── Resources.cs     # Resources script
└── Projects/
    └── Todo/                # Example Todo project
        ├── Definitions/     # Example definitions
        └── Output/          # Generated output
```

## How It Works

1. **Read Configuration**: TemGen reads your `ProjectDefinition` file to understand project structure
2. **Load Definitions**: Parses definition files to understand your data models
3. **Process Templates**: Iterates through templates, executing embedded scripts
4. **Execute Scripts**: Runs C#, JavaScript, Python, or Lua code to generate output
5. **Write Output**: Saves generated files to the output directory
6. **Incremental Updates**: Only writes files that have changed

## Definition Files

Definition files use a simple format to describe data models:

```
# Basic properties
string PropertyName
int Count
DateTime CreatedAt

# References to other types
UserId Owner
TodoItemStatus Status

# Arrays
string[] Tags
ChecklistTask[] Items
```

### Supported Types

- Primitive types: `string`, `int`, `long`, `bool`, `DateTime`, `Guid`, etc.
- Custom types: Reference other definitions by name
- Arrays: Append `[]` to any type
- Nullable: Append `?` to make properties nullable (when applicable)

## Template Syntax

Templates are regular files with embedded script sections. Script sections are delimited by `<#handler ... #>` tags.

### Supported Handlers

- `<#cs ... #>`: C# scripts
- `<#js ... #>`: JavaScript scripts
- `<#py ... #>`: Python scripts
- `<#lua ... #>`: Lua scripts
- `<#reflection ... #>`: Reflection-based helpers

### Example Template

```csharp
using System;

namespace <#cs Write(Project.Namespace)#>.Models;

public class <#cs Write(Definition.Name)#>
{
<#cs 
    foreach (var entry in Definition.Entries)
    {
        WriteLine($"    public {entry.Type} {entry.Name} {{ get; set; }}");
    }
#>
}
```

### Available Context Objects

In templates, you have access to:

- **Project**: Project configuration (Namespace, paths, custom properties)
- **Definition**: Current definition being processed (Name, Entries, properties)
- **Definitions**: All definitions in the project
- **Write()**, **WriteLine()**: Output methods
- **SetOutputPath()**: Control output file path

## CLI Options

```
Usage: TemGen [options]

Options:
  -p, --path <path>         Path to a project definition file or folder
                            containing a ProjectDefinition file
                            [default: .]
                            
  -l, --loop                Run execution in a loop (watch mode)
                            
  --log-level <level>       Log level: Debug|Error|Fatal|Info|None|Trace|Warn
                            [default: Info]
                            
  -?, -h, --help            Show help and usage information
  --version                 Show version information
```

### Examples

Generate once:
```bash
temgen --path ./MyProject
```

Watch mode (regenerate on demand):
```bash
temgen --path ./MyProject --loop
```

With debug logging:
```bash
temgen --path ./MyProject --log-level Debug
```

## Example Project

The repository includes a complete Todo application example in `Projects/Todo/`:

### Definitions

The Todo project defines entities like:
- `TodoItem`: Tasks with title, content, status, checklist
- `User`: User accounts
- `Note`: Simple notes
- `TodoItemStatus`: Enumeration for task status

### Generated Output

From these simple definitions, TemGen generates:
- **Service Backend**: ASP.NET Core service with repositories, controllers
- **WPF Client**: Desktop application with MVVM pattern
- **Blazor App**: Server-side Blazor web application
- **Contracts**: Events, DTOs, interfaces
- **Tests**: Unit tests for services and client data layer
- **Localization**: Resource files for multi-language support

### Try It

```bash
# Clone the repository
git clone https://github.com/Najlot/TemGen.git
cd TemGen

# Build TemGen
dotnet build src/TemGen.slnx

# Generate the Todo project
dotnet run --project src/TemGen/TemGen.csproj -- --path ./Projects/Todo

# Check the generated output
ls Projects/Todo/Output/
```

You can explore the generated projects:

```bash
cd Projects/Todo/Output
ls -la  # See all generated projects
```

Note: The WPF project requires Windows to build. The generated code structure demonstrates TemGen's capabilities.

## License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

## Contributing

Contributions are welcome! Please feel free to submit a Pull Request.
