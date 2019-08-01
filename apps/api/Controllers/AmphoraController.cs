using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using api.Models;

namespace api.Controllers
{
    [Route("amphorae")]
    public class AmphoraController : Controller
    {
        public IActionResult Index()
        {
            return Ok("hello");
        }


        [Route("{id}")]
        public IActionResult GetAmphoraInformation(string id)
        {
            return Json(new AmphoraModel(){
                Title= "Soil Data",
                Description= "some numbers and letteers",
                Price=21,
                Id=id
            });
        }
    }
}
