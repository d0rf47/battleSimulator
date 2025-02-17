using Csla;
using Csla.Core;
using Csla.Reflection;
using Csla.Server;
using System.Collections.Concurrent;
using System.Reflection;

namespace BattleSimulatorAPI.Factory
{
    public class ObjectFactory
    {
		/// <summary>
		/// Creates a new instance of a business object implementing the specified interface
		/// </summary>
		/// <typeparam name="T">Type of interface</typeparam>
		/// <returns>Instance of business object supporting the specified interface</returns>
		public static T New<T>()
		{
			var concreteType = TypeResolver.ResolveType(typeof(T));

			if (concreteType.GetMethod("New", BindingFlags.Static | BindingFlags.Public) != null)
			{
				return (T)MethodCaller.CallFactoryMethod(concreteType, "New");
			}
			// Use reflection to call DataPortal.Create<T>() dynamically
			var method = typeof(DataPortal)
				.GetMethod("Create", Type.EmptyTypes) // Get the generic Create<T>() method
				.MakeGenericMethod(concreteType); // Assign the resolved type as T

			return (T)method.Invoke(null, null);
		}

		/// <summary>
		/// Fetches a business object, implementing the specified interface, from Data Portal
		/// </summary>
		/// <param name="parameters">Parameters for the factory method of business object</param>
		/// <typeparam name="T">Type of interface</typeparam>
		/// <returns>Instance of business object supporting the specified interface</returns>
		public static T Get<T>(params object[] parameters)
		{
			var concreteType = TypeResolver.ResolveType(typeof(T));

			return (T)MethodCaller.CallFactoryMethod(concreteType, "Get", parameters);
		}

		/// <summary>
		/// Fetches a list of read-only objects, implementing the specified interface, from Data Portal
		/// </summary>
		/// <param name="commandSuffix">CommandSuffix</param>
		/// <param name="parameters">Parameters</param>
		/// <typeparam name="T">Type of interface supported by list of objects</typeparam>
		/// <returns>DbResultSet</returns>
		//public static DbResultSet GetObjects<T>(string commandSuffix, IReadOnlyDictionary<string, object> parameters)
		//{
		//	var concreteType = TypeResolver.ResolveType(typeof(T));

		//	return (DbResultSet)MethodCaller.CallFactoryMethod(concreteType, "GetResultSet", commandSuffix, parameters);
		//}

		/// <summary>
		/// Executes a static method on a business object
		/// </summary>
		/// <typeparam name="T">Type of interface</typeparam>
		/// <returns>Return value of static method </returns>
		public static object ExecuteStaticMethod<T>(string methodName, params object[] parameters)
		{
			var concreteType = TypeResolver.ResolveType(typeof(T));

			return MethodCaller.CallFactoryMethod(concreteType, methodName, parameters);
		}
	}


	public static class TypeResolver
	{
		private static readonly ConcurrentDictionary<Type, Type> ResolverCache = new ConcurrentDictionary<Type, Type>();
		private static readonly Dictionary<string, string> FrameworkTypes = new Dictionary<string, string>() { };

		private const string FrameworkTypeNameFormat =
			"Constellationhb.Nx.Core.Business.Framework.{0}, Constellationhb.Nx.Core.Business";

		/// <summary>
		/// Resolves type of a business object from type of an interface
		/// </summary>
		/// <param name="interfaceType">Type of interface</param>
		/// <returns>Type of business object supporting that interface</returns>
		public static Type ResolveType(Type interfaceType)
		{
			if (interfaceType == null || !interfaceType.IsInterface ||
				string.IsNullOrWhiteSpace(interfaceType.Namespace))
				return interfaceType;

			return ResolverCache.GetOrAdd(interfaceType, t =>
			{
				if (ResolveFrameworkInterface(interfaceType, out var frameworkClassType))
					return frameworkClassType;

				var interfaceNamespace = t.Namespace ?? string.Empty;
				var interfaceName = t.Name;
				if (!interfaceNamespace.StartsWith("Constellationhb.Nx.Business.Common.Contracts."))
					throw new NotSupportedException(
						$"Unable to identify type from interface {interfaceNamespace}.{interfaceName}");

				var namespaceParts = interfaceNamespace.Split('.');
				if (namespaceParts.Length != 7)
					throw new NotSupportedException(
						$"Unable to identify type from interface {interfaceNamespace}.{interfaceName}");

				// Module name is the second last part of namespace
				var moduleName = namespaceParts[namespaceParts.Length - 2];
				// Entity name is the last part of namespace
				var entityName = namespaceParts.Last();
				var assemblyName = $"Constellationhb.Nx.{moduleName}.Business";
				var fullyQualifiedName = $"{assemblyName}.{entityName}Entities.{interfaceName.Substring(1)}, {assemblyName}";

				var actualType = Type.GetType(fullyQualifiedName);
				if (actualType == null)
					throw new NotSupportedException($"Unable to find type {fullyQualifiedName}");

				if (actualType.GetInterface(interfaceType.FullName) == null)
					throw new NotImplementedException(
						$"The type {fullyQualifiedName} does not implement the specified interface ({interfaceNamespace}.{interfaceName}).");

				return actualType;
			});
		}

		/// <summary>
		/// Resolves type of interface from a business object type
		/// </summary>
		/// <param name="objType">Type of business object</param>
		/// <returns>Type of interface supported by the specified business object type</returns>
		public static Type ResolveInterface(Type objType)
		{
			if (objType == null || !objType.IsClass ||
				string.IsNullOrWhiteSpace(objType.Namespace))
				return null;

			return ResolverCache.GetOrAdd(objType, t =>
			{
				var typeNamespace = t.Namespace ?? string.Empty;
				var typeName = t.Name;
				if (!typeNamespace.StartsWith("Constellationhb.Nx."))
					return null;

				var namespaceParts = typeNamespace.Split('.');
				if (namespaceParts.Length != 5)
					return null;

				// Module name is the third part of namespace
				var moduleName = namespaceParts[2];
				// Entity name is in the last part of namespace
				var entityName = namespaceParts.Last();
				if (!entityName.EndsWith("Entities"))
					return null;

				entityName = entityName.Substring(0, entityName.Length - "Entities".Length);
				var assemblyName = "Constellationhb.Nx.Business.Common";
				var fullyQualifiedName = $"{assemblyName}.Contracts.{moduleName}.{entityName}.I{typeName}, {assemblyName}";

				var interfaceType = Type.GetType(fullyQualifiedName);
				if (interfaceType == null)
					return null;

				return objType.GetInterface(interfaceType.FullName) == null ? null : interfaceType;
			});
		}

		public static object CreateInstance(Type objectType)
		{
			return Activator.CreateInstance(ResolveType(objectType));
		}

		public static TEnumerator ConvertToEnumerator<TEnumerator>(string value)
		{
			return (TEnumerator)Enum.Parse(typeof(TEnumerator), string.IsNullOrEmpty(value) ? "-1" : value);
		}

		private static bool ResolveFrameworkInterface(Type frameworkInterfaceType, out Type frameworkClassType)
		{
			var ns = frameworkInterfaceType.Namespace;

			frameworkClassType = null;

			if (ns != "Constellationhb.Enterprise.Business.Contracts") return false;

			if (!FrameworkTypes.TryGetValue(frameworkInterfaceType.Name, out var frameworkTypeName)) return false;

			frameworkClassType = Type.GetType(frameworkTypeName);

			if (frameworkClassType == null)
				throw new NotSupportedException($"Unable to find type {frameworkTypeName}");

			return true;
		}
	}
}
