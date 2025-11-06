# TodoAttribute - School Project

A C# attribute library for marking code with TODO items, with a reflection-based scanning tool.

## What It Does

- Mark code with `[Todo]` attributes
- Scan assemblies to find all TODOs
- Track optional Owner and Priority

## Usage Example

```csharp
using Todo.Attribute;

[Todo("Add error handling")]
public class Calculator
{
    [Todo("Implement validation", Owner = "Alex", Priority = 1)]
    public int Add(int a, int b)
 {
        return a + b;
    }
}
```

## Running the TodoFinder

```bash
dotnet run --project TodoFinder/TodoFinder.csproj
```

This will scan loaded assemblies and print all TODO items found.

## Building

```bash
dotnet build
```

## Project Structure

- **TodoAttribute**: The custom attribute library
- **TodoFinder**: Console app that scans for TODOs using reflection

## Features

- ? Custom C# attribute
- ? Reflection-based scanning
- ? Optional metadata (Owner, Priority)
- ? .NET 8 with C# 12