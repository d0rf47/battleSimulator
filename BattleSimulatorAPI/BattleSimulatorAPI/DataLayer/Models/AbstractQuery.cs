using BattleSimulatorAPI.DataLayer.Models.Enums;
using Microsoft.Data.SqlClient;
using System.Xml.Linq;

namespace BattleSimulatorAPI.DataLayer.Models
{
	public static class AbstractQueryExtensions
	{
		private static readonly Dictionary<SimpleQueryOperatorType, FilterOperation> InternalToCslaQueryOperationMap = new Dictionary<SimpleQueryOperatorType, FilterOperation>
		{
			{SimpleQueryOperatorType.Contains, FilterOperation.Contains} ,
			{SimpleQueryOperatorType.EndsWith, FilterOperation.EndsWith},
			{SimpleQueryOperatorType.Equals, FilterOperation.Exact},
			{SimpleQueryOperatorType.GreatThan, FilterOperation.GreaterThanOrEqual},
			{SimpleQueryOperatorType.GreatThanEqual, FilterOperation.GreaterThanOrEqual},
			{SimpleQueryOperatorType.LessThan, FilterOperation.LessThanOrEqual},
			{SimpleQueryOperatorType.LessThanEqual, FilterOperation.LessThanOrEqual},
			{SimpleQueryOperatorType.StartsWith, FilterOperation.StartsWith},
			{SimpleQueryOperatorType.NotEqual, FilterOperation.NotEqual},
			{SimpleQueryOperatorType.In, FilterOperation.In},
		};
		private static readonly Dictionary<Type, string> ClrToCslaDataTypeMap = new Dictionary<Type, string>
		{
			{typeof(string), "string"} ,
			{typeof(Int16), "int"} ,
			{typeof(Int32), "int"} ,
			{typeof(Int64), "int"} ,
			{typeof(double), "numeric"} ,
			{typeof(decimal), "numeric"} ,
			{typeof(float), "numeric"} ,
			{typeof(Guid), "string"},
			{typeof(DateTime), "datetime"},
			{typeof(bool), "Edm.Boolean"}
		};
		private static readonly Dictionary<string, Type> EdmToClrDataTypeMap = new Dictionary<string, Type>
		{
			{"Edm.Int16", typeof(int)},
			{"Edm.Int32", typeof(int)} ,
			{"Edm.Int64", typeof(long)},
			{"Edm.String", typeof(string)},
			{"Edm.Guid", typeof(Guid)},
			{"Edm.DateTime", typeof(DateTime)},
			{"Edm.Float", typeof(float) },
			{"Edm.Decimal", typeof(decimal) },
			{"Edm.Boolean", typeof(bool) }
		};

		/// <summary>
		/// Transforms an <see cref="AbstractQuery"/> into an <see cref="EntityPagedQuery"/>
		/// </summary>
		/// <param name="abstractQuery">The <see cref="AbstractQuery"/> to transform</param>
		/// <returns></returns>
		public static EntityPagedQuery GetCslaPagedQuery(this AbstractQuery abstractQuery)
		{
			var cslaQuery = new EntityPagedQuery
			{
				Filter = GetCslaFilterList(abstractQuery),
				Sort = GetCslaSortParameterList(abstractQuery),
				PageNumber = abstractQuery.PageNumber,
				RowsPerPage = abstractQuery.PageSize,
				HasMoreRows = abstractQuery.HasMoreRows,
				ShowDisabled = abstractQuery.PageSize == 0
			};

			return cslaQuery;
		}

		public static EntityPagedQuery GetCslaPagedQueryBy(this AbstractQuery abstractQuery, string suffix, string entityName, IReadOnlyDictionary<string, object> parameters)
		{
			return new EntityPagedQuery(suffix, entityName, parameters)
			{
				Filter = GetCslaFilterList(abstractQuery),
				Sort = GetCslaSortParameterList(abstractQuery),
				PageNumber = abstractQuery.PageNumber,
				RowsPerPage = abstractQuery.PageSize,
				HasMoreRows = abstractQuery.HasMoreRows,
				ShowDisabled = abstractQuery.PageSize == 0
			};
		}

