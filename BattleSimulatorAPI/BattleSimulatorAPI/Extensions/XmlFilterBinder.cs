using Microsoft.Data.Edm;
using Microsoft.Data.OData.Query.SemanticAst;
using Microsoft.Data.OData.Query;
using System.Diagnostics;
using System.Web.Http.OData.Query;
using Microsoft.Data.OData;
using System.Text;

namespace BattleSimulatorAPI.Extensions
{
	public class XmlFilterBinder
	{
		public static string BindFilterQueryOption(FilterQueryOption filterQuery)
		{
			if (filterQuery == null) return "";
			var binder = new XmlFilterBinder();
			return $"<condition-set>{binder.BindFilter(filterQuery)}</condition-set>";
		}

		protected string BindFilter(FilterQueryOption filterQuery)
		{
			return BindFilterClause(filterQuery.FilterClause);
		}

		protected string BindFilterClause(FilterClause filterClause)
		{
			return Bind(filterClause.Expression);
		}

		protected string Bind(QueryNode node)
		{
			var singleValueNode = node as SingleValueNode;

			if (node is CollectionNode)
			{
				switch (node.Kind)
				{
					case QueryNodeKind.CollectionNavigationNode:
						var navigationNode = node as CollectionNavigationNode;
						Debug.Assert(navigationNode != null, nameof(navigationNode) + " != null");
						return BindNavigationPropertyNode(navigationNode.Source, navigationNode.NavigationProperty);

					case QueryNodeKind.CollectionPropertyAccess:
						return BindCollectionPropertyAccessNode(node as CollectionPropertyAccessNode);
				}
			}
			else if (singleValueNode != null)
			{
				switch (node.Kind)
				{
					case QueryNodeKind.BinaryOperator:
						return BindBinaryOperatorNode(node as BinaryOperatorNode);

					case QueryNodeKind.Constant:
						return BindConstantNode(node as ConstantNode);

					case QueryNodeKind.Convert:
						return BindConvertNode(node as ConvertNode);

					case QueryNodeKind.EntityRangeVariableReference:
						return BindRangeVariable((node as EntityRangeVariableReferenceNode)?.RangeVariable);

					case QueryNodeKind.NonentityRangeVariableReference:
						return BindRangeVariable((node as NonentityRangeVariableReferenceNode)?.RangeVariable);

					case QueryNodeKind.SingleValuePropertyAccess:
						return BindPropertyAccessQueryNode(node as SingleValuePropertyAccessNode);

					case QueryNodeKind.UnaryOperator:
						return BindUnaryOperatorNode(node as UnaryOperatorNode);

					case QueryNodeKind.SingleValueFunctionCall:
						return BindSingleValueFunctionCallNode(node as SingleValueFunctionCallNode);

					case QueryNodeKind.SingleNavigationNode:
						var navigationNode = node as SingleNavigationNode;
						Debug.Assert(navigationNode != null, nameof(navigationNode) + " != null");
						return BindNavigationPropertyNode(navigationNode.Source, navigationNode.NavigationProperty);

					case QueryNodeKind.Any:
						return BindAnyNode(node as AnyNode);

					case QueryNodeKind.All:
						return BindAllNode(node as AllNode);
				}
			}

			throw new NotSupportedException($"Nodes of type {node.Kind} are not supported");
		}

		private string BindCollectionPropertyAccessNode(CollectionPropertyAccessNode collectionPropertyAccessNode)
		{
			return Bind(collectionPropertyAccessNode.Source) + "." + collectionPropertyAccessNode.Property.Name;
		}

		private string BindNavigationPropertyNode(SingleValueNode singleValueNode, IEdmNavigationProperty edmNavigationProperty)
		{
			return Bind(singleValueNode) + "." + edmNavigationProperty.Name;
		}

		private string BindAllNode(AllNode allNode)
		{
			var innerQuery = "not exists ( from " + Bind(allNode.Source) + " " + allNode.RangeVariables.First().Name;
			innerQuery += " where NOT(" + Bind(allNode.Body) + ")";
			return innerQuery + ")";
		}

		private string BindAnyNode(AnyNode anyNode)
		{
			var innerQuery = "exists ( from " + Bind(anyNode.Source) + " " + anyNode.RangeVariables.First().Name;
			if (anyNode.Body != null)
			{
				innerQuery += " where " + Bind(anyNode.Body);
			}
			return innerQuery + ")";
		}

		private string BindNavigationPropertyNode(SingleEntityNode singleEntityNode, IEdmNavigationProperty edmNavigationProperty)
		{
			return Bind(singleEntityNode) + "." + edmNavigationProperty.Name;
		}

