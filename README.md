# TodoAttribute - Drag & Drop Solution

A C# attribute library for marking code with TODO items, with a reflection-based scanning tool.

## Quick Setup for Your Bigger Project (30 seconds)

### Step 1: Drop the Folder
Copy the entire `TodoAttribute.Sol` folder into your solution directory:
```
YourBigSolution/
├── YourMainProject/
├── YourWebAPI/
└── TodoAttribute.Sol/    ← Drop this whole folder here
    ├── TodoAttribute/  (The attribute library)
    └── TodoFinder/         (The scanning tool)
```

### Step 2: Add Project Reference
Right-click your project → Add → Project Reference → Check `Todo.Attribute`

**Or** edit your `.csproj`:

```xml
<ItemGroup>
  <ProjectReference Include="..\TodoAttribute.Sol\TodoAttribute\Todo.Attribute.csproj" />
</ItemGroup>
```

### Step 3: Use It
```csharp
using Todo.Attribute;

[Todo("Implement this feature")]
public class MyClass 
{
    [Todo("Add validation", Owner = "Alex", Priority = 1)]
    public void MyMethod() { }
}
```

### Step 4: Find All TODOs
```cmd
dotnet run --project TodoAttribute.Sol\TodoFinder\TodoFinder.csproj
```

---

##  Finding All TODOs in Your Project

### Option 1: Run TodoFinder Tool (Recommended)
```cmd
# From your solution directory
dotnet run --project TodoAttribute.Sol\TodoFinder\TodoFinder.csproj
```

**Output:**
```
Starting TODO Finder...
---------------------------------
--- Scanning Project: YourProject ---

[CLASS] UserService
  Message: Implement caching

  [METHOD] UserService.CreateUser()
  Message: Add validation
  Owner: Alex
  Priority: 1

---------------------------------
Scan complete. Found 2 TODO items!
```

### Option 2: Create a Helper Script

**find-todos.bat** (in your solution root):
```cmd
@echo off
echo Scanning for TODOs...
set TODO_PATH=C:\Users\alexs\source\repos\A-Simonsen\TODO
dotnet add reference "%TODO_PATH%\TodoFinder\TodoFinder.csproj"
dotnet run --project "%TODO_PATH%\TodoFinder\TodoFinder.csproj"
dotnet remove reference "%TODO_PATH%\TodoFinder\TodoFinder.csproj"
pause
```

### Option 3: Quick Search in IDE
- Press `Ctrl+Shift+F` in Visual Studio
- Search for: `[Todo(`

---

## What You Get

- **TodoAttribute** - Mark classes, methods, properties, fields, constructors
- **TodoFinder** - Scans and lists all TODOs using reflection
- **Optional metadata** - Owner, Priority, and extensible
- **Zero dependencies** - Pure .NET 8, no NuGet packages needed
- **Drag & drop** - Just copy the folder and reference it

---

## Full Example

```csharp
using Todo.Attribute;

[Todo("Refactor this class")]
public class ProductService
{
    [Todo("Implement caching", Priority = 1)]
    public Product GetProduct(int id) 
 {
        return null; // TODO implementation
    }

    [Todo("Add data validation", Owner = "TeamA", Priority = 2)]
    public string ProductName { get; set; }

    [Todo("Make async")]
    public void SaveProduct(Product product)
 {
        // Save logic
    }
}
```

Then run: `dotnet run --project TodoAttribute.Sol\TodoFinder\TodoFinder.csproj`

---

## Extending the Attribute

Want more properties? Edit `TodoAttribute/TodoAttribute.cs`:

```csharp
public class TodoAttribute : System.Attribute
{
    // ...existing code...
    
    // Add custom properties:
    public DateTime? DueDate { get; set; }
    public string? IssueUrl { get; set; }
}
```

TodoFinder will **automatically discover and display** new properties!

---

## Perfect for School Projects

Demonstrates:
- Custom C# attributes
- Reflection and metadata
- Multi-project solutions
- Clean architecture
- Extensible design

---

## Integration Patterns

### Multiple Projects