		/// <summary>
		/// Transforms an <see cref="AbstractQuery"/> into an <see cref="EntityLookupPagedQuery"/>
		/// </summary>
		/// <param name="abstractQuery">The <see cref="AbstractQuery"/> to transform</param>
		/// <returns></returns>
		public static IEntityLookupPagedQuery GetCslaLookupPagedQuery(this AbstractQuery abstractQuery)
		{
			var cslaQuery = new EntityLookupPagedQuery
			{
				Filter = GetCslaFilterList(abstractQuery),
				Sort = GetCslaSortParameterList(abstractQuery),
				PageNumber = abstractQuery.PageNumber,
				RowsPerPage = abstractQuery.PageSize,
				ShowAll = abstractQuery.PageSize == 0
			};

			return cslaQuery;
		}

		/// <summary>
		/// Transforms an <see cref="AbstractQuery"/> into an <see cref="EntityLookupPagedQuery"/>
		/// </summary>
		/// <param name="abstractQuery">The <see cref="AbstractQuery"/> to transform</param>
		/// <param name="suffix">suffix for SP</param>
		/// <param name="parameters">parameters</param>
		/// <returns></returns>
		public static IEntityLookupPagedQuery GetCslaLookupPagedQueryBy(this AbstractQuery abstractQuery, string suffix, IReadOnlyDictionary<string, object> parameters)
		{
			var cslaQuery = EntityLookupPagedQuery.By(suffix, parameters);

			cslaQuery.Filter = GetCslaFilterList(abstractQuery);
			cslaQuery.Sort = GetCslaSortParameterList(abstractQuery);
			cslaQuery.PageNumber = abstractQuery.PageNumber;
			cslaQuery.RowsPerPage = abstractQuery.PageSize;
			cslaQuery.ShowAll = abstractQuery.PageSize == 0;

			return cslaQuery;
		}

		/// <summary>
		/// Transforms an <see cref="AbstractQuery"/> into an <see cref="EntityLookupPagedQuery"/>
		/// </summary>
		/// <param name="abstractQuery">The <see cref="AbstractQuery"/> to transform</param>
		/// <param name="suffix">suffix for SP</param>
		/// <param name="parameters">parameters</param>
		/// <returns></returns>
		public static IEntityLookupPagedQuery GetCslaLookupPagedQueryFor(this AbstractQuery abstractQuery, string suffix, IReadOnlyDictionary<string, object> parameters)
		{
			var cslaQuery = EntityLookupPagedQuery.For(suffix, parameters);

			cslaQuery.Filter = GetCslaFilterList(abstractQuery);
			cslaQuery.Sort = GetCslaSortParameterList(abstractQuery);
			cslaQuery.PageNumber = abstractQuery.PageNumber;
			cslaQuery.RowsPerPage = abstractQuery.PageSize;
			cslaQuery.ShowAll = abstractQuery.PageSize == 0;

			return cslaQuery;
		}

		#region Query Helper Methods
		private static SortParameterList GetCslaSortParameterList(AbstractQuery query)
		{
			var sortList = new SortParameterList();

			var internalSortSet = GetInternalSortSet(query);

			sortList.AddRange(internalSortSet.Select(GetCslaSortClauseFromInternalSortClause));

			return sortList;
		}

		private static FilterParameterList GetCslaFilterList(AbstractQuery query)
		{
			var paramList = new FilterParameterList();

			var internalConditionSet = GetInternalConditionSet(query);

			CombineDuplicatePredicates((List<SimpleQueryCondition>)internalConditionSet);

			var currentFilterGroupId = 0;

			foreach (var internalCondition in internalConditionSet)
			{
				if (internalCondition.Joiner == SimpleQueryConditionJoiner.And)
				{
					currentFilterGroupId++;
				}
				paramList.Add(GetCslaFilterParameterFromInternalCondition(internalCondition, currentFilterGroupId));
			}

			return paramList;
		}

