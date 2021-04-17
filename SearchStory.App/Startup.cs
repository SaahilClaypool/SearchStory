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
using SearchStory.App.Search;
using SearchStory.App.Services;
using SearchStory.App.UseCases;
using SearchStory.App.Platform;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Components.Authorization;
using Blazored.Modal;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using System.Net.Http;

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
            services.AddBlazoredModal();

            services.AddDbContext<AppDbContext>(
                options =>
                options.UseSqlite(
                    Configuration.GetConnectionString("DefaultConnection"))
            );


            services.AddRazorPages(o =>
            {
                o.RootDirectory = "/UI/Pages/";
            });

            #region Auth
            services.AddDefaultIdentity<IdentityUser>(options => { })
                .AddEntityFrameworkStores<AppDbContext>();
            services.AddAuthentication(
                CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie();
            services.AddScoped<AuthenticationStateProvider, RevalidatingIdentityAuthenticationStateProvider<IdentityUser>>();
            services.AddHttpContextAccessor();
            services.AddScoped<HttpContextAccessor>();
            services.AddHttpClient();
            services.AddScoped<HttpClient>();
            #endregion

            services.AddServerSideBlazor();
            services.AddSingleton<DirectoryService>();
            services.AddScoped<LuceneWriter>();
            services.AddScoped<LuceneReader>();
            services.AddControllers();
            services.AddUseCases();
            services.AddScoped<StateContainer>();
            services.AddScoped<LoginService>();
            services.AddCors(o => o.AddPolicy("LocalBrowser", builder =>
            {
                builder
                    .AllowAnyOrigin()
                    .AllowAnyHeader()
                    .WithMethods(new string[] { "POST" });

            }));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, AppDbContext db)
        {
            app.UsePathBase("/searchstory");
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
            db.Database.EnsureCreated();
            try 
            {
                db.Database.Migrate();
            }
            catch 
            {
                Console.WriteLine("No migrations could be added");
            }
            
            using (var serviceScope = app.ApplicationServices.GetRequiredService<IServiceScopeFactory>().CreateScope())
            {
                ScopedSetup(serviceScope);
            }
            #region Cookie // https://blazorhelpwebsite.com/ViewBlogPost/36
            System.Console.WriteLine("Setting cookies");
            app.UseCookiePolicy();
            app.UseAuthentication();
            #endregion

            // app.UseHttpsRedirection();
            // System.Console.WriteLine(string.Join("\n\t", typeof(Program).Assembly.GetManifestResourceNames()));
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
                RequestPath = "/Document",
                ServeUnknownFileTypes = true
            });

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();
            // serviceProvider is app.ApplicationServices from Configure(IApplicationBuilder app) method

            app.UseCors();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapBlazorHub();
                endpoints.MapFallbackToPage("/_Host");
            });

            var iconManager = new SystemTrayIconManager();
            iconManager.Instantiate().Wait();
        }
        
        void ScopedSetup(IServiceScope scope)
        {
            var login = scope.ServiceProvider.GetService<LoginService>()!;
            login.SeedUsers();
        }
    }
}
