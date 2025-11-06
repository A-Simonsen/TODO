// ============================================================================
// TodoFinder - A reflection-based tool to scan assemblies for [Todo] attributes
// ============================================================================
// This program scans all loaded assemblies for TodoAttribute decorations on:
// - Classes, Methods, Properties, Fields, and Constructors
// Returns exit code 1 if any TODOs are found (for build integration)
// ============================================================================

using System;
using System.Reflection;
using Todo.Attribute;

namespace TodoFinder
{
    class Program
    {
        #region Constants

        /// <summary>
        /// Binding flags used for efficient reflection scanning.
        /// Includes: Public, NonPublic, Instance, Static, and DeclaredOnly members.
        /// DeclaredOnly ensures we don't scan inherited members multiple times.
        /// </summary>
        private const BindingFlags bindingFlags = BindingFlags.Public |
                                                  BindingFlags.NonPublic |
                                                  BindingFlags.Instance |
                                                  BindingFlags.Static |
                                                  BindingFlags.DeclaredOnly;

        #endregion

        #region Main Entry Point

        static void Main(string[] args)
        {
            Console.WriteLine("Starting TODO Finder...");
            Console.WriteLine("---------------------------------");

            // Get all currently loaded assemblies in the application domain
            var allAssemblies = AppDomain.CurrentDomain.GetAssemblies();
            string finderAssemblyName = Assembly.GetExecutingAssembly().GetName().Name;
            
            // Track the number of TODOs found and the exit code for the process
            int todoCount = 0;
            int exitCode = 0;

            // Scan each assembly for TODO attributes
            foreach (Assembly assemblyToScan in allAssemblies)
            {
                string assemblyName = assemblyToScan.GetName().Name;

                // Skip system assemblies, Microsoft assemblies, and this program itself
                if (assemblyName.StartsWith("System", StringComparison.OrdinalIgnoreCase)
                    || assemblyName.StartsWith("Microsoft", StringComparison.OrdinalIgnoreCase)
                    || assemblyName == finderAssemblyName)
                {
                    continue;
                }

                Console.WriteLine($"--- Scanning Project: {assemblyName} ---");
                Console.WriteLine();

                // Attempt to scan all types in the assembly
                try
                {
                    todoCount += ScanAssemblyForTodos(assemblyToScan);
                }
                catch (ReflectionTypeLoadException ex)
                {
                    // Handle cases where some types in the assembly cannot be loaded
                    HandleReflectionException(assemblyName, ex);
                }
            }

            // Display final results and determine exit code
            PrintFinalResults(todoCount, ref exitCode);

            // Wait for user input before exiting (useful for debugging)
            Console.WriteLine("Press any key to exit.");
            Console.ReadKey();
            Environment.Exit(exitCode);
        }

        #endregion

        #region Assembly Scanning

        /// <summary>
        /// Scans all types in an assembly for TODO attributes on classes, methods, properties, fields, and constructors.
        /// </summary>
        /// <param name="assembly">The assembly to scan</param>
        /// <returns>The number of TODO items found in this assembly</returns>
        private static int ScanAssemblyForTodos(Assembly assembly)
        {
            int count = 0;

            foreach (Type type in assembly.GetTypes())
            {
                // Scan the class/type itself
                count += ScanType(type);

                // Scan all members of the type
                count += ScanMethods(type);
                count += ScanProperties(type);
                count += ScanFields(type);
                count += ScanConstructors(type);
            }

            return count;
        }

        /// <summary>
        /// Checks if a class or type is decorated with [Todo] attribute.
        /// </summary>
        private static int ScanType(Type type)
        {
            var classTodo = type.GetCustomAttribute<TodoAttribute>();
            if (classTodo != null)
            {
                PrintTodo($"[CLASS] {type.Name}", classTodo);
                return 1;
            }
            return 0;
        }

        /// <summary>
        /// Scans all methods in a type for [Todo] attributes.
        /// </summary>
        private static int ScanMethods(Type type)
        {
            int count = 0;
            foreach (MethodInfo method in type.GetMethods(bindingFlags))
            {
                var methodTodo = method.GetCustomAttribute<TodoAttribute>();
                if (methodTodo != null)
                {
                    count++;
                    PrintTodo($"  [METHOD] {type.Name}.{method.Name}()", methodTodo);
                }
            }
            return count;
        }

        /// <summary>
        /// Scans all properties in a type for [Todo] attributes.
        /// </summary>
        private static int ScanProperties(Type type)
        {
            int count = 0;
            foreach (PropertyInfo prop in type.GetProperties(bindingFlags))
            {
                var propTodo = prop.GetCustomAttribute<TodoAttribute>();
                if (propTodo != null)
                {
                    count++;
                    PrintTodo($"  [PROPERTY] {type.Name}.{prop.Name}", propTodo);
                }
            }
            return count;
        }