		private string BindSingleValueFunctionCallNode(SingleValueFunctionCallNode singleValueFunctionCallNode)
		{
			var arguments = singleValueFunctionCallNode.Arguments.ToList();
			switch (singleValueFunctionCallNode.Name)
			{
				case "concat":
					return singleValueFunctionCallNode.Name + "(" + Bind(arguments[0]) + "," + Bind(arguments[1]) + ")";

				case "startswith":
					return
						$"<function name=\"{singleValueFunctionCallNode.Name}\"><argument>{Bind(arguments[0])}</argument><argument>{Bind(arguments[1])}</argument></function>";

				case "endswith":
					return
						$"<function name=\"{singleValueFunctionCallNode.Name}\"><argument>{Bind(arguments[0])}</argument><argument>{Bind(arguments[1])}</argument></function>";

				case "substringof":
					return
						$"<function name=\"contains\"><argument>{Bind(arguments[1])}</argument><argument>{Bind(arguments[0])}</argument></function>";

				case "length":
				case "trim":
				case "year":
				case "years":
				case "month":
				case "months":
				case "day":
				case "days":
				case "hour":
				case "hours":
				case "minute":
				case "minutes":
				case "second":
				case "seconds":
				case "round":
				case "floor":
				case "ceiling":
					return singleValueFunctionCallNode.Name + "(" + Bind(arguments[0]) + ")";
				default:
					throw new ArgumentException($"{singleValueFunctionCallNode.Name} is not a supported single value function.");
			}
		}

		private string BindUnaryOperatorNode(UnaryOperatorNode unaryOperatorNode)
		{
			return ToString(unaryOperatorNode.OperatorKind) + "(" + Bind(unaryOperatorNode.Operand) + ")";
		}

		private string BindPropertyAccessQueryNode(SingleValuePropertyAccessNode singleValuePropertyAccessNode)
		{
			return
				$"<property><name>{singleValuePropertyAccessNode.Property.Name}</name><type>{singleValuePropertyAccessNode.Property.Type.FullName()}</type></property>";
		}

		private string BindRangeVariable(NonentityRangeVariable nonentityRangeVariable)
		{
			return nonentityRangeVariable.Name;
		}

		private string BindRangeVariable(EntityRangeVariable entityRangeVariable)
		{
			return entityRangeVariable.Name;
		}

		private string BindConvertNode(ConvertNode convertNode)
		{
			return Bind(convertNode.Source);
		}

		public string BindConstantNode(ConstantNode constantNode)
		{
			return constantNode.Value == null
				? $"<constant>{new System.Xml.Linq.XText("null")}</constant>"
				: $"<constant>{new System.Xml.Linq.XText(constantNode.Value.ToString())}</constant>";
		}

		private string BindBinaryOperatorNode(BinaryOperatorNode binaryOperatorNode)
		{
			var left = Bind(binaryOperatorNode.Left);
			var right = Bind(binaryOperatorNode.Right);

			if (binaryOperatorNode.OperatorKind == BinaryOperatorKind.And ||
				binaryOperatorNode.OperatorKind == BinaryOperatorKind.Or)
			{
				return
					$"{left}<condition-add>{new System.Xml.Linq.XText(ToString(binaryOperatorNode.OperatorKind))}</condition-add>{right}";
			}

			return
				$"<comparison><left>{left}</left><operator>{new System.Xml.Linq.XText(ToString(binaryOperatorNode.OperatorKind))}</operator><right>{right}</right></comparison>";
		}

		public string ToString(BinaryOperatorKind binaryOpertor)
		{
			switch (binaryOpertor)
			{
				case BinaryOperatorKind.Add:
					return "+";
				case BinaryOperatorKind.And:
					return "AND";
				case BinaryOperatorKind.Divide:
					return "/";
				case BinaryOperatorKind.Equal:
					return "=";
				case BinaryOperatorKind.GreaterThan:
					return ">";
				case BinaryOperatorKind.GreaterThanOrEqual:
					return ">=";
				case BinaryOperatorKind.LessThan:
					return "<";
				case BinaryOperatorKind.LessThanOrEqual:
					return "<=";
				case BinaryOperatorKind.Modulo:
					return "%";
				case BinaryOperatorKind.Multiply:
					return "*";
				case BinaryOperatorKind.NotEqual:
					return "!=";
				case BinaryOperatorKind.Or:
					return "OR";
				case BinaryOperatorKind.Subtract:
					return "-";
				default:
					return null;
			}
		}

		private static string ToString(UnaryOperatorKind unaryOperator)
		{
			switch (unaryOperator)
			{
				case UnaryOperatorKind.Negate:
					return "!";
				case UnaryOperatorKind.Not:
					return "NOT";
				default:
					return null;
			}
		}
	}

	public class XmlOrderByBinder
	{
		public static string BindOrderByQueryOption(OrderByQueryOption orderByQuery)
		{
			var sb = new StringBuilder();

			if (orderByQuery != null)
			{
				sb.Append("<sort-set>");

				foreach (var orderByPropertyNode in orderByQuery.OrderByNodes.Select(orderByNode => orderByNode as OrderByPropertyNode))
				{
					if (orderByPropertyNode != null)
					{
						sb.Append("<sort>");
						sb.Append(string.Format("<property>{0}</property>", orderByPropertyNode.Property.Name));
						sb.Append(string.Format("<direction>{0}</direction>", orderByPropertyNode.Direction == OrderByDirection.Ascending ? "ASC" : "DESC"));
						sb.Append("</sort>");
					}
					else
					{
						throw new ODataException("Only ordering by properties is supported at this time.");
					}
				}



				sb.Append("</sort-set>");
			}



			return sb.ToString();
		}
	}
}
