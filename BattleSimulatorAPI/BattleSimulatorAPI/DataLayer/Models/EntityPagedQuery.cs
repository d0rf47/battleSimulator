using BattleSimulatorAPI.DataLayer.Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace BattleSimulatorAPI.DataLayer.Models
{
	/// <summary>
	/// Interface for Entity Info Paged Query
	/// </summary>
	public interface IEntityInfoPagedQuery
	{
		/// <summary>
		/// Gets/sets Page Number to fetch rows for
		/// </summary>
		int PageNumber { get; set; }

		/// <summary>
		/// Gets/sets maximum number of rows on a single page
		/// </summary>
		int RowsPerPage { get; set; }

		/// <summary>
		/// Gets/sets Has More Rows flag
		/// </summary>
		bool HasMoreRows { get; set; }

		/// <summary>
		/// Gets/sets list of Parameters to Sort By
		/// </summary>
		SortParameterList Sort { get; set; }

		/// <summary>
		/// Gets/sets list of Parameters to Filter By
		/// </summary>
		FilterParameterList Filter { get; set; }

		/// <summary>
		/// Gets/sets flag (tri-state) to decide whether to show disabled records or not
		/// </summary>
		bool? ShowDisabled { get; set; }

		/// <summary>
		/// Gets/sets flag to decide whether to show System records or not
		/// </summary>
		bool? ShowSystemRecord { get; set; }

		/// <summary>
		/// Gets/sets value indicating number of retries
		/// </summary>
		int RetryCount { get; set; }

		/// <summary>
		/// Gets/sets the type of Result required
		/// </summary>
		PagedResultType ResultType { get; set; }

		/// <summary>
		/// Gets/sets Child List Properties to PreFetch
		/// </summary>
		PrefetchPropertyList PrefetchProperties { get; set; }

		/// <summary>
		/// Gets Data Layer Methods Suffix
		/// </summary>
		/// <returns></returns>
		string GetDataLayerMethodSuffix();

		/// <summary>
		/// Gets the Parameters for Info Query
		/// </summary>
		/// <returns>A dictionary of parameter names and values</returns>
		IReadOnlyDictionary<string, object> GetAdditionalParameters();
	}
	/// <summary>
	/// Base Class for Entity Lookup Paged Query
	/// </summary>
	[Serializable]
	public abstract class EntityInfoPagedQueryBase : IEntityInfoPagedQuery
	{
		private static readonly Dictionary<Type, Dictionary<string, PropertyInfo>> TypeQueryPropertiesCache
			= new Dictionary<Type, Dictionary<string, PropertyInfo>>();

		/// <inheritdoc />
		public int PageNumber { get; set; }

		/// <inheritdoc />
		public int RowsPerPage { get; set; }

		/// <inheritdoc />
		public SortParameterList Sort { get; set; }

		/// <inheritdoc />
		public FilterParameterList Filter { get; set; }

		/// <inheritdoc />
		public bool? ShowDisabled { get; set; }


		/// <inheritdoc />
		public bool HasMoreRows { get; set; }

		/// <inheritdoc />
		public bool? ShowSystemRecord { get; set; }

		/// <inheritdoc />
		public int RetryCount { get; set; }

		/// <inheritdoc />
		public PagedResultType ResultType { get; set; }

		/// <inheritdoc />
		public PrefetchPropertyList PrefetchProperties { get; set; }

		/// <summary>
		/// Creates the EntityInfoPagedQueryBase Object
		/// </summary>
		protected EntityInfoPagedQueryBase()
		{
			PrefetchProperties = new PrefetchPropertyList();
			Filter = new FilterParameterList();
			Sort = new SortParameterList();
			PageNumber = 1;
			RowsPerPage = 0;
			HasMoreRows = false;
			ShowDisabled = true;
			RetryCount = 0;
			ResultType = PagedResultType.Sequential;
		}

		/// <summary>
		/// Overridable Method to get Data Layer Methods Suffix
		/// </summary>
		/// <returns></returns>
		public virtual string GetDataLayerMethodSuffix()
		{
			return string.Empty;
		}

		/// <inheritdoc />
		public virtual IReadOnlyDictionary<string, object> GetAdditionalParameters()
		{
			var parameters = new Dictionary<string, object>();
			var queryProperties = GetQueryProperties();

			foreach (var propName in queryProperties.Keys)
			{
				var prop = queryProperties[propName];
				var propValue = prop.GetValue(this);

				parameters.Add(propName, propValue);
			}

			return parameters;
		}

		private Dictionary<string, PropertyInfo> GetQueryProperties()
		{
			var type = GetType();

			if (TypeQueryPropertiesCache.ContainsKey(type))
				return TypeQueryPropertiesCache[type];

			var collection = new Dictionary<string, PropertyInfo>();
			var queryProps = type
				.GetProperties()
				.Where(p => Attribute.IsDefined(p, typeof(QueryParameterAttribute)));

			foreach (var prop in queryProps)
			{
				var attrib = prop.GetCustomAttribute<QueryParameterAttribute>();
				var name = string.IsNullOrEmpty(attrib.Name) ? prop.Name : attrib.Name;

				collection.Add(name, prop);
			}

			TypeQueryPropertiesCache.Add(type, collection);

			return collection;
		}
	}

	/// <summary>
	/// Query Object for Paged Data
	/// </summary>
	[Serializable]
	public sealed class EntityPagedQuery : EntityInfoPagedQueryBase
	{
		[Serializable]
		private class ChildEntityInfoPagedQuery : EntityInfoPagedQueryBase
		{
			private readonly string _prefix;
			private readonly string _entityName;
			private readonly IReadOnlyDictionary<string, object> _parameters;

			public ChildEntityInfoPagedQuery(string prefix, string entityName, IReadOnlyDictionary<string, object> parameters)
			{
				_prefix = prefix;
				_entityName = entityName;
				_parameters = parameters.ToDictionary(pv => pv.Key, pv => pv.Value);
			}

			public override string GetDataLayerMethodSuffix()
			{
				return $"{_prefix}{_entityName}";
			}

			public override IReadOnlyDictionary<string, object> GetAdditionalParameters()
			{
				return _parameters;
			}
		}

		private readonly string _prefix;
		private readonly string _entityName;
		private readonly IReadOnlyDictionary<string, object> _parameters;

		public EntityPagedQuery()
		{
			_parameters = new Dictionary<string, object>();
		}

		public EntityPagedQuery(string prefix, string entityName, IReadOnlyDictionary<string, object> parameters)
		{
			_prefix = prefix;
			_entityName = entityName;
			_parameters = parameters.IsNullOrEmpty() ? null : parameters.ToDictionary(pv => pv.Key, pv => pv.Value);
		}

		public override string GetDataLayerMethodSuffix()
		{
			return $"{_prefix}{_entityName}";
		}

		public override IReadOnlyDictionary<string, object> GetAdditionalParameters()
		{
			var parameters = new Dictionary<string, object>(base.GetAdditionalParameters().ToDictionary(pv => pv.Key, pv => pv.Value));

			if (_parameters.IsNullOrEmpty() != false) { return parameters; }

			foreach (var parameter in _parameters)
			{
				parameters.Add(parameter.Key, parameter.Value);
			}
			return parameters;
		}

		public static IEntityInfoPagedQuery By(string name, IReadOnlyDictionary<string, object> parameters)
		{
			return new ChildEntityInfoPagedQuery("By", name, parameters);
		}

		public static IEntityInfoPagedQuery For(string name, IReadOnlyDictionary<string, object> parameters)
		{
			return new ChildEntityInfoPagedQuery("For", name, parameters);
		}

		public static EntityPagedQuery Default()
		{
			return new EntityPagedQuery
			{
				Filter = new FilterParameterList(),
				PageNumber = 1,
				PrefetchProperties = PrefetchPropertyList.All,
				RowsPerPage = 10,
				ShowDisabled = false,
				Sort = new SortParameterList()
			};
		}

		public static EntityPagedQuery QueryAll()
		{
			return new EntityPagedQuery
			{
				Filter = new FilterParameterList(),
				PageNumber = 1,
				PrefetchProperties = PrefetchPropertyList.All,
				RowsPerPage = 0,
				ShowDisabled = null,
				Sort = new SortParameterList()
			};
		}
	}
}
