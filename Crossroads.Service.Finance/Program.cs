using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Xml;
using log4net;
using log4net.Config;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;

namespace Crossroads.Service.Finance
{
    public class Program
    {
        private static readonly log4net.ILog log =
            log4net.LogManager.GetLogger(typeof(Program));

        public static void Main(string[] args)
        {
             // load logging here to capture issues in starting the services
            var loggingPath = Environment.GetEnvironmentVariable("APP_LOG_ROOT");

            XmlDocument log4netConfig = new XmlDocument();

            log4netConfig.Load(File.OpenRead($"Logging/log4net.config"));

            log4netConfig.InnerXml = log4netConfig.InnerXml.Replace("${APP_LOG_ROOT}", loggingPath);

            var repo = log4net.LogManager.CreateRepository(
                Assembly.GetEntryAssembly(), typeof(log4net.Repository.Hierarchy.Hierarchy));

            log4net.Config.XmlConfigurator.Configure(repo, log4netConfig["log4net"]);

            log.Info("Application - Main is invoked");

            // load environment variables from .env file for local development
            try
            {
                DotNetEnv.Env.Load("../.env");
            }
            catch (Exception)
            {
                // no .env file present but since not required, just write
                Console.WriteLine("no .env file found, reading environment variables from machine");
            }

            var host = new WebHostBuilder()
                .UseKestrel()
                .UseContentRoot(Directory.GetCurrentDirectory())
                .UseIISIntegration()
                .UseStartup<Startup>()
                .Build();

            host.Run();
        }
    }
}
