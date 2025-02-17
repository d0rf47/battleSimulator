using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Reflection;
using BattleSimulatorAPI.Repositories.Models;
using BattleSimulatorAPI.DataLayer.Models.ViewModels;

namespace BattleSimulatorAPI.DataLayer.Models.ViewModels
{
	public static class ViewModelHelpers
	{
		private static ConcurrentDictionary<string, EntityModel> _entityModelCache;
		private static ConcurrentDictionary<string, Type> _viewModelcache;

		static ViewModelHelpers()
		{
			_entityModelCache = new ConcurrentDictionary<string, EntityModel>();
			_viewModelcache = new ConcurrentDictionary<string, Type>();
		}

		public static EntityModel GetEntityModel<TVm>() where TVm : IViewModel
		{
			var modelType = typeof(TVm);
			var eModel = GetEntityModel(modelType);
			return eModel;
		}

		public static Type FindModelByInterface(Type type, List<Type> existingTypes = null)
		{
			if (!_viewModelcache.ContainsKey(type.Name))
			{
				Type viewModel = null;
				if (existingTypes != null)
				{
					viewModel = existingTypes.FirstOrDefault(p =>
						type.IsAssignableFrom(p) && !p.IsInterface &&
						((typeof(ILookup).IsAssignableFrom(type) && typeof(ILookup).IsAssignableFrom(p))));
				}

				if (viewModel == null)
				{
					viewModel = AppDomain.CurrentDomain.GetAssemblies()
						.Where(p => p.FullName.Contains("ViewModel"))
						.SelectMany(s => s.GetTypes()
						).FirstOrDefault(p =>
							type.IsAssignableFrom(p) && !p.IsInterface &&
							((typeof(ILookup).IsAssignableFrom(type) && typeof(ILookup).IsAssignableFrom(p))));
				}

				_viewModelcache[type.Name] = viewModel;
			}

			return _viewModelcache[type.Name];
		}

		public static EntityModel GetEntityModel(string modelName)
		{
			Type type = FindModel(modelName);
			var eModel = GetEntityModel(type);
			return eModel;
		}
		public static EntityModel GetEntityModel(Type entityType)
		{
			if (_entityModelCache.ContainsKey(entityType.Name))
			{
				var entityModelCopyFromCache = new EntityModel();
				entityModelCopyFromCache.Entities.AddRange(_entityModelCache[entityType.Name].Entities);
				return entityModelCopyFromCache;
			}

			var entityModelTypes = new List<Type> { entityType };

			var entityTypeInstance = Activator.CreateInstance(entityType);
			var explicitAssociations = (entityTypeInstance as IViewModel).AssociatedEntites().Select(p => p.GetType()).ToList();
			entityModelTypes.AddRange(explicitAssociations);
			entityModelTypes.AddRange(GetNavigationProperties(entityType, entityModelTypes));


			// clean DefaultModel validations
			for (int i = 0; i < entityModelTypes.Count; i++)
			{
				if (entityModelTypes[i].IsInterface)
				{
					entityModelTypes[i] = FindModelByInterface(entityModelTypes[i], entityModelTypes.Where(p => !p.IsInterface).ToList());
				}
			}
			var emBuilder = new ViewModelEntityModelBuilder();
			EntityModel entityModel = emBuilder.ConstructFromTypes(entityModelTypes);

			entityModel.Entities.RemoveAll(p => p.EntityType.IsInterface && entityModel.Entities.Any(q => p.EntityType.IsAssignableFrom(q.EntityType)));

			foreach (var entity in entityModel.Entities.Where(p => p.EntityType.IsInterface))
			{
				entity.EntityType = FindModelByInterface(entity.EntityType);
			}

			_entityModelCache.TryAdd(entityType.Name, entityModel);
			var entityModelCopy = new EntityModel();
			entityModelCopy.Entities.AddRange(entityModel.Entities);
			return entityModelCopy;
		}

		#region Private Methods

