// 1. Make sure this 'using' matches your attribute's namespace
using System;
using System.Reflection;
using Todo.Attribute;
using YourProjectName.Attributes;
// using System.IO; // Not needed
// using System.Linq; // Not needed

namespace TodoFinder
{
    class Program
    {
        // 2. We add BindingFlags to be more efficient.
        // This tells Reflection to get public, non-public, instance, and static members,
        // but ONLY the ones declared in this type (not inherited).
        private const BindingFlags bindingFlags = BindingFlags.Public |
                                                  BindingFlags.NonPublic |
                                                  BindingFlags.Instance |
                                                  BindingFlags.Static |
                                                  BindingFlags.DeclaredOnly;
        static void Main(string[] args)
        {
            Console.WriteLine("Starting TODO Finder...");
            Console.WriteLine("---------------------------------");

            var allAssemblies = AppDomain.CurrentDomain.GetAssemblies();
            string finderAssemblyName = Assembly.GetExecutingAssembly().GetName().Name;
            int todoCount = 0;

            foreach (Assembly assemblyToScan in allAssemblies)
            {
                string assemblyName = assemblyToScan.GetName().Name;

                if (assemblyName.StartsWith("System")
                    || assemblyName.StartsWith("Microsoft")
                    || assemblyName == finderAssemblyName)
                {
                    continue;
                }

                Console.WriteLine($"--- Scanning Project: {assemblyName} ---");
                Console.WriteLine();

                try
                {
                    foreach (Type type in assemblyToScan.GetTypes())
                    {
                        // Check for TODOs on the class itself
                        var classTodo = type.GetCustomAttribute<TodoAttribute>();
                        if (classTodo != null)
                        {
                            todoCount++;
                            PrintTodo($"[CLASS] {type.Name}", classTodo);
                        }

                        // 3. Scan methods using our efficient bindingFlags
                        foreach (MethodInfo method in type.GetMethods(bindingFlags))
                        {
                            var methodTodo = method.GetCustomAttribute<TodoAttribute>();
                            if (methodTodo != null)
                            {
                                todoCount++;
                                PrintTodo($"  [METHOD] {type.Name}.{method.Name}()", methodTodo);
                            }
                        }

                        // 4. ADDITION: Scan properties
                        foreach (PropertyInfo prop in type.GetProperties(bindingFlags))
                        {
                            var propTodo = prop.GetCustomAttribute<TodoAttribute>();
                            if (propTodo != null)
                            {
                                todoCount++;
                                PrintTodo($"  [PROPERTY] {type.Name}.{prop.Name}", propTodo);
                            }
                        }

                        // 5. ADDITION: Scan constructors
                        foreach (ConstructorInfo ctor in type.GetConstructors(bindingFlags))
                        {
                            var ctorTodo = ctor.GetCustomAttribute<TodoAttribute>();
                            if (ctorTodo != null)
                            {
                                todoCount++;
                                // .ctor is the standard name for a constructor in metadata
                                PrintTodo($"  [CONSTRUCTOR] {type.Name}.ctor()", ctorTodo);
                            }
                        }
                    }
                }
                catch (ReflectionTypeLoadException ex)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"Could not scan types in {assemblyName}: {ex.Message}");
                    Console.ResetColor();
                }
            }
            Console.WriteLine("---------------------------------");

            if (todoCount > 0)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"Scan complete. Found {todoCount} TODO items! Build would fail.");
                Console.ResetColor();

                // 6. ADDITION: This is how you fail a build script.
                // An exit code other than 0 signals an error.
                Environment.Exit(1);
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("Scan complete. No TODO items found. Build passes!");
                Console.ResetColor();
            }

            // We only reach ReadKey if the build passes.
            Console.WriteLine("Press any key to exit.");
            Console.ReadKey();
        }

        static void PrintTodo(string location, TodoAttribute todo)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine(location);

            Console.ResetColor();
            Console.WriteLine($"  Message: {todo.Message}");

            var properties = typeof(TodoAttribute).GetProperties(
                BindingFlags.Public |
                BindingFlags.Instance |
                BindingFlags.DeclaredOnly);

            foreach (var prop in properties)
            {
                if (prop.Name == "Message")
                {
                    continue;
                }

                object? value = prop.GetValue(todo);
                bool shouldPrint = value != null;

                if (value is string strValue)
                {
                    shouldPrint = !string.IsNullOrEmpty(strValue);
                }
                else if (value is int intValue)
                {
                    shouldPrint = (intValue != -1);
                }

                if (shouldPrint)
                {
                    Console.WriteLine($"  {prop.Name}: {value}");
                }
            }
            Console.WriteLine();
        }
    }
}