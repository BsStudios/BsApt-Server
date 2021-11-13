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
using LiteDB;
using System.Text;
using System.Security.Cryptography;

namespace TestApi
{
    public class Program
    {
        public static config _config;
        public static LiteDatabase _liteDB;

        public static void Main(string[] args)
        {

            if (!File.Exists(Environment.CurrentDirectory + @"\config.json"))
            {
                File.WriteAllText(Environment.CurrentDirectory + @"\config.json", @"{""hosts"": [""https://localhost:8006"", ""http://localhost:8005""],""loadrootpage"": ""true"",""litedbpath"": "".\bsdb.db""}");
            }
            _config = JsonConvert.DeserializeObject<config>(File.ReadAllText(Environment.CurrentDirectory + @"\config.json"));
            bool isNewDB = false;
            if(_config.litedbpath == "" || _config.litedbpath == null || !File.Exists(_config.litedbpath))
            {
                Console.WriteLine("LiteDB Database Not Found. Making One In Default Directory.");
                _config.litedbpath = ".\\bsdb.db";
                File.WriteAllText(Environment.CurrentDirectory + @"\config.json", JsonConvert.SerializeObject(_config));
                isNewDB = true;
            }
            _liteDB = new LiteDatabase(_config.litedbpath);
        newDB:;
            if (isNewDB)
            {
                Console.WriteLine("Initializing New Database.\nPlease Enter New Password For Root Account.");
                string pass = String.Empty;
                StringBuilder passwordsb = new StringBuilder();
                while (true)
                {
                    var key = Console.ReadKey(true);
                    if (key.Key == ConsoleKey.Enter) break;
                    if (key.Key == ConsoleKey.Backspace && passwordsb.Length > 0) passwordsb.Remove(passwordsb.Length - 1, 1);
                    if (key.Key != ConsoleKey.Backspace) passwordsb.Append(key.KeyChar);
                }
                pass = passwordsb.ToString();

                string encryptpass = Encryption.Encrypt(pass);

                var users = _liteDB.GetCollection<LiteDBClasses.Users>("users");
                LiteDBClasses.Users user = new LiteDBClasses.Users
                {
                    username = "root",
                    password = encryptpass
                };
                users.Insert(user);
            AccountCreate:;
                Console.WriteLine("Would You Like To Add Another Account? Y or N (Default: N)");
                string temp = Console.ReadLine();
                if(temp.ToLower() == "y" || temp.ToLower() == "yes")
                {
                    string usname = string.Empty;
                    Console.WriteLine("Please New Enter username.");
                    usname = Console.ReadLine();
                    var results = users.Query()
                        .Where(x => x.username.Equals(temp))
                        .Select(x => new { x.username })
                        .Limit(1)
                        .ToList();
                    if(results.Count >= 1)
                    {
                        Console.WriteLine("Username Already Exists.");
                        goto AccountCreate;
                    }
                    else
                    {
                        Console.WriteLine("Please Enter New Password For Account.");
                        pass = String.Empty;
                        passwordsb = new StringBuilder();
                        while (true)
                        {
                            var key = Console.ReadKey(true);
                            if (key.Key == ConsoleKey.Enter) break;
                            if (key.Key == ConsoleKey.Backspace && passwordsb.Length > 0) passwordsb.Remove(passwordsb.Length - 1, 1);
                            if (key.Key != ConsoleKey.Backspace) passwordsb.Append(key.KeyChar);
                        }
                        pass = passwordsb.ToString();

                        string encryptpass2 = Encryption.Encrypt(pass);

                        user = new LiteDBClasses.Users
                        {
                            username = usname,
                            password = encryptpass
                        };
                        users.Insert(user);
                        goto AccountCreate;
                    }

                }
                else
                {
                    pass = null;
                    passwordsb = null;
                    user = null;
                }
            }
            var users1 = _liteDB.GetCollection<LiteDBClasses.Users>("users");
            var results1 = users1.Query()
                        .Where(x => x.username.Equals("root"))
                        .Select(x => new { x.username })
                        .Limit(1)
                        .ToList();
            if (!(results1.Count >= 1))
            {
                isNewDB = true;
                goto newDB;
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
