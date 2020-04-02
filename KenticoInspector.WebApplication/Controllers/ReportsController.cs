using System;
using System.Collections.Generic;

using KenticoInspector.Core.Models.Results;
using KenticoInspector.Core.Modules;
using KenticoInspector.Core.Services.Interfaces;

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
        public ActionResult<IEnumerable<IReport>> GetReports(Guid instanceGuid)
        {
            return Ok(moduleService.GetReports(instanceGuid));
        }

        [HttpGet("{codename}/results/{instanceGuid}")]
        public ActionResult<ReportResults> GetReportResults(string codename, Guid instanceGuid)
        {
            return moduleService.GetReportResults(codename, instanceGuid);
        }
    }
}