		private static void CombineDuplicatePredicates(List<SimpleQueryCondition> conditionSet)
		{
			var duplicates = conditionSet
				.Where(x => x.OperatorType == SimpleQueryOperatorType.Contains ||
					x.OperatorType == SimpleQueryOperatorType.Equals)
				.GroupBy(x => x.PropertyName)
				.Where(y => y.Count() > 1)
				.Select(z => z.Key);
			foreach (var duplicate in duplicates)
			{
				List<SimpleQueryCondition> selectedSet = conditionSet.Where(x => x.PropertyName == duplicate).ToList<SimpleQueryCondition>();
				selectedSet[0].OperatorType = SimpleQueryOperatorType.In;
				selectedSet[0].Value = string.Join(",", selectedSet.Select(x => x.Value));
				conditionSet.RemoveAll(x => x.PropertyName == duplicate);

				//If duplicated predicate is first one, then add it in first of the list itself else add it in the end for condition to work properly
				if (selectedSet[0].Joiner == 0)
				{
					conditionSet.Insert(0, selectedSet[0]);
				}
				else
				{
					conditionSet.Add(selectedSet[0]);
				}
			}
		}

		private static IList<SimpleQueryCondition> GetInternalConditionSet(AbstractQuery query)
		{

			var conditionSet = new List<SimpleQueryCondition>();

			if (String.IsNullOrEmpty(query.WhereConditionsXml)) return conditionSet;

			var xdoc = XDocument.Parse(query.WhereConditionsXml);

			var conditionSetElement = xdoc.Descendants("condition-set").ElementAtOrDefault(0);

			if (conditionSetElement == null) return conditionSet;

			conditionSet.AddRange(conditionSetElement.Descendants("comparison").Select(GetInternalCondition));

			return conditionSet;
		}

		private static FilterParameter GetCslaFilterParameterFromInternalCondition(SimpleQueryCondition condition, int cslaFilterGroupId = 0)
		{
			var filterParam = new FilterParameter
			{
				Operation = InternalToCslaQueryOperationMap[condition.OperatorType],
				Datatype = ClrToCslaDataTypeMap[condition.DataType],
				FieldName = condition.PropertyName,
				FilterGroupId = cslaFilterGroupId,
				PrimaryValue = condition.Value.ToString()
			};

			if (filterParam.Operation == FilterOperation.Contains && (filterParam.PrimaryValue.StartsWith("*") || filterParam.PrimaryValue.EndsWith("*")))
			{
				if (filterParam.PrimaryValue.StartsWith("*"))
				{
					filterParam.Operation = FilterOperation.EndsWith;
					filterParam.PrimaryValue = filterParam.PrimaryValue.Remove(0, 1);
				}
				else
				{
					filterParam.Operation = FilterOperation.StartsWith;
					var lastIndexOfAsterisk = filterParam.PrimaryValue.Length - 1;
					filterParam.PrimaryValue = filterParam.PrimaryValue.Remove(lastIndexOfAsterisk, 1);
				}
			}
			return filterParam;
		}

		private static SimpleQueryCondition GetInternalCondition(XElement conditionElement)
		{
			var currentConditionJoiner = SimpleQueryConditionJoiner.None;

			if (conditionElement.PreviousNode is XElement elementBeforeThis && elementBeforeThis.Name.LocalName.ToLowerInvariant() == "condition-add")
			{
				var conditionJoinerString = elementBeforeThis.Value.ToLowerInvariant();
				currentConditionJoiner = conditionJoinerString == "or"
					? SimpleQueryConditionJoiner.Or
					: conditionJoinerString == "and"
					? SimpleQueryConditionJoiner.And
					: SimpleQueryConditionJoiner.None;
			}

			var leftPartElement = conditionElement.Descendants("left").ElementAtOrDefault(0);
			var rightPartElement = conditionElement.Descendants("right").ElementAtOrDefault(0);

			if (leftPartElement == null)
			{
				throw new ArgumentException("Left part of the abstract query condition element cannot be null.");
			}

			if (rightPartElement == null)
			{
				throw new ArgumentException("Right part of the query condition element cannot be null");
			}


			var leftPartFunctionElement = leftPartElement.Descendants("function").ElementAtOrDefault(0);
			if (leftPartFunctionElement == null)
			{
				return GetPropertyCondition(leftPartElement, rightPartElement, conditionElement, currentConditionJoiner);
			}
			return GetFunctionCondition(leftPartFunctionElement, currentConditionJoiner);
		}

