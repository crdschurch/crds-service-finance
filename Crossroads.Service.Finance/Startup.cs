using AutoMapper;
using Crossroads.Service.Finance.Interfaces;
using Crossroads.Service.Finance.Services;
using Crossroads.Web.Common.Configuration;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MinistryPlatform.Interfaces;
using MinistryPlatform.Repositories;
using Pushpay.Client;
using Pushpay.Token;
using System;
using Hangfire;

namespace Crossroads.Service.Finance
{
    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {
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
            var dbUser = Environment.GetEnvironmentVariable("MP_API_DB_USER");
            var dbPassword = Environment.GetEnvironmentVariable("MP_API_DB_PASSWORD");
            var hangfireConnectionString = Configuration["ConnectionStrings:Hangfire"];
            hangfireConnectionString = String.Format(hangfireConnectionString, dbUser, dbPassword);
            services.AddHangfire(config =>config.UseSqlServerStorage(hangfireConnectionString));
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
            services.AddSingleton<IRecurringGiftRepository, RecurringGiftRepository>();
            services.AddSingleton<IProgramRepository, ProgramRepository>();
            services.AddSingleton<IContactRepository, ContactRepository>();
            services.AddSingleton<IPledgeRepository, PledgeRepository>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            app.UseHangfireServer();
            app.UseCors(builder => builder
                .AllowAnyOrigin()
                .AllowAnyMethod()
                .AllowAnyHeader()
                .AllowCredentials());
            app.UseMvc(routes =>
            {
                routes.MapRoute("default", "api/{controller}/{action?}/{id?}");
            });
            //app.UseMvc();
        }
    }
}
