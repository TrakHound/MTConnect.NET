// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Hosting.WindowsServices;
using Microsoft.Extensions.Logging;
using MTConnect.Agents;
using MTConnect.Applications.Loggers;
using MTConnect.Configurations;
using NLog.Web;
using System;
using System.IO;
using System.IO.Compression;
using System.Linq;

namespace MTConnect.Applications
{
    public class Program
    {
        private static AgentGatewayConfiguration _configuration;


        public static void Main(string[] args)
        {
            var logger = NLogBuilder.ConfigureNLog("nlog.config").GetCurrentClassLogger();
            try
            {
                // Set WebApplication Options
                var options = new WebApplicationOptions
                {
                    Args = args,
                    ContentRootPath = WindowsServiceHelpers.IsWindowsService() ? AppContext.BaseDirectory : default
                };

                // Create WebApplication Builder
                var builder = WebApplication.CreateBuilder(options);
                ConfigureBuilder(builder);
                AddServices(builder);

                // Create WebApplication
                var app = builder.Build();
                ConfigureServices(app);

                // Run WebApplication
                app.Run();
            }
            catch (Exception exception)
            {
                //NLog: catch setup errors
                logger.Fatal(exception, "Stopped program because of exception");
                throw;
            }
            finally
            {
                // Ensure to flush and stop internal timers/threads before application-exit (Avoid segmentation fault on Linux)
                NLog.LogManager.Shutdown();
            }
        }

        // This is the method that should be able to be used instead of CreateHostBuilder()
        private static void ConfigureBuilder(WebApplicationBuilder builder)
        {
            // Set to allow Windows Service
            builder.Host.UseWindowsService();

            // Add Logging
            builder.Host.UseNLog();
            builder.Logging.AddConsole();
        }

        private static void AddServices(WebApplicationBuilder builder)
        {
            // Copy Default Configuration File
            string configPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, AgentConfiguration.JsonFilename);
            string defaultPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, AgentConfiguration.DefaultJsonFilename);
            if (!File.Exists(configPath) && File.Exists(defaultPath))
            {
                File.Copy(defaultPath, configPath);
            }

            _configuration = AgentConfiguration.ReadJson<AgentGatewayConfiguration>();
            if (_configuration != null)
            {
                // Read Agent Information File
                var agentInformation = MTConnectAgentInformation.Read();
                if (agentInformation == null)
                {
                    agentInformation = new MTConnectAgentInformation();
                    agentInformation.Save();
                }

                // Create MTConnectAgent
                var agent = new MTConnectAgentBroker(_configuration, agentInformation.Uuid);
                agent.MTConnectVersion = MTConnectVersions.Max;
                agent.Start();
                builder.Services.AddSingleton<IMTConnectAgentBroker>(agent);

                // Individual Logger Classes
                builder.Services.AddSingleton<AgentGatewayConfiguration>(_configuration);
                builder.Services.AddSingleton<IHttpAgentConfiguration>(_configuration);
                builder.Services.AddSingleton<AgentLogger>();
                builder.Services.AddSingleton<AgentMetricLogger>();
                builder.Services.AddSingleton<AgentValidationLogger>();

                // Add the MTConnectAgentService that handles the MTConnect Agent
                builder.Services.AddHostedService<MTConnectAgentService>();

                if (!_configuration.ResponseCompression.IsNullOrEmpty())
                {
                    // Add Compression Services
                    builder.Services.AddResponseCompression(options =>
                    {
                        options.EnableForHttps = true;

                        // Gzip
                        if (_configuration.ResponseCompression.Any(o => o == Http.HttpResponseCompression.Gzip))
                        {
                            options.Providers.Add<GzipCompressionProvider>();
                        }

                        // Brotli
                        if (_configuration.ResponseCompression.Any(o => o == Http.HttpResponseCompression.Br))
                        {
                            options.Providers.Add<BrotliCompressionProvider>();
                        }
                    });

                    // Gzip
                    if (_configuration.ResponseCompression.Any(o => o == Http.HttpResponseCompression.Gzip))
                    {
                        builder.Services.Configure<GzipCompressionProviderOptions>(options => { options.Level = CompressionLevel.Optimal; });
                    }

                    // Brotli
                    if (_configuration.ResponseCompression.Any(o => o == Http.HttpResponseCompression.Br))
                    {
                        builder.Services.Configure<BrotliCompressionProviderOptions>(options => { options.Level = CompressionLevel.Optimal; });
                    }
                }

                if (_configuration.Port > 0)
                {
                    builder.WebHost.UseKestrel(o =>
                    {
                        o.ListenAnyIP(_configuration.Port);
                    });
                }
            }

            // Add Controllers
            builder.Services.AddControllers();
        }

        private static void ConfigureServices(WebApplication app)
        {
            if (app.Environment.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
            }

            app.UseStaticFiles();
            app.UseRouting();

            if (_configuration != null && !_configuration.ResponseCompression.IsNullOrEmpty())
            {
                app.UseResponseCompression();
            }

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
