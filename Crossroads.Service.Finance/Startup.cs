using AutoMapper;
using Crossroads.Service.Finance.Interfaces;
using Crossroads.Service.Finance.Services;
using Crossroads.Web.Common.Configuration;
using Hangfire;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MinistryPlatform.Donors;
using MinistryPlatform.Interfaces;
using MinistryPlatform.Repositories;
using Pushpay.Client;
using Pushpay.Token;
using System;
using Utilities.Logging;

namespace Crossroads.Service.Finance
{
    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddEnvironmentVariables();
            Configuration = builder.Build();
        }

        public IConfigurationRoot Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            var hangfireConnectionString = Environment.GetEnvironmentVariable("HANGFIRE_URL");
            services.AddHangfire(config => config.UseSqlServerStorage(hangfireConnectionString));
            services.AddMvc();
            services.AddAutoMapper();
            services.AddDistributedMemoryCache();
            services.AddRouting(options => options.LowercaseUrls = true);
            services.AddCors();

            // Dependency Injection
            CrossroadsWebCommonConfig.Register(services);

            // commenting this out as "Crossroads.Service.Finance.xml" file is not being
            // generated when building

            // Add Swagger
            // try {
            //     services.AddSwaggerGen(c =>
            //     {
            //         c.SwaggerDoc("v1", new Info { Title = "Finance Microservice"});
            //         var xmlPath = Path.Combine(AppContext.BaseDirectory, "Crossroads.Service.Finance.xml");
            //         c.IncludeXmlComments(xmlPath);
            //     });
            // } catch (Exception e) {
            //     Console.WriteLine(e.Message);
            // }

            // Service Layer
            services.AddSingleton<IBatchService, BatchService>();
            services.AddSingleton<IContactService, ContactService>();
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
            services.AddSingleton<IDonorRepository, DonorRepository>();
            services.AddSingleton<IRecurringGiftRepository, RecurringGiftRepository>();
            services.AddSingleton<IProgramRepository, ProgramRepository>();
            services.AddSingleton<IContactRepository, ContactRepository>();
            services.AddSingleton<IPledgeRepository, PledgeRepository>();
            services.AddSingleton<IDonationDistributionRepository, DonationDistributionRepository>();
            services.AddSingleton<IGatewayService, GatewayService>();

            // Utilities Layer
            services.AddSingleton<IDataLoggingService, NewRelicAgentWrapper>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            app.UseHangfireServer();
            app.UseCors(builder => builder
                .AllowAnyOrigin()
                .AllowAnyMethod()
                .AllowAnyHeader()
                .AllowCredentials());
            app.UseMvc();

            // commenting this out as "Crossroads.Service.Finance.xml" file is not being
            // generated when building

            // app.UseSwagger(o =>
            // {
            //     // ensure controller is lowercased
            //     o.PreSerializeFilters.Add((document, request) =>
            //     {
            //         document.Paths = document.Paths.ToDictionary(p => p.Key.ToLowerInvariant(), p => p.Value);
            //     });
            // });
            // app.UseSwaggerUI(c =>
            // {
            //     c.SwaggerEndpoint("/swagger/v1/swagger.json", "Finance Microservice");
            //     c.DocExpansion(DocExpansion.None);
            //     c.RoutePrefix = string.Empty;
            // });
        }
    }
}
