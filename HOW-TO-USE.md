# How to Use in Your Bigger Project

## Copy This Entire Folder

Just copy the `TodoAttribute.Sol` folder into your solution:

```
C:\MySchoolProject\
??? MyMainApp\
?   ??? MyMainApp.csproj
??? MyLibrary\
?   ??? MyLibrary.csproj
??? TodoAttribute.Sol\          ? Paste this whole folder here
    ??? TodoAttribute\
    ??? TodoFinder\
    ??? find-todos.bat? Helper script
  ??? HOW-TO-USE.md          ? This file
```

## Add Reference

In any project where you want to use `[Todo]` attributes:

**Option A:** Visual Studio
1. Right-click your project ? Add ? Project Reference
2. Check `Todo.Attribute`
3. Click OK

**Option B:** Edit `.csproj` manually
```xml
<ItemGroup>
  <ProjectReference Include="..\TodoAttribute.Sol\TodoAttribute\Todo.Attribute.csproj" />
</ItemGroup>
```

## Use in Code

```csharp
using Todo.Attribute;

[Todo("Implement this")]
public class MyClass
{
    [Todo("Add validation", Owner = "Me", Priority = 1)]
    public void MyMethod() { }
}
```

## Find All TODOs

**Windows:** Double-click `find-todos.bat`

**Or run manually:**
```cmd
dotnet run --project TodoAttribute.Sol\TodoFinder\TodoFinder.csproj
```

**Linux/Mac:**
```bash
./find-todos.sh
```

---

## That's Everything! 

Three steps:
1. ? Copy folder
2. ? Add project reference
3. ? Use `[Todo("...")]` in your code

Run `find-todos.bat` anytime to see all your TODOs! ??
