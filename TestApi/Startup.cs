using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.IO;


namespace TestApi
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {

        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
				app.UseBrowserLink();
            }

            

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                if (Program._config.loadrootpage)
                {
                    endpoints.MapGet("/", async context =>
                    {
                        context.Response.Headers.Add("Server", $"BSRepo/1.0.0 ({osType()})");
                        await context.Response.WriteAsync(File.ReadAllText(Environment.CurrentDirectory + @"\public\root.html"));
                    });
                }
                else
                {
                    endpoints.MapGet("/", async context =>
                    {
                        context.Response.Headers.Add("Server", $"BSRepo/1.0.0 ({osType()})");
                        context.Response.StatusCode = 404;
                        await context.Response.WriteAsync("<h1>404 Page Not Found</h1>");
                    });
                }
                /*endpoints.MapGet("/", async context =>
                {
                    context.Response.Headers.Add("Server", $"BSRepo/1.0.0 ({osType()})");
                });*/
            });
        }

        public static string osType()
        {
            if (OperatingSystem.IsWindows())
            {
                return "Win";
            }
            else if (OperatingSystem.IsLinux() || OperatingSystem.IsMacOS())
            {
                return "Unix";
            }
            return "Empty";
        }
    }
}
