using Newtonsoft.Json.Linq;
using BattleSimulatorAPI.Repositories.Models.Repositories;
using Breeze.WebApi2;
using System.Web.Http;

namespace BattleSimulatorAPI.Controllers
{
    /// <summary>
    /// 
    /// </summary>
    [BreezeController]
    public class BreezeControllerBase : ApiController
    {
        protected virtual string EntityName { get; set; }

        protected BreezeControllerBase(string entityName) 
        {
            EntityName = entityName;
        }        
    }

}
