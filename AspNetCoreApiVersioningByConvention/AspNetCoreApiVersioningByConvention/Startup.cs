﻿using AspNetCoreApiVersioningByConvention.Conventions;
using AspNetCoreApiVersioningByConvention.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace AspNetCoreApiVersioningByConvention
{
    public class Startup
    {
        public IConfigurationRoot Configuration { get; }

        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();
            Configuration = builder.Build();
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // Add framework services.
            services.AddApiVersioning(options =>
            {
                options.ReportApiVersions = true;
            });

            services.AddMvc(options =>
            {
                options.Conventions.Remove<ApiVersionConvention>();
                options.Conventions.Remove<ImplicitControllerVersionConvention>();
                options.Conventions.Add(new ApiVersionRoutePrefixConvention());
                options.Conventions.Add(new ApiVersionByNamespaceConvention());
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();
            app.UseMvc();
        }
    }
}