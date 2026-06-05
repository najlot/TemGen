# Todo Application

Generated with `Default_Backend`.

## Included templates
- `Default_Backend`: contracts, service, and service tests

## Projects
- `Todo.Contracts`
- `Todo.Service`
- `Todo.Service.Test`

## Note
Note has following children:
 - Color (PredefinedColor enum)

## TodoItem
TodoItem has following children:
 - Status (TodoItemStatus enum)
 - Checklist (list of ChecklistTask)

## Data access layer
Requires `Default_Backend`.

Projects:
- `Todo.Client.Data`
- `Todo.Client.Data.Test`
- `Todo.Client.Localisation`

## MVVM layer
Requires `Default_Backend` and `Default_DataAccess`.

Projects:
- `Todo.Client.MVVM`
- `Todo.ClientBase`
- `Todo.ClientBase.Test`

## WPF client
Requires `Default_Backend`, `Default_DataAccess`, and `Default_MVVM`.

Projects:
- `Todo.Wpf`

## Avalonia client
Requires `Default_Backend`, `Default_DataAccess`, and `Default_MVVM`.

Projects:
- `Todo.Avalonia`
- `Todo.Avalonia.Android`
- `Todo.Avalonia.Browser`
- `Todo.Avalonia.Desktop`
- `Todo.Avalonia.iOS`

## Blazor client
Requires `Default_Backend` and `Default_DataAccess`.

Projects:
- `Todo.Blazor`

## HTMX client
Requires `Default_Backend` and `Default_DataAccess`.

Projects:
- `Todo.Htmx`
