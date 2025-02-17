using BattleSimulatorAPI.DataLayer.Models;
using Csla;
using System.Diagnostics;
using System.Web.Http.OData.Builder;
using System.Web.Http.OData;
using System.Web.Http;
using System.Web.Http.OData.Query;
using System.Xml.Linq;
using BattleSimulatorAPI.DataLayer.Models.Enums;

namespace BattleSimulatorAPI.Extensions
{
	public static class ODataQueryOptionExtensions
	{

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

		public static ODataQueryOptions CreateDynamicQueryForViemModel(this ODataQueryOptions<DynamicPoco> originalQuery, Type entityClrType)
		{
			var builder = new ODataConventionModelBuilder(GlobalConfiguration.Configuration, true);
			var entityTypeConfiguration = builder.AddEntity(entityClrType);
			builder.AddEntitySet(entityClrType.Name, entityTypeConfiguration);

			var edmModel = builder.GetEdmModel();

			var queryContext = new ODataQueryContext(edmModel, entityClrType);
			var actualQueryOptions = new ODataQueryOptions(queryContext, originalQuery.Request);

			return actualQueryOptions;
		}

		public static string GetValueFromPropertyName(this ODataQueryOptions query, string propertyName)
		{
			var internalConditionSet = GetInternalConditionSet(query.ToAbstractQuery());

			if (internalConditionSet == null || !internalConditionSet.Any())
				return null;

			var propertyCondition = internalConditionSet.First(c => c.PropertyName == propertyName);
			return propertyCondition.Value.ToString();
		}

		public static object GetIdValueForContracts<T>(this ODataQueryOptions<DynamicPoco> query) where T : IBusinessBase
		{
			Type targetType = typeof(T);
			//  Contracts inherit from INXBusinessBase<T> which have the Id property and Odata Cannot Read from base class properties 
			if (targetType.GetInterfaces().Any(q => q.Name.Contains("INxBusinessBase")))
			{
				targetType = targetType.GetInterfaces().FirstOrDefault(q => q.Name.Contains("INxBusinessBase"));
			}


			var actualQuery = query.CreateDynamicQueryForViemModel(targetType);
			var id = GetValueFromPropertyName(actualQuery, "Id");
			Debug.Assert(targetType != null, nameof(targetType) + " != null");
			var idProp = targetType.GetProperty("Id");

			if (string.IsNullOrEmpty(id))
				throw new ArgumentException("Invalid ID supplied.", nameof(query));

			Debug.Assert(idProp != null, nameof(idProp) + " != null");
			var idType = idProp.PropertyType;

			if (idType == typeof(short))
				return short.Parse(id);

			if (idType == typeof(int))
				return int.Parse(id);

			if (idType == typeof(long))
				return long.Parse(id);

			throw new ArgumentException("Invalid ID supplied.", nameof(query));
		}

		private static IList<SimpleQueryCondition> GetInternalConditionSet(AbstractQuery query)
		{
			var conditionSet = new List<SimpleQueryCondition>();

			if (string.IsNullOrEmpty(query.WhereConditionsXml)) return conditionSet;

			var xdoc = XDocument.Parse(query.WhereConditionsXml);

			var conditionSetElement = xdoc.Descendants("condition-set").ElementAtOrDefault(0);

			if (conditionSetElement == null) return conditionSet;

			conditionSet.AddRange(conditionSetElement.Descendants("comparison").Select(GetInternalCondition));

			return conditionSet;
		}

		private static SimpleQueryCondition GetInternalCondition(XElement conditionElement)
		{
			var currentConditionJoiner = SimpleQueryConditionJoiner.None;

			var elementBeforeThis = conditionElement.PreviousNode as XElement;
			if (elementBeforeThis != null && elementBeforeThis.Name.LocalName.ToLowerInvariant() == "condition-add")
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

			return leftPartFunctionElement == null
				? GetPropertyCondition(leftPartElement, rightPartElement, conditionElement, currentConditionJoiner)
				: GetFunctionCondition(leftPartFunctionElement, currentConditionJoiner);
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

		private static string GetAbstractQueryConstantValue(XElement parentElement)
		{
			var constantElement = parentElement.Descendants("constant").ElementAtOrDefault(0);
			if (constantElement == null)
			{
				throw new ArgumentException("The supplied parent element of the abstract query does not contain a constant declaration.");
			}

			return constantElement.Value;
		}

		private static AbstractQuery ToAbstractQuery(this ODataQueryOptions query)
		{
			var abstractQuery = new AbstractQuery
			{
				TargetTypeName = query.Context.ElementClrType.Name,
				WhereConditionsXml = ToString(query.Filter),
				OrderByExpressionsXml = ToString(query.OrderBy)
			};

			if (query.Top == null) return abstractQuery;

			var skipValue = query.Skip?.Value ?? 0;

			abstractQuery.PageSize = query.Top.Value;
			abstractQuery.PageNumber = query.Top.Value == 0 ? 1 : skipValue / query.Top.Value + 1;

			return abstractQuery;
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
					throw new ArgumentException($"Unsuppored abstract query operatory type: {operatorString}");
			}
		}

		private static string ToString(OrderByQueryOption orderByQuery)
		{
			return XmlOrderByBinder.BindOrderByQueryOption(orderByQuery);
		}
		private static string ToString(FilterQueryOption filterQuery)
		{
			return XmlFilterBinder.BindFilterQueryOption(filterQuery);
		}
	}
}
