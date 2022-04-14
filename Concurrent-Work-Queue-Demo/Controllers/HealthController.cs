using System;

using Microsoft.AspNetCore.Mvc;

namespace Concurrent_Work_Queue_Demo.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class HealthController : ControllerBase
    {
        #region Methods

        [HttpGet]
        public ActionResult<String> Get()
        {
            return this.Ok("Healthy");
        }

        #endregion
    }
}
