﻿using Digital.Slovensko.Ekosystem.GeneratorPP.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;


namespace Digital.Slovensko.Ekosystem.GeneratorPP
{
    /// <summary>
    /// The web application startup.
    /// </summary>
    public class Startup
    {
        /// <summary>
        /// Gets the configuration.
        /// </summary>
        public IConfiguration Configuration { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="Startup"/> class.
        /// </summary>
        /// <param name="environment">The environment.</param>
        public Startup(IWebHostEnvironment environment)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(environment.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);

            if (environment.IsDevelopment())
            {
                builder.AddUserSecrets<Startup>();
            }

            this.Configuration = builder.Build();
        }

        /// <summary>
        /// Configures the services.
        /// This method gets called by the runtime. Use this method to add services to the container.
        /// </summary>
        /// <param name="services">The services.</param>
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDistributedMemoryCache();
            services.AddSession();
            services.AddOptions();

            // Register the IConfiguration instance which AppSettings binds against.
            services.Configure<AppSettings>(this.Configuration);

            // services.AddMvc();
            services.AddControllersWithViews();
        }

        /// <summary>
        /// Configures the specified application.
        /// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        /// </summary>
        /// <param name="app">The application.</param>
        /// <param name="env">The env.</param>
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseSession();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // app.UseHsts();
            }

            // app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseEndpoints(routes =>
            {
                routes.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}"
                );
            });
        }
    }
}
