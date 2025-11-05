using Todo.Attribute;
using System;
using System.IO;
using System.Reflection;
using System.Linq;

namespace TodoFinder
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Starting TODO Finder...");

            /// Get all assemblies in the current domain.
            var allAssemblies = AppDomain.CurrentDomain.GetAssemblies();

            /// Get the executing assembly name
            string finderAssemblyName = Assembly.GetExecutingAssembly().GetName().Name;

            /// Counts the number of TODOs found
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

                        // Scan methods
                        foreach (MethodInfo method in type.GetMethods())
                        {
                            var methodTodo = method.GetCustomAttribute<TodoAttribute>();
                            if (methodTodo != null)
                            {
                                todoCount++;
                                PrintTodo($"  [METHOD] {type.Name}.{method.Name}()", methodTodo);
                            }
                        }
                    }
                }
                catch (ReflectionTypeLoadException ex)
                {
                    // This can happen, just log it and continue
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"Could not scan types in {assemblyName}: {ex.Message}");
                    Console.ResetColor();
                }
            }
            Console.WriteLine("---------------------------------");
            Console.WriteLine($"Scan complete. Found {todoCount} TODO items.");
            Console.ReadKey();
        }
        static void PrintTodo(string location, TodoAttribute todo)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine(location);

            Console.ResetColor();

            // 1. We know "Message" always exists (from the constructor), so we print it first.
            Console.WriteLine($"  Message: {todo.Message}");

            // 2. Get all other public properties that are declared *only* on our TodoAttribute class
            //    (This skips properties from the base 'Attribute' class, like 'TypeId')
            var properties = typeof(TodoAttribute).GetProperties(
                BindingFlags.Public |
                BindingFlags.Instance |
                BindingFlags.DeclaredOnly);

            foreach (var prop in properties)
            {
                // 3. We already printed Message, so skip it in this loop
                if (prop.Name == "Message")
                {
                    continue;
                }

                // 4. Get the value of the property (e.g., "Alex", 123, or null)
                object? value = prop.GetValue(todo);

                // 5. Check if the value is meaningful before printing
                bool shouldPrint = value != null;

                if (value is string strValue) // Check if it's a string
                {
                    shouldPrint = !string.IsNullOrEmpty(strValue);
                }
                else if (value is int intValue) // Check if it's an int
                {
                    // This checks against the default value we set in the attribute
                    shouldPrint = (intValue != -1);
                }

                // 6. If the value is not null or default, print its name and value
                if (shouldPrint)
                {
                    Console.WriteLine($"  {prop.Name}: {value}");
                }
            }

            Console.WriteLine(); // Add a blank line for spacing
        }
    }
}