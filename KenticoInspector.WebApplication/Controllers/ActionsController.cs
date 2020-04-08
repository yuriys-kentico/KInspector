using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;

using KenticoInspector.Core.Services;

using Microsoft.AspNetCore.Mvc;

namespace KenticoInspector.WebApplication.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ActionsController : ControllerBase
    {
        private readonly IModuleService moduleService;

        public ActionsController(IModuleService moduleService)
        {
            this.moduleService = moduleService;
        }

        [HttpGet("{instanceGuid}")]
        public IActionResult GetActions(Guid instanceGuid) => Ok(moduleService.GetActions(instanceGuid));

        [HttpPost("{codename}/execute/{instanceGuid}")]
        public async Task<IActionResult> ExecuteAsync(string codename, Guid instanceGuid)
        {
            using var reader = new StreamReader(Request.Body, Encoding.UTF8);

            var optionsJson = await reader.ReadToEndAsync();

            return Ok(moduleService.GetActionResults(codename, instanceGuid, optionsJson));
        }
    }
}