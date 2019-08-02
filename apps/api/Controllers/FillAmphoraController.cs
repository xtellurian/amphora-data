using Microsoft.AspNetCore.Mvc;

namespace api.Controllers
{
    [Route("api/amphorae")]
    public class FillAmphoraController : Controller
    {
       

        public FillAmphoraController()
        {
           
        }


        [HttpGet("{id}/fill")]
        public IActionResult GetAmphoraInformation([FromBody] dynamic data)
        {
            return Ok(data);
        }
    }
}
