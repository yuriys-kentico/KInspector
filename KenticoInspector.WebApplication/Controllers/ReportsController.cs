using System;

using KenticoInspector.Core.Services;

using Microsoft.AspNetCore.Mvc;

namespace KenticoInspector.WebApplication.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReportsController : ControllerBase
    {
        private readonly IModuleService moduleService;

        public ReportsController(IModuleService moduleService)
        {
            this.moduleService = moduleService;
        }

        [HttpGet("{instanceGuid}")]
        public IActionResult GetReports(Guid instanceGuid) => Ok(moduleService.GetReports(instanceGuid));

        [HttpGet("{codename}/results/{instanceGuid}")]
        public IActionResult GetReportResults(string codename, Guid instanceGuid) => Ok(moduleService.GetReportResults(codename, instanceGuid));
    }
}