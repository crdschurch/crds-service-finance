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
            // this basically duplicates the section in startup, but may be necessary to log exceptions on application load here
            var loggingEnv = Environment.GetEnvironmentVariable("CRDS_ENV");

            XmlDocument log4netConfig = new XmlDocument();

            switch (loggingEnv)
            {
                case "dev":
                    log4netConfig.Load(File.OpenRead("log4net.dev.config"));
                    break;
                case "int":
                    log4netConfig.Load(File.OpenRead("log4net.int.config"));
                    break;
                case "demo":
                    log4netConfig.Load(File.OpenRead("log4net.demo.config"));
                    break;
                case "prod":
                    log4netConfig.Load(File.OpenRead("log4net.prod.config"));
                    break;
                default:
                    log4netConfig.Load(File.OpenRead("log4net.config"));
                    break;
            }

            var repo = log4net.LogManager.CreateRepository(
                Assembly.GetEntryAssembly(), typeof(log4net.Repository.Hierarchy.Hierarchy));

            log4net.Config.XmlConfigurator.Configure(repo, log4netConfig["log4net"]);

            log.Info("Application - Main is invoked");

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
