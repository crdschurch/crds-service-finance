using System;
using Crossroads.Web.Common.Configuration;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using AutoMapper;
using Crossroads.Service.Finance.Interfaces;
using Crossroads.Service.Finance.Services;
using MinistryPlatform.Interfaces;
using MinistryPlatform.Repositories;
using Pushpay;
using Pushpay.Client;
using RestSharp;
using Pushpay.Token;

namespace Crossroads.Service.Finance
{
    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {
            try
            {
                DotNetEnv.Env.Load("../.env");
            }
            catch (Exception e)
            {
                // no .env file present but since not required, just write
                Console.Write(e);
            }
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("./appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"./appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();
            Configuration = builder.Build();
        }

        public IConfigurationRoot Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc();
            services.AddAutoMapper();
            services.AddDistributedMemoryCache();
            services.AddRouting(options => options.LowercaseUrls = true );
            services.AddCors();

            // Dependency Injection
            CrossroadsWebCommonConfig.Register(services);

            // Service Layer
            services.AddSingleton<IBatchService, BatchService>();
            services.AddSingleton<IDonationService, DonationService>();
            services.AddSingleton<IDepositService, DepositService>();

            services.AddSingleton<IPaymentEventService, PaymentEventService>();
            services.AddSingleton<IPushpayService, PushpayService>();
            services.AddSingleton<IPushpayClient, PushpayClient>();
            services.AddSingleton<IPushpayTokenService, PushpayTokenService>();

            // Repo Layer
            services.AddSingleton<IBatchRepository, BatchRepository>();
            services.AddSingleton<IDepositRepository, DepositRepository>();
            services.AddSingleton<IDonationRepository, DonationRepository>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();

            app.UseCors(builder => builder
                .AllowAnyOrigin()
                .AllowAnyMethod()
                .AllowAnyHeader()
                .AllowCredentials());
            app.UseMvc();
        }
    }
}
