# Default C# Templates for TemGen

The default application template is split into composable layers. Add template folders to `TemplatesPath` in dependency order; every default application layer requires `Default_Backend`.

## Layers

| Template | Depends on | Contents |
| --- | --- | --- |
| `Default_Backend` | none | Contracts, ASP.NET Core service, service tests, persistence abstractions, auth, audit, history, favorites, filters, generated entity features |
| `Default_DataAccess` | `Default_Backend` | Client data services, API repositories, localisation project, client data tests |
| `Default_MVVM` | `Default_Backend`, `Default_DataAccess` | MVVM helpers, shared client view models, view-model tests |
| `Default_WPF` | `Default_Backend`, `Default_DataAccess`, `Default_MVVM` | WPF desktop client |
| `Default_Avalonia` | `Default_Backend`, `Default_DataAccess`, `Default_MVVM` | Avalonia shared client plus Desktop, Browser, Android, and iOS heads |
| `Default_Blazor` | `Default_Backend`, `Default_DataAccess` | Server-side Blazor client |
| `Default_HTMX` | `Default_Backend`, `Default_DataAccess` | ASP.NET Core Razor Pages/HTMX client |

`Default_Scripts` is not an output layer. Use it as `ScriptsPath`; it contains shared TemGen helper functions used by the default templates.

## Examples

- Backend only: `Default_Backend`
- Backend plus Blazor: `Default_Backend;Default_DataAccess;Default_Blazor`
- Backend plus WPF: `Default_Backend;Default_DataAccess;Default_MVVM;Default_WPF`
- Full default app: `Default_Backend;Default_DataAccess;Default_MVVM;Default_WPF;Default_Avalonia;Default_Blazor;Default_HTMX`

`Directory.Packages.props`, `README.md`, and `Todo.slnx` are layered files. Later templates append their required package versions, generated README sections, and solution projects to the content produced by earlier templates.
