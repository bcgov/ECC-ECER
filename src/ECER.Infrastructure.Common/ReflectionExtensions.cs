using System.Reflection;

namespace ECER.Infrastructure.Common
{
    /// <summary>
    /// helper methods to reflection API
    /// </summary>
    public static class ReflectionExtensions
    {
        /// <summary>
        /// Gets a manifest resource by name and return the content as a string
        /// </summary>
        /// <param name="assembly">The assembly hosting the manifest resource</param>
        /// <param name="manifestName">the manifest name</param>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException">Thrown if the manifest is not found in the assembly or cannot be loaded</exception>
        public static async Task<string> GetManifestResourceString(this Assembly assembly, string manifestName)
        {
            ArgumentNullException.ThrowIfNull(assembly);
            using var stream = assembly.GetManifestResourceStream(manifestName);
            if (stream == null) throw new InvalidOperationException($"{nameof(stream)} is null");
            using var reader = new StreamReader(stream);
            return await reader.ReadToEndAsync();
        }

        /// <summary>
        /// Gets an array of types implementing type T in a specific assembly
        /// </summary>
        /// <typeparam name="T">The interface type</typeparam>
        /// <param name="assembly">The assembly to search</param>
        /// <returns>Array of implementing types</returns>
        public static Type[] GetTypesImplementing<T>(this Assembly assembly) => assembly.GetTypesImplementing(typeof(T));

        /// <summary>
        /// Gets an array of types implementing a type in a specific assembly
        /// </summary>
        /// <param name="assembly">The assembly to search</param>
        /// <param name="type">The type to search</param>
        /// <returns>Array of implementing types</returns>
        public static Type[] GetTypesImplementing(this Assembly assembly, Type type)
        {
            ArgumentNullException.ThrowIfNull(assembly);
            return assembly.DefinedTypes.Where(t => t.IsClass && !t.IsAbstract && t.IsPublic && (type.IsAssignableFrom(t) || t.IsAssignableToGenericType(type))).ToArray();
        }

        /// <summary>
        /// Instantiates all types of type T found in an assembly
        /// </summary>
        /// <typeparam name="T">The type to instantiate</typeparam>
        /// <param name="assembly">The assembly to search</param>
        /// <returns>Array of instances of type T</returns>
        public static T[] CreateInstancesOf<T>(this Assembly assembly)
        {
            ArgumentNullException.ThrowIfNull(assembly);
            return assembly.GetTypesImplementing<T>().Select(t => (T?)Activator.CreateInstance(t)).Where(i => i != null).Select(i => i!).ToArray();
        }

        /// <summary>
        /// Checks if a type implements a generic type T
        /// </summary>
        /// <param name="type">The type to check</param>
        /// <param name="genericType">the generic type to evaluate</param>
        /// <returns>true if type implement genericType, otherwise false</returns>
        public static bool IsAssignableToGenericType(this Type type, Type genericType)
        {
            ArgumentNullException.ThrowIfNull(type);
            ArgumentNullException.ThrowIfNull(genericType);
            var interfaceTypes = type.GetInterfaces();

            foreach (var it in interfaceTypes)
            {
                if (it.IsGenericType && it.GetGenericTypeDefinition() == genericType)
                    return true;
            }

            if (type.IsGenericType && type.GetGenericTypeDefinition() == genericType)
                return true;

            var baseType = type.BaseType;
            if (baseType == null) return false;

            return baseType.IsAssignableToGenericType(genericType);
        }

        /// <summary>
        /// Invoke an asynchronous method and await for the value
        /// </summary>
        /// <param name="method">The method to invoke</param>
        /// <param name="obj">The instance to invoke the method on</param>
        /// <param name="parameters">The parameters to pass to the method</param>
        /// <returns>the awaited result</returns>
        public static async Task<object?> InvokeAsync(this MethodInfo method, object obj, params object[] parameters)
        {
            ArgumentNullException.ThrowIfNull(method);
            var task = (Task)(method.Invoke(obj, parameters) ?? null!);
            await task.ConfigureAwait(false);
            return method.ReturnType.IsGenericType
                ? task.GetType().GetProperty("Result")?.GetValue(task)
                : null;
        }

        private static string[] defaultExcludedAssemblyPrefixes = ["System.", "Microsoft."];
        private static string[] assemblyFileExtensions = ["*.dll"];

        /// <summary>
        /// Discovers all local assemblies in a folder
        /// </summary>
        /// <param name="directory">Optional directory, defaults to the current executing directory</param>
        /// <param name="prefix">Optional prefix of assembly names to exclude in the discovery</param>
        /// <returns>array of </returns>
        /// <exception cref="InvalidOperationException"></exception>
        public static Assembly[] DiscoverLocalAessemblies(string? directory = null, string? prefix = null)
        {
            directory ??= Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) ?? throw new InvalidOperationException("Can't determine local assembly directory");
            var excludedPrefixes = prefix != null ? defaultExcludedAssemblyPrefixes.Append(prefix) : defaultExcludedAssemblyPrefixes;

            return assemblyFileExtensions.SelectMany(ext => Directory.GetFiles(directory, ext, SearchOption.TopDirectoryOnly))
                .Where(file => !excludedPrefixes.Any(prefix => Path.GetFileName(file).StartsWith(prefix)))
                .Select(file => Assembly.Load(AssemblyName.GetAssemblyName(file)))
                .ToArray();
        }
    }
}