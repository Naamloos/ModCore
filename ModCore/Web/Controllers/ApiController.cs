using Microsoft.AspNetCore.Mvc;
using ModCore.CoreApi.Entities;
using ModCore.Database;
using ModCore.Entities;
using ModCore.Logic.Extensions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DSharpPlus;

namespace ModCore.Web.Controllers
{
    [Route("api")]
    public class ApiController : Controller
    {
        private ModCore ModCore;

        public ApiController(ModCore modcore)
        {
            ModCore = modcore;
        }

        public string Index()
        {
            return "ModCore API";
        }

        // GET ping
        [HttpGet("ping")]
        public string Ping()
        {
            return "pong!";
        }

        [HttpGet("default")]
        public string Dp()
        {
            return "Default prefix: " + ModCore.SharedData.DefaultPrefix;
        }
    }
}