        /// <summary>
        /// Scans all fields in a type for [Todo] attributes.
        /// </summary>
        private static int ScanFields(Type type)
        {
            int count = 0;
            foreach (FieldInfo field in type.GetFields(bindingFlags))
            {
                var fieldTodo = field.GetCustomAttribute<TodoAttribute>();
                if (fieldTodo != null)
                {
                    count++;
                    PrintTodo($"  [FIELD] {type.Name}.{field.Name}", fieldTodo);
                }
            }
            return count;
        }

        /// <summary>
        /// Scans all constructors in a type for [Todo] attributes.
        /// </summary>
        private static int ScanConstructors(Type type)
        {
            int count = 0;
            foreach (ConstructorInfo ctor in type.GetConstructors(bindingFlags))
            {
                var ctorTodo = ctor.GetCustomAttribute<TodoAttribute>();
                if (ctorTodo != null)
                {
                    count++;
                    // .ctor is the standard name for a constructor in metadata
                    PrintTodo($"  [CONSTRUCTOR] {type.Name}.ctor()", ctorTodo);
                }
            }
            return count;
        }

        #endregion

        #region Output and Display

        /// <summary>
        /// Prints a formatted TODO item to the console with all its properties.
        /// </summary>
        /// <param name="location">The location of the TODO (e.g., class, method, property)</param>
        /// <param name="todo">The TodoAttribute instance containing the TODO details</param>
        private static void PrintTodo(string location, TodoAttribute todo)
        {
            // Print the location in yellow
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine(location);
            Console.ResetColor();

            // Always print the message (it's required)
            Console.WriteLine($"  Message: {todo.Message}");

            // Get all public instance properties of TodoAttribute (excluding Message)
            var properties = typeof(TodoAttribute).GetProperties(
                BindingFlags.Public |
                BindingFlags.Instance |
                BindingFlags.DeclaredOnly);

            // Print any optional properties that have values
            foreach (var prop in properties)
            {
                // Skip the Message property since we already printed it
                if (prop.Name == "Message")
                {
                    continue;
                }

                object? value = prop.GetValue(todo);
                bool shouldPrint = false;

                // Check if the value should be printed based on its type
                if (value is string strValue)
                {
                    // Only print non-empty strings
                    shouldPrint = !string.IsNullOrEmpty(strValue);
                }
                else if (value is int intValue)
                {
                    // Print any integer value (non-nullable int with a value)
                    shouldPrint = true;
                }
                else if (value != null)
                {
                    // Print any other non-null value
                    shouldPrint = true;
                }

                if (shouldPrint)
                {
                    Console.WriteLine($"  {prop.Name}: {value}");
                }
            }
            Console.WriteLine();
        }

        /// <summary>
        /// Prints the final results of the scan and sets the exit code.
        /// </summary>
        /// <param name="todoCount">The total number of TODOs found</param>
        /// <param name="exitCode">Reference to the exit code variable to set</param>
        private static void PrintFinalResults(int todoCount, ref int exitCode)
        {
            Console.WriteLine("---------------------------------");

            if (todoCount > 0)
            {
                // TODOs found - this would fail a build
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"Scan complete. Found {todoCount} TODO items! Build would fail.");
                Console.ResetColor();

                // Set exit code to 1 to signal failure (useful for CI/CD pipelines)
                exitCode = 1;
            }
            else
            {
                // No TODOs found - build can proceed
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("Scan complete. No TODO items found. Build passes!");
                Console.ResetColor();
            }
        }

        #endregion

        #region Error Handling

        /// <summary>
        /// Handles ReflectionTypeLoadException by displaying the main error and all loader exceptions.
        /// This provides detailed debugging information when types fail to load.
        /// </summary>
        /// <param name="assemblyName">The name of the assembly that failed to load</param>
        /// <param name="ex">The ReflectionTypeLoadException that was thrown</param>
        private static void HandleReflectionException(string assemblyName, ReflectionTypeLoadException ex)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"Could not scan types in {assemblyName}: {ex.Message}");

            // Print detailed loader exceptions for better debugging
            // These contain the actual reasons why specific types failed to load
            if (ex.LoaderExceptions != null)
            {
                foreach (Exception loaderEx in ex.LoaderExceptions)
                {
                    if (loaderEx != null)
                    {
                        Console.WriteLine($"  Loader Exception: {loaderEx.Message}");
                    }
                }
            }
            Console.ResetColor();
        }

        #endregion
    }
}