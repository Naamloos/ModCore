using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using AspNetCoreRateLimit;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace ModCore.CoreApi
{
	public class Startup
	{
		public Startup(IConfiguration configuration)
		{
			Configuration = configuration;
		}

		public IConfiguration Configuration { get; }

		// This method gets called by the runtime. Use this method to add services to the container.
		public void ConfigureServices(IServiceCollection services)
		{
            //Taken from https://github.com/stefanprodan/AspNetCoreRateLimit/wiki/IpRateLimitMiddleware#setup
            services.AddOptions();
            services.AddMemoryCache();
            services.Configure<IpRateLimitOptions>(Configuration.GetSection("IpRateLimiting"));
            services.Configure<IpRateLimitPolicies>(Configuration.GetSection("IpRateLimitPolicies"));
            services.AddSingleton<IIpPolicyStore, MemoryCacheIpPolicyStore>();
            services.AddSingleton<IRateLimitCounterStore, MemoryCacheRateLimitCounterStore>();
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddSingleton<IRateLimitConfiguration, RateLimitConfiguration>();

            services.AddControllersWithViews()
                .AddNewtonsoftJson();
		}

		// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
		public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
		{
			if (env.IsDevelopment())
			{
				app.UseDeveloperExceptionPage();
			}

			if (!Directory.Exists("wwwstatic"))
				Directory.CreateDirectory("wwwstatic");

			var fp = new PhysicalFileProvider(Path.Combine(Directory.GetCurrentDirectory(), "wwwstatic"));

			var df = new DefaultFilesOptions();
			df.DefaultFileNames.Clear();
			df.DefaultFileNames.Add("index.html");
			df.FileProvider = fp;
			df.RequestPath = "";

            app.UseIpRateLimiting();

            app.UseDefaultFiles(df);
			app.UseStaticFiles(new StaticFileOptions() { FileProvider = fp, RequestPath = "" });

			app.UseRouting();
			app.UseEndpoints(x => {
				x.MapControllers();
                
			});
		}
	}
}
