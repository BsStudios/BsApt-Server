using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.IO;

namespace TestApi
{
    public class Program
    {
        public static config _config;


        public static void Main(string[] args)
        {
            if (!File.Exists(Environment.CurrentDirectory + @"\config.json"))
            {
                File.WriteAllText(Environment.CurrentDirectory + @"\config.json", @"{""hosts"": [""https://localhost:8006"", ""http://localhost:8005""],""loadrootpage"": ""true"",""litedbpath"": "".\bsdb.db""}");
            }
                _config = JsonConvert.DeserializeObject<config>(File.ReadAllText(Environment.CurrentDirectory + @"\config.json"));
            if(_config.litedbpath == "" || _config.litedbpath == null || !File.Exists(_config.litedbpath))
            {
                Console.WriteLine("LiteDB Database Not Found. Making One In Default Directory.");
                _config.litedbpath = ".\\bsdb.db";
                File.WriteAllText(Environment.CurrentDirectory + @"\config.json", JsonConvert.SerializeObject(_config));

            }
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    
                    string[] hosts = _config.hosts;
                    webBuilder.UseUrls(hosts);
                    webBuilder.UseStartup<Startup>();
                });
    }
}