		private static SimpleQueryCondition GetPropertyCondition(XElement leftPartElement, XElement rightPartElement, XElement parentElement, SimpleQueryConditionJoiner joiner)
		{
			return new SimpleQueryCondition
			{
				PropertyName = GetAbstractQueryPropertyName(leftPartElement),
				Value = GetAbstractQueryConstantValue(rightPartElement),
				Joiner = joiner,
				OperatorType = GetAbstractQueryOperator(parentElement),
				DataType = GetAbstractQueryPropertyType(leftPartElement)
			};

		}

		private static Type GetAbstractQueryPropertyType(XElement parentElement)
		{
			var propertyElement = parentElement.Descendants("property").ElementAtOrDefault(0);
			if (propertyElement == null)
			{
				throw new ArgumentException("The supplied parent element of the abstract query does not contain a property declaration.");
			}

			var propertyEdmTypeNameElement = propertyElement.Descendants("type").ElementAtOrDefault(0);
			if (propertyEdmTypeNameElement == null)
			{
				throw new ArgumentException("The supplied parent element of the abstract query does not specify a valid property type.");
			}

			if (!EdmToClrDataTypeMap.ContainsKey(propertyEdmTypeNameElement.Value))
			{
				throw new ArgumentException(
					$"Unable to map abstract query type: {propertyEdmTypeNameElement.Value} to a native CLR type.");
			}

			return EdmToClrDataTypeMap[propertyEdmTypeNameElement.Value];
		}

		private static string GetAbstractQueryConstantValue(XElement parentElement)
		{
			var constantElement = parentElement.Descendants("constant").ElementAtOrDefault(0);
			if (constantElement == null)
			{
				throw new ArgumentException("The supplied parent element of the abstract query does not contain a constant declaration.");
			}

			return constantElement.Value;
		}

		private static SimpleQueryOperatorType GetAbstractQueryOperator(XElement parentElement)
		{

			var operatorElement = parentElement.Descendants("operator").ElementAtOrDefault(0);
			if (operatorElement == null)
			{
				throw new ArgumentException("The supplied parent element of the abstract query does not contain an operator declaration.");
			}

			var operatorString = operatorElement.Value;
			if (String.IsNullOrEmpty(operatorString))
			{
				throw new ArgumentException("The supplied parent element of the abstract query contains an empty operator declaration.");

			}

			return GetAbstractQueryOperator(operatorString);
		}

		private static string GetAbstractQueryPropertyName(XElement parentElement)
		{
			var propertyElement = parentElement.Descendants("property").ElementAtOrDefault(0);
			if (propertyElement == null)
			{
				throw new ArgumentException("The supplied parent element of the abstract query does not contain a property declaration.");
			}

			var propertyNameElement = propertyElement.Descendants("name").ElementAtOrDefault(0);
			if (propertyNameElement == null)
			{
				throw new ArgumentException("The supplied parent element of the abstract query does not have a valid property name.");
			}

			return propertyNameElement.Value;
		}

		private static SimpleQueryCondition GetFunctionCondition(XElement functionElement, SimpleQueryConditionJoiner joiner)
		{
			//assume property name first arg, constant second arg
			var functionName = functionElement.Attribute("name")?.Value;
			if (string.IsNullOrEmpty(functionName))
			{
				throw new ArgumentException("Function name specified in the abstract query cannot be null or empty string.");
			}

			var functionArgumentElements = functionElement.Descendants("argument").ToArray();
			if (functionArgumentElements == null)
			{
				throw new ArgumentException($"No arguments are specified for the abstract query function: {functionName}");
			}


			var firstArg = functionArgumentElements.ElementAtOrDefault(0);
			var secondArg = functionArgumentElements.ElementAtOrDefault(1);

			return new SimpleQueryCondition
			{
				PropertyName = GetAbstractQueryPropertyName(firstArg),
				DataType = GetAbstractQueryPropertyType(firstArg),
				Value = GetAbstractQueryConstantValue(secondArg),
				Joiner = joiner,
				OperatorType = GetAbstractQueryOperatorFromFunctionName(functionName)
			};


		}

