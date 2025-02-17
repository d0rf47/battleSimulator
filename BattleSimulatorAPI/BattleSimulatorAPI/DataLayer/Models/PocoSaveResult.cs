using Microsoft.Data.OData;
using System.Dynamic;
using System.Xml.Serialization;

namespace BattleSimulatorAPI.DataLayer.Models
{
	public class PocoSaveResult
	{
		public IList<IPoco> RefreshedPocoObjects { get; set; }
		//public IList<EntityValidationMessage> ValidationMessages { get; set; }

		/// <summary>
		/// Basic constructor
		/// </summary>
		public PocoSaveResult()
		{
			RefreshedPocoObjects = new List<IPoco>();
			//ValidationMessages = new List<EntityValidationMessage>();
		}

		/// <summary>
		/// Updates all validation messages containing a reference to a POCO
		/// </summary>
		/// <param name="originatingPoco">The used to identify which messages to update</param>
		/// <param name="refreshedPoco">The POCO to add to the validation message</param>
		//public void UpdateWithRefreshedPoco(IPoco originatingPoco, IPoco refreshedPoco)
		//{
		//	foreach (var validationMessage in ValidationMessages.Where(m => m.OriginatingPoco == originatingPoco))
		//	{
		//		validationMessage.RefreshedPoco = refreshedPoco;
		//	}
		//}
	}

	public class PocoBase : IPoco
	{

		[XmlIgnore]
		public virtual Int64 Id { get; set; }

		[XmlIgnore]	
		public Guid _InternalId { get; set; }

		//[XmlIgnore]
		//public byte[] Concurrency { get; set; }

		//[XmlIgnore]		
		//public IEnumerable<EntityRule> Rules { get; protected set; }

		//[XmlIgnore]
		//[NxExportIgnore]
		//public DataOperation DataOperation { get; set; }

		public PocoBase()
		{
			//this.DataOperation = new DataOperation();
			this.Id = 0;
			//this.Concurrency = BitConverter.GetBytes(1);
			this._InternalId = Guid.NewGuid();
		}


	}

	public interface IPoco
	{
		Int64 Id { get; set; }
		//[NxExportIgnore]
		Guid _InternalId { get; set; }
		//byte[] Concurrency { get; set; }
		//[NxExportIgnore]
		//IEnumerable<EntityRule> Rules { get; }
		//[NxExportIgnore]
		//DataOperation DataOperation { get; set; }
	}

	public class DynamicPoco : DynamicObject
	{

		public override bool TryGetMember(
			GetMemberBinder binder, out object result)
		{

			var name = binder.Name.ToLower();

			// If the property name is found in a dictionary,
			// set the result parameter to the property value and return true.
			// Otherwise, return false.
			result = new object();
			return true;
		}
	}
}
