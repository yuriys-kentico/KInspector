using System;

using KenticoInspector.Core.Instances.Models;
using KenticoInspector.Core.Instances.Services;

using Microsoft.AspNetCore.Mvc;

namespace KenticoInspector.Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class InstancesController : ControllerBase
    {
        private readonly IInstanceService instanceService;

        public InstancesController(IInstanceService instanceService)
        {
            this.instanceService = instanceService;
        }

        [HttpGet("details/{instanceGuid}")]
        public IActionResult Details(Guid instanceGuid) => Ok(instanceService.GetInstanceDetails(instanceGuid));

        [HttpDelete("{instanceGuid}")]
        public void Delete(Guid instanceGuid)
        {
            instanceService.DeleteInstance(instanceGuid);
        }

        [HttpGet]
        public IActionResult GetInstances() => Ok(instanceService.GetInstances());

        [HttpGet("{instanceGuid}")]
        public IActionResult Get(Guid instanceGuid) => Ok(instanceService.GetInstance(instanceGuid));

        [HttpPost]
        public IActionResult Post([FromBody] Instance instance) => Ok(instanceService.UpsertInstance(instance));
    }
}