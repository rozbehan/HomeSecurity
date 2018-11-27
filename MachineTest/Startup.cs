using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Http;

namespace MachineTest
{
    public class Startup
    {
        
        public IConfiguration Configuration { get; }
        public static string HomeSecurityApiUrl { get; private set; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;

            HomeSecurityApiUrl = configuration.GetSection("HomeSecurityApiUrl").GetSection("Url").Value;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseBrowserLink();
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
            }

            app.UseStaticFiles();

            app.UseMvc();

            app.Run(async (context) =>
            {
                HomeSecurityApiUrl = Configuration["HomeSecurityApiUrl"];
                await context.Response.WriteAsync(HomeSecurityApiUrl);
            });
        }
    }
}
