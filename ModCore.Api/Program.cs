using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace ModCore.CoreApi
{
	public class ApiListener
	{
		internal static void StartASP()
		{
			BuildWebHost().Run();
		}

		internal static IWebHost BuildWebHost()
		{
			return WebHost.CreateDefaultBuilder(new string[0])
				.UseStartup<Startup>()
				.Build();
		}
	}
}
