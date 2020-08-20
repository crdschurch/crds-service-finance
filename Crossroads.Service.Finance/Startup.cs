using AutoMapper;
using Crossroads.Microservice.Logging;
using Crossroads.Microservice.Settings;
using Crossroads.Service.Finance.Interfaces;
using Crossroads.Service.Finance.Middleware;
using Crossroads.Service.Finance.Services;
using Crossroads.Service.Finance.Services.DbClients;
using Crossroads.Service.Finance.Services.Exports;
using Crossroads.Service.Finance.Services.JournalEntry;
using Crossroads.Service.Finance.Services.JournalEntryBatch;
using Crossroads.Service.Finance.Services.Recurring;
using Crossroads.Web.Common.Configuration;
using Exports.JournalEntries;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MinistryPlatform.Adjustments;
using MinistryPlatform.Congregations;
using MinistryPlatform.Donors;
using MinistryPlatform.Interfaces;
using MinistryPlatform.JournalEntries;
using MinistryPlatform.Repositories;
using MinistryPlatform.Users;
using MongoDB.Driver;
using Newtonsoft.Json;
using ProcessLogging.Transfer;
using Pushpay.Cache;
using Pushpay.Client;
using Pushpay.Token;
using System;
using Crossroads.Service.Finance.Services.Donor;

namespace Crossroads.Service.Finance
{
    public class Startup
    {
        public Startup(IWebHostEnvironment env)
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
            services.AddMvc();
            services.AddAutoMapper();
            services.AddDistributedMemoryCache();
            services.AddRouting(options => options.LowercaseUrls = true);
            services.AddCors();

            services.AddMvc(option => option.EnableEndpointRouting = false)
                .SetCompatibilityVersion(CompatibilityVersion.Version_3_0)
                .AddNewtonsoftJson(opt => opt.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore);


            SettingsService settingsService = new SettingsService();
            services.AddSingleton<ISettingsService>(settingsService);

            //services.AddLogging(loggingBuilder =>
            //{
            //    loggingBuilder.AddConsole();
            //    loggingBuilder.AddDebug();
            //});

            Logger.SetUpLogging(settingsService.GetSetting("LOGZ_IO_KEY"),
                                settingsService.GetSetting("CRDS_ENV"));

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
            services.AddSingleton<IAdjustmentsToJournalEntriesService, AdjustmentsToJournalEntriesService>();
            services.AddSingleton<IDonorService, DonorService>();
            services.AddSingleton<IJournalEntryService, JournalEntryService>();
            services.AddSingleton<IJournalEntryBatchService, JournalEntryBatchService>();
            services.AddSingleton<IContactService, ContactService>();
            services.AddSingleton<IDonationService, DonationService>();
            services.AddSingleton<IDepositService, DepositService>();
            services.AddSingleton<IPaymentEventService, PaymentEventService>();
            services.AddSingleton<IPushpayService, PushpayService>();
            services.AddSingleton<INewPushpayService, NewPushpayService>();
            services.AddSingleton<IPushpayClient, PushpayClient>();
            services.AddSingleton<IPushpayTokenService, PushpayTokenService>();
            services.AddSingleton<IRecurringService, RecurringService>();
            services.AddSingleton<IExportService, ExportService>();

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
            services.AddSingleton<IWebhooksRepository, WebhooksRepository>();
            services.AddSingleton<IGatewayService, GatewayService>();
            services.AddSingleton<IUserRepository, UserRepository>();
            services.AddSingleton<ICongregationRepository, CongregationRepository>();
            services.AddSingleton<IAdjustmentRepository, AdjustmentRepository>();
            services.AddSingleton<IJournalEntryRepository, JournalEntryRepository>();

            // Exports Layer
            services.AddSingleton<IJournalEntryExport, VelosioExportClient>();

            // Process Logging Layer
            services.AddSingleton<IProcessLogger, NoSqlProcessLogger>();
            services.AddSingleton<INoSqlDbService>(InitializeProcessLoggingDbService());

            // Add support for caching
            services.AddSingleton<ICacheService, CacheService>();
            services.AddDistributedMemoryCache();

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseCors(builder => builder
                .AllowAnyOrigin()
                .AllowAnyMethod()
                .AllowAnyHeader());

            // used for impersonation
            app.UseImpersonationMiddleware();

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

        public INoSqlDbService InitializeProcessLoggingDbService()
        {
            var mongoClient = new MongoClient(Environment.GetEnvironmentVariable("NO_SQL_CONNECTION_STRING"));
            var cosmosDbService = new NoSqlDbService(mongoClient);
            return cosmosDbService;
        }
    }
}
