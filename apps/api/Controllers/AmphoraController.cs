using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using api.Models;
using api.Contracts;

namespace api.Controllers
{
    [Route("amphorae")]
    public class AmphoraController : Controller
    {
        private readonly IAmphoraModelService amphoraModelService;

        public AmphoraController(IAmphoraModelService amphoraModelService)
        {
            this.amphoraModelService = amphoraModelService;
        }
        
        [HttpGet()]
        public IActionResult ListAmphoraIds()
        {
            return Ok(this.amphoraModelService.ListAmphoraeIds());
        }


        [HttpGet("{id}")]
        public IActionResult GetAmphoraInformation(string id)
        {
            return Ok(this.amphoraModelService.GetAmphora(id));
        }

        [HttpPut()]
        public IActionResult SetAmphora([FromBody] AmphoraModel model)
        {
            return Ok(this.amphoraModelService.SetAmphora(model));
        }
    }
}
