﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

using KenticoInspector.Core.Models;
using KenticoInspector.Core.Modules;
using KenticoInspector.Core.Services.Interfaces;

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
        public ActionResult<IEnumerable<IReport>> Get(Guid instanceGuid)
        {
            return Ok(moduleService.GetActions(instanceGuid));
        }

        // POST api/values
        [HttpPost("{codename}/execute/{instanceGuid}")]
        public ActionResult<ActionResults> Excecute(string codename, Guid instanceGuid)
        {
            using (StreamReader reader = new StreamReader(Request.Body, Encoding.UTF8))
            {
                var optionsJson = reader.ReadToEnd();
                return moduleService.ExecuteAction(codename, instanceGuid, optionsJson);
            }
        }
    }
}