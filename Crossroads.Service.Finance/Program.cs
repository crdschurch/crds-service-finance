using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using System;
using System.IO;

namespace Crossroads.Service.Finance
{
    public class Program
    {

        public static void Main(string[] args)
        {
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