		private static Type FindModel(string typeName)
		{
			var moduleAndType = GetModuleAndType(typeName);
			var viewModel = AppDomain.CurrentDomain.GetAssemblies()
				.Where(p => p.GetName().Name.Contains("ViewModel"))
				.SelectMany(s => s.GetTypes()).FirstOrDefault(p =>
					p.Name == moduleAndType[1] && p.FullName.Contains(moduleAndType[0]) && p.FullName.Contains(moduleAndType[1]) &&
					!p.IsInterface);

			return viewModel;
		}

		private static string[] GetModuleAndType(string typeName)
		{
			var moduleAndType = typeName.Split('.');

			Debug.Assert(moduleAndType.Length == 2);
			return moduleAndType;
		}

		private static IEnumerable<Type> GetNavigationProperties(Type type, List<Type> existingTypes)
		{
			var visitedTypes = new HashSet<Type>();
			var result = new List<Type>();
			InternalVisit(type, visitedTypes, result, existingTypes);
			return result;
		}

		private static void InternalVisit(Type t, HashSet<Type> visitedTypes, IList<Type> result, List<Type> existingTypes)
		{
			Dictionary<string, Type> explicitTypes = new Dictionary<string, Type>();
			if (visitedTypes.Contains(t))
			{
				return;
			}

			var explicitPropertiesOnTypes = t.GetProperties(BindingFlags.Instance | BindingFlags.NonPublic);
			explicitTypes = explicitPropertiesOnTypes.ToDictionary(ele => ele.Name.Split('.').Last(), ele => ele.PropertyType);

			if (typeof(IViewModel).IsAssignableFrom(t))
			{
				visitedTypes.Add(t);
				foreach (var property in t.GetProperties())
				{
					if (typeof(IViewModel).IsAssignableFrom(property.PropertyType))
					{
						Type propType = explicitTypes.ContainsKey(property.Name) ? explicitTypes[property.Name] : property.PropertyType;
						if (propType.IsInterface && typeof(IViewModel).IsAssignableFrom(propType))
						{
							propType = FindModelByInterface(propType, existingTypes);
							if (propType == null)
								throw new NullReferenceException(
									$"Unable to find model by interface for property {property.Name}");
						}

						result.Insert(0, propType);
						InternalVisit(propType, visitedTypes, result, existingTypes);
					}
					else if (IsGenericViewModelList(property.PropertyType))
					{
						Type propType = explicitTypes.ContainsKey(property.Name) ? explicitTypes[property.Name].GetGenericArguments()[0] : property.PropertyType.GetGenericArguments()[0];
						if (propType.IsInterface && typeof(IViewModel).IsAssignableFrom(propType))
						{
							propType = FindModelByInterface(propType, existingTypes);
						}

						result.Insert(0, propType);
						InternalVisit(propType, visitedTypes, result, existingTypes);
					}

					//ToDo Recursive Property Navigation might be required disabled for now
					//InternalVisit(property.PropertyType, visitedTypes, result);
				}
			}
		}

		private static bool IsGenericViewModelList(Type type)
		{
			if (!type.IsGenericType)
				return false;
			var genericArguments = type.GetGenericArguments();
			if (genericArguments.Length != 1)
				return false;
			if (typeof(IViewModel).IsAssignableFrom(genericArguments[0]))
			{
				var listType = typeof(IList<>).MakeGenericType(genericArguments);
				return listType.IsAssignableFrom(type);
			}

			return false;
		}

		#endregion Private Methods
	}

	public class ViewModelEntityModelBuilder
	{
		internal const string InternalPropertiesPrefix = "_Internal";
		internal const string ParentPropertyBase = "UnifiedParent";
		internal const string IdPropertyName = "Id";

		public EntityModel ConstructFromTypes(List<Type> rootTypes)
		{
			var entityModel = new EntityModel();

			foreach (var rootType in rootTypes)
			{
				GetEntityFromType(entityModel, rootType);
			}

			foreach (var entity in entityModel.Entities)
			{
				var entitiesWithNavigationProperties = entityModel.GetEntitiesForNavigationTarget(entity);
				//entity.IsRoot = entitiesWithNavigationProperties.Keys.Count == 0;
			}

			return entityModel;
		}