		private static SimpleQueryOperatorType GetAbstractQueryOperatorFromFunctionName(string functionName)
		{
			return GetAbstractQueryOperator(functionName);
		}

		private static SimpleQueryOperatorType GetAbstractQueryOperator(string operatorString)
		{

			switch (operatorString.ToLowerInvariant())
			{
				case "=":
					return SimpleQueryOperatorType.Equals;

				case "startswith":
					return SimpleQueryOperatorType.StartsWith;

				case "endswith":
					return SimpleQueryOperatorType.EndsWith;

				case "contains":
					return SimpleQueryOperatorType.Contains;

				case "in":
					return SimpleQueryOperatorType.In;

				case "like":
					return SimpleQueryOperatorType.Contains;

				case ">=":
				case "ge":
				case "GreaterThanOrEqual":
					return SimpleQueryOperatorType.GreatThanEqual;


				case "<=":
				case "le":
				case "LessThanOrEqual":
					return SimpleQueryOperatorType.LessThanEqual;

				case "!=":
					return SimpleQueryOperatorType.NotEqual;

				default:
					throw new ArgumentException($"Unsupported abstract query operator type: {operatorString}");
			}
		}
		#endregion

		#region "Internal sort set helper methods"
		private static SortParameter GetCslaSortClauseFromInternalSortClause(SimpleQuerySortClause internalSort)
		{
			return new SortParameter
			{
				FieldName = internalSort.PropertyName,
				SortOrder = internalSort.SortDirection == SimpleQuerySortClauseDirection.Ascending ? SortOrder.Asc : SortOrder.Desc
			};
		}

		private static IList<SimpleQuerySortClause> GetInternalSortSet(AbstractQuery query)
		{
			var sortSet = new List<SimpleQuerySortClause>();

			if (String.IsNullOrEmpty(query.OrderByExpressionsXml)) return sortSet;

			var xdoc = XDocument.Parse(query.OrderByExpressionsXml);

			var sortSetElement = xdoc.Descendants("sort-set").ElementAtOrDefault(0);

			if (sortSetElement == null) return sortSet;

			sortSet.AddRange(sortSetElement.Descendants("sort").Select(GetInternalSort));

			return sortSet;
		}

		private static SimpleQuerySortClause GetInternalSort(XElement sortElement)
		{
			return new SimpleQuerySortClause
			{
				PropertyName = GetAbstractQuerySortPropertyName(sortElement),
				SortDirection = GetAbstractQuerySortDirection(sortElement)
			};
		}

		private static string GetAbstractQuerySortPropertyName(XElement parentElement)
		{
			var propertyElement = parentElement.Descendants("property").ElementAtOrDefault(0);
			if (propertyElement == null)
			{
				throw new ArgumentException("No property is specified for the sort expression in the abstract query.");
			}

			return propertyElement.Value;
		}

		private static SimpleQuerySortClauseDirection GetAbstractQuerySortDirection(XElement parentElement)
		{
			var directionElement = parentElement.Descendants("direction").ElementAtOrDefault(0);
			if (directionElement == null)
			{
				throw new ArgumentException("No sort direction is specified for the sort expression in the abstract query.");
			}

			var directionName = directionElement.Value;
			if (String.IsNullOrEmpty(directionName))
			{
				throw new ArgumentException("Empty sort direction is specified for the sort expression in the abstract query.");
			}

			directionName = directionName.ToLowerInvariant();
			return directionName == "asc"
				? SimpleQuerySortClauseDirection.Ascending
				: SimpleQuerySortClauseDirection.Descending;

		}
		#endregion
	}
	public class AbstractQuery
	{
		public string TargetTypeName { get; set; }

		public string WhereConditionsXml { get; set; }

		public string OrderByExpressionsXml { get; set; }

		public int PageSize { get; set; }

		public int PageNumber { get; set; }

		public bool HasMoreRows { get; set; }
	}
}
