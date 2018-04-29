using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace ModCore.CoreApi.Controllers
{
    [Route("")]
    public class ModCoreApi : Controller
    {
        // GET ping
        [HttpGet("ping")]
        public string Ping()
        {
            return "pong!";
        }

		[HttpGet("prefix")]
		internal string Prefix()
		{
			return "lul";
		}
    }
}
