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
using System.Text;
using System.Security.Cryptography;

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

                endpoints.MapGet("/users", async context =>
                {
                    context.Response.Headers.Add("Server", $"BSRepo/1.0.0 ({osType()})");
                    var users = Program._liteDB.GetCollection<LiteDBClasses.Users>("users");
                    var results = users.Query()
                        .Select(x => new LiteDBClasses.Users { Id = x.Id, username = x.username, password = x.password})
                        .Limit(10)
                        .ToList();
                    StringBuilder stringBuilder = new StringBuilder();
                    foreach (LiteDBClasses.Users users1 in results)
                    {
                        string password = Encryption.Decrypt(users1.password);

                        stringBuilder.Append(users1.Id + "   " + users1.username + "   " + users1.password.ToString() + "     " + password + "     " +"\n");
                    }
                    await context.Response.WriteAsync(stringBuilder.ToString());
                });
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
