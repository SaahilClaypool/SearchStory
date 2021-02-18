using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using SearchStory.App.Data;
using SearchStory.App.Search;
using SearchStory.App.Services;
using SearchStory.App.UseCases;

namespace SearchStory.App
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddRazorPages();
            services.AddServerSideBlazor();
            services.AddSingleton<WeatherForecastService>();
            services.AddSingleton<DirectoryService>();
            // singleton to re-use between searches
            services.AddScoped<LuceneWriter>();
            services.AddScoped<LuceneReader>();
            services.AddControllers();
            services.AddUseCases();
            services.AddCors(o => o.AddPolicy("LocalBrowser", builder =>
            {
                builder
                    .AllowAnyOrigin()
                    .AllowAnyHeader()
                    .WithMethods(new string[] { "POST" });

            }));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            new DirectoryService().EnsurePathsExist();
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            // app.UseHttpsRedirection();
            System.Console.WriteLine(string.Join("\n\t", typeof(Program).Assembly.GetManifestResourceNames()));
            app.UseStaticFiles();
            try
            {
                app.UseStaticFiles(options: new()
                {
                    FileProvider = new ManifestEmbeddedFileProvider(
                        typeof(Program).Assembly,
                        "wwwroot"
                        ),
                });

                app.UseStaticFiles(options: new()
                {
                    FileProvider = new ManifestEmbeddedFileProvider(
                        typeof(Program).Assembly
                    ),
                });
            }
            catch
            {

            }


            app.UseStaticFiles(options: new()
            {
                FileProvider = new PhysicalFileProvider(
                    new DirectoryService().DocumentDir.FullName
                ),
                RequestPath = "/Document"
            });

            app.UseRouting();

            app.UseCors();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapBlazorHub();
                endpoints.MapFallbackToPage("/_Host");
            });
        }
    }
}
