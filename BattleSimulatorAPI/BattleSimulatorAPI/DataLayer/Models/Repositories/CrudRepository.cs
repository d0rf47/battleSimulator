using Microsoft.EntityFrameworkCore;
using BattleSimulatorAPI.DataLayer;
using BattleSimulatorAPI.Factory;
using System.Web.Http.OData.Query;
using Newtonsoft.Json.Linq;
using BattleSimulatorAPI.DataLayer.Models;
using BattleSimulatorAPI.DataLayer.Models.ViewModels;
using Csla;
using System.Linq;
using System.Reflection;
using System.Collections;
using BattleSimulatorAPI.Extensions;

namespace BattleSimulatorAPI.Repositories.Models.Repositories
{
    public interface ICrudRepository<TVm> where TVm : IViewModel
	{
		/// <summary>
		/// Save Changes to Entity
		/// </summary>
		/// <param name="model"></param>
		/// <returns>VM</returns>
		PocoSaveResult SaveChanges(JObject model);

		/// <summary>
		///  Delete Entity by entityId
		/// </summary>
		/// <param name="queryOptions"></param>
		/// <returns>bool</returns>
		bool DeleteEntity(ODataQueryOptions<DynamicPoco> queryOptions);

		/// <summary>
		/// Get Entity By Id , usually for edit
		/// </summary>
		/// <param name="queryOptions"></param>
		DbsResult<TVm> GetEntity(ODataQueryOptions<DynamicPoco> queryOptions);

		/// <summary>
		/// Create New Entity
		/// </summary>
		/// <param name="queryOptions"></param>
		/// <returns></returns>
		TVm CreateEntity(ODataQueryOptions<DynamicPoco> queryOptions);

		DbsResult<IEnumerable<TVm>> GetInfoListPaged(ODataQueryOptions<DynamicPoco> queryOptions, bool showAll,
			int maxRecordsToReturn = -1);
	}