		private Entity GetEntityFromType(EntityModel entityModel, Type type)
		{
			var entity = new Entity
			{
				EntityType = type
			};

			var existingEntity = entityModel.Entities.FirstOrDefault(e => e.EntityType == type);
			if (existingEntity != null)
			{
				return existingEntity;
			}

			if (!typeof(IPoco).IsAssignableFrom(type))
			{
				throw new InvalidOperationException(
					$"Requested type: {type.Name} does not implement the IPoco interface; meta data cannot be generated.");
			}
			List<PropertyInfo> props;
			if (!type.IsInterface)
			{
				props = type.GetProperties()
				.Where(pi => pi.GetGetMethod() != null && pi.GetSetMethod() != null).ToList();
			}
			else
			{
				props = type.GetInterfaces().Union(new[] { type }).SelectMany(i => i.GetProperties().Where(pi => pi.GetGetMethod() != null && pi.GetSetMethod() != null)).Distinct().ToList();
			}

			// Rule List
			var navigationProperties = new List<NavigationPropertyInfo>();

			foreach (var prop in props)
			{
				if (prop.Name == "Id" && prop.PropertyType == typeof(Guid))
					continue;
			}				

			var regularProperties =
				props.Where(
					prop => prop.PropertyType != typeof(DataOperation)).ToList();

			entityModel.Entities.Add(entity);

			foreach (var prop in regularProperties)
			{
				if (prop.Name == "Id" && prop.PropertyType == typeof(Guid))
					continue;

				

				var propType = prop.PropertyType.IsEnum ? typeof(Int32) : prop.PropertyType;
				entity.DataProperties.Add(new EntityProperty
				{
					Name = prop.Name,
					PropertyType = propType,
					Attributes = prop.GetCustomAttributes(typeof(Attribute)),					
				});
			}

			//construct dependent entities
			//recursively construct child type definitions from navigation properties
			foreach (var navigationProp in navigationProperties)
			{
				if (navigationProp.TargetNavigationType != null)
				{
					Entity relatedEntity;
					if (navigationProp.TargetNavigationType.IsInterface && typeof(IViewModel).IsAssignableFrom(navigationProp.TargetNavigationType))
					{
						relatedEntity = GetEntityFromType(entityModel, ViewModelHelpers.FindModelByInterface(navigationProp.TargetNavigationType));
					}
					else
					{
						relatedEntity = GetEntityFromType(entityModel, navigationProp.TargetNavigationType);
					}
					var foreignKeyName = navigationProp.IsScalar ?
							$"{InternalPropertiesPrefix}{navigationProp.OriginalPropertyInfo.Name}{IdPropertyName}"
							: $"{InternalPropertiesPrefix}{ParentPropertyBase}{IdPropertyName}";
					if (navigationProp.IsScalar)
					{
						if (Attribute.IsDefined(navigationProp.OriginalPropertyInfo, typeof(DisplayNameAttribute)))
						{
							var displayName = navigationProp.OriginalPropertyInfo.GetCustomAttribute<DisplayNameAttribute>().DisplayName;
							//AddDisplayRule(entity, foreignKeyName, displayName);
						}
						//else if (Attribute.IsDefined(navigationProp.OriginalPropertyInfo, typeof(DisplayAttribute)))
						//{
						//	var displayName = navigationProp.OriginalPropertyInfo.GetCustomAttribute<DisplayAttribute>().Name;
						//	AddDisplayRule(entity, foreignKeyName, displayName);
						//}

					}
					var navProperty = new EntityNavigationProperty
					{
						Name = navigationProp.OriginalPropertyInfo.Name,
						TargetEntity = relatedEntity,
						IsScalar = navigationProp.IsScalar,
						IsDirectChild = navigationProp.IsDirectChild,
						AssociationName = $"{entity.EntityType.Name}_{relatedEntity.EntityType.Name}",
						ForeignKeyName = foreignKeyName
					};

					entity.NavigationProperties.Add(navProperty);
				}
			}

			return entity;
		}
	}
}
