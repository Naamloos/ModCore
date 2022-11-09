using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace ModCore.Api
{
    public class ApiStartup
	{
		public ApiStartup(IConfiguration configuration)
		{
			Configuration = configuration;
		}

		public IConfiguration Configuration { get; }

		// This method gets called by the runtime. Use this method to add services to the container.
		public void ConfigureServices(IServiceCollection services)
		{
            services.AddOptions();
            services.AddMemoryCache();
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

			services.AddControllers()
				.AddNewtonsoftJson();
        }

		// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
		public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
		{
			if (env.IsDevelopment())
			{
				app.UseDeveloperExceptionPage();
			}

            //app.UseStaticFiles(new StaticFileOptions()
            //{
            //    FileProvider = new EmbeddedFileProvider(typeof(Web.Pages.IndexModel).Assembly, "ModCore.Web.Pages")
            //});

			app.UseRouting();
			app.UseEndpoints(x => {
				x.MapControllers();
			});
		}
	}
}