	/// <summary>
	/// Base Repo Class
	/// </summary>
	/// <typeparam name="TBusiness"></typeparam>
	/// <typeparam name="TInfoBusiness"></typeparam>
	/// <typeparam name="TModel"></typeparam>
	/// <typeparam name="TModelInterface"></typeparam>
    public abstract partial class CrudRepository<TBusiness, TInfoBusiness, TModel, TModelInterface>
		where TBusiness : IBusinessBase
		where TInfoBusiness : IReadOnlyBase
		where TModelInterface : IViewModel
		where TModel : class, IViewModel, TModelInterface
	{
		//protected readonly BattleSimDbContext _context;
		//protected readonly DbSet<T> _dbSet;

		//public CrudRepository(BattleSimDbContext context)
		//{
		//    _context = context;
		//    _dbSet = context.Set<T>();
		//}

		//public async Task<IEnumerable<T>> GetAllAsync()
		//{
		//    return await _dbSet.ToListAsync();
		//}

		//public  async Task<T> GetByIdAsync(int id)
		//{
		//    return await _dbSet.FindAsync(id);
		//}

		//public async Task AddAsync(T entity)
		//{
		//    await _dbSet.AddAsync(entity);
		//    await _context.SaveChangesAsync();
		//}

		//public async Task UpdateAsync(T entity)
		//{
		//    _dbSet.Update(entity);
		//    await _context.SaveChangesAsync();
		//}

		//public async Task DeleteAsync(int id)
		//{
		//    var entity = await _dbSet.FindAsync(id);
		//    if (entity != null)
		//    {
		//        _dbSet.Remove(entity);
		//        await _context.SaveChangesAsync();
		//    }
		//}

		#region Public Virtual Methods

		public virtual TModelInterface CreateEntity(ODataQueryOptions<DynamicPoco> queryOptions)
		{
			var cslaResults = Factory.ObjectFactory.New<TBusiness>();
			var vm = GetModelInstance();
			CopyFromCslaObject(cslaResults, vm, vm.GetEntityModel());
			return vm;
		}

		public virtual bool DeleteEntity(ODataQueryOptions<DynamicPoco> queryOptions)
		{
			var id = queryOptions.GetIdValueForContracts<TBusiness>();
			Factory.ObjectFactory.ExecuteStaticMethod<TBusiness>("Delete", id);
			return true;
		}

		public virtual DbsResult<TModelInterface> GetEntity(ODataQueryOptions<DynamicPoco> queryOptions)
		{
			var id = queryOptions.GetIdValueForContracts<TBusiness>();
			return GetEntityById(id);
		}
		protected DbsResult<IEnumerable<TModelInterface>> GetInfoListPaged<TInfoPagedBusiness>(
			ODataQueryOptions<DynamicPoco> queryOptions, bool showAll, int maxRecordsToReturn = -1)
		{
			var query = queryOptions.ToEntityPagedQuery(typeof(TModel), showAll, maxRecordsToReturn);

			if (showAll)
			{
				query.PrefetchProperties = PrefetchPropertyList.Hierarchy;
			}

			UpdateQueryForInfoPaged(query);

			var cslaResult = (dynamic)ObjectFactory.Get<TInfoPagedBusiness>(query);
			var entityModel = GetModelInstance().GetEntityModel();
			var vmList = new List<TModel>();
			foreach (var cslaModel in (IList)cslaResult.List)
			{
				var vm = GetModelInstance();
				CopyFromCslaObject(cslaModel, vm, entityModel);
				vmList.Add(vm);
			}

			return new NxResult<IEnumerable<TModelInterface>>
			{
				InlineCount =
					maxRecordsToReturn == -1
						? cslaResult.TotalRows
						: Math.Max(maxRecordsToReturn, cslaResult.TotalRows),
				Result = vmList
			};
		}
		#endregion

		#region Protected methods
		public virtual DbsResult<TModelInterface> GetEntityById(object id, Dictionary<Guid, InternalKeyMapping> keyDefinitions = null)
		{
			var cslaResults = Factory.ObjectFactory.Get<TBusiness>(SetIdByPropType<TBusiness>(id));
			var vm = GetModelInstance();
			var entityModel = vm.GetEntityModel();
			CopyFromCslaObject(cslaResults, vm, entityModel, keyDefinitions);
			return new DbsResult<TModelInterface> { Result = vm, InlineCount = 1 };
		}

		protected TModel GetModelInstance()
		{
			return (TModel)Activator.CreateInstance(typeof(TModel));
		}

		protected T GetModelInstance<T>()
		{
			return (T)Activator.CreateInstance(typeof(T));
		}

		protected T GetModelInstanceByInterface<T>(EntityModel entityModel)
		{
			var modelType = entityModel.Entities.FirstOrDefault(p => typeof(T).IsAssignableFrom(p.EntityType))?.EntityType;
			return (T)Activator.CreateInstance(modelType);
		}

		protected void CopyFromCslaObject(object cslaSource, IViewModel destination, EntityModel entityModel, Dictionary<Guid, InternalKeyMapping> keyDefinitions = null)
		{
			var isInfoModel = destination is IInfoViewModel;
			// If any this null throw an exception
			if (cslaSource == null || destination == null)
				throw new Exception("Source or/and Destination Objects are null");
			// Getting the Types of the objects
			var typeDest = destination.GetType();
			var typeSrc = cslaSource.GetType();
			var isAuditModel = false;
			var srcProps = typeSrc.GetProperties();
			if (!(destination is ILookup))
			{
				destination.RowId = (Guid)typeSrc.GetProperty("RowId")?.GetValue(cslaSource);
				destination._InternalId = (keyDefinitions != null && keyDefinitions.ContainsKey(destination.RowId)) ? keyDefinitions[destination.RowId].TempKey : destination.RowId;
			}

			foreach (var srcProp in srcProps)
			{
				if (srcProp.Name == "RowId")
				{
					continue;
				}

				if (!srcProp.CanRead)
				{
					continue;
				}

				var targetProperty = typeDest.GetProperty(srcProp.Name);
				if (targetProperty == null)
				{
					continue;
				}

				if (!targetProperty.CanWrite)
				{
					continue;
				}
				if (srcProp.PropertyType.IsEnum && (targetProperty.PropertyType == (typeof(string))))
				{
					var sourceValue = !isInfoModel ? ((int)srcProp.GetValue(cslaSource, null)).ToString() : (srcProp.GetValue(cslaSource, null)).ToString();
					if (sourceValue == "-1")
					{
						sourceValue = null;
					}
					targetProperty.SetValue(destination, sourceValue, null);
					continue;
				}

				if ((typeof(IViewModel)).IsAssignableFrom(targetProperty.PropertyType) &&
					((typeof(IReadOnlyBase)).IsAssignableFrom(srcProp.PropertyType)
						|| (typeof(InterfaceLookupBase)).IsAssignableFrom(srcProp.PropertyType)))
				{
					if (isInfoModel)
					{
						targetProperty.SetValue(destination, null, null);
						continue;
					}
					var vmType =
						entityModel.Entities.FirstOrDefault(p => targetProperty.PropertyType.IsAssignableFrom(p.EntityType));
					var vm = (IViewModel)Activator.CreateInstance(vmType.EntityType);

					var srcValue = srcProp.GetValue(cslaSource, null);
					if (srcValue != null)
					{
						CopyFromCslaObject(srcProp.GetValue(cslaSource, null), vm, entityModel);
					}
					targetProperty.SetValue(destination, vm, null);
					typeDest.GetProperty($"_Internal{targetProperty.Name}Id")?.SetValue(destination, vm._InternalId);

					continue;
				}

				if (IsGenericList(targetProperty.PropertyType) && IsGenericList(srcProp.PropertyType))
				{
					var typeForList = targetProperty.PropertyType.GenericTypeArguments[0];
					var vmType = entityModel.Entities.FirstOrDefault(p => typeForList.IsAssignableFrom(p.EntityType));
					var modeList =
						(InstantiateGenericListByType(destination, targetProperty.Name, typeForList) as IList);
					var cslaList = (dynamic)srcProp.GetValue(cslaSource, null);
					if (cslaList != null)
					{
						foreach (var cslaObject in cslaList)
						{
							var vm = (IViewModel)Activator.CreateInstance(vmType.EntityType);
							CopyFromCslaObject(cslaObject, vm, entityModel, keyDefinitions);
							((dynamic)vm)._InternalUnifiedParentId = destination._InternalId;

							modeList.Add(vm);
						}
					}
					targetProperty.SetValue(destination, modeList, null);
					//AddItemToCSLAListByReflectionCopy(destination, targetProperty.Name, modeList);
					continue;
				}

				if (targetProperty.GetSetMethod(true) != null && targetProperty.GetSetMethod(true).IsPrivate)
				{
					continue;
				}

				if ((targetProperty.GetSetMethod()?.Attributes & MethodAttributes.Static) != 0)
				{
					continue;
				}

				// int is assignable to long in most cases it is Id property
				if (!targetProperty.PropertyType.IsAssignableFrom(srcProp.PropertyType) &&
					!(targetProperty.PropertyType == typeof(long) && srcProp.PropertyType == typeof(int)))
				{
					continue;
				}
				// Passed all tests, lets set the value

				targetProperty.SetValue(destination, srcProp.GetValue(cslaSource, null), null);
			}
			if (isAuditModel)
			{
				var entity = entityModel.GetEntity(destination.GetType().Name);
			}
		}

		#endregion

		private bool IsGenericList(Type type)
		{
			if (type.GetInterfaces().Any(p =>
				p.Name.Contains("INxBusinessListBase") || p.Name.Contains("INxReadOnlyListBase")))
			{
				return true;
			}

			return (type.IsGenericType && (type.GetGenericTypeDefinition() == typeof(List<>)));
		}

		private object InstantiateGenericListByType(object targetResource, string propertyName, Type genericType)
		{
			var listType = targetResource.GetType().GetProperty(propertyName).PropertyType;
			if (IsGenericList(listType) && listType.GetGenericArguments()[0].IsAssignableFrom(genericType))
			{
				var listRef = typeof(List<>);
				var resultListInstance = Activator.CreateInstance(listRef.MakeGenericType(genericType));
				targetResource.GetType().GetProperty(propertyName).SetValue(targetResource, resultListInstance);

				return resultListInstance;
			}

			return null;
		}

		protected object SetIdByPropType<TId>(object id) where TId : IBusinessBase
		{
			return SetIdByPropType(id, typeof(TId));
		}

		private object SetIdByPropType(object id, Type type)
		{
			var idType = type.GetInterfaces()
				.FirstOrDefault(p => p.IsGenericType && p.GetGenericTypeDefinition() == typeof(IEntity<>))
				?.GetProperty("Id")?.PropertyType;
			if (id.GetType() == idType)
			{
				return id;
			}
			return Convert.ChangeType(id,
				idType ?? throw new Exception($"Unable find Id property on type : {type.Name}"));
		}

	}

}
