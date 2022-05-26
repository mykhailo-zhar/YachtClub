using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Project.Migrations;
using Project.Models;
using Project.Database;

namespace WebApp
{
    public class Startup
    {
        public Startup(IConfiguration config)
        {
            Configuration = config;
        }
        public IConfiguration Configuration { get; set; }
        public void ConfigureServices(IServiceCollection services)
        {
            Hash_Extension.StandartPassword = Configuration["StandartPassword"];
            services.AddDbContext<DataContext>(opts =>
            {
               // opts.UseNpgsql(Configuration["ConnectionStrings:YachtClubConnection"]);
                opts.EnableSensitiveDataLogging(true);
            });
            services.AddControllersWithViews().AddRazorRuntimeCompilation();
            services.AddRazorPages().AddRazorRuntimeCompilation();

            services.AddHttpContextAccessor();

            services.Configure<AntiforgeryOptions>(opts =>
            {
                opts.HeaderName = "X-XSRF-TOKEN";
            });

           services.AddAuthorization(options =>
            {
                options.AddPolicy("Warehouse", policy =>
                      policy.RequireRole(RolesReadonly.DB_Admin, RolesReadonly.Storekeeper));

                options.AddPolicy("Staff", policy =>
                      policy.RequireRole(RolesReadonly.DB_Admin, RolesReadonly.Personell_Officer)); 

                options.AddPolicy("CaptainCrew", policy =>
                      policy.RequireRole(RolesReadonly.DB_Admin, RolesReadonly.Personell_Officer, RolesReadonly.Captain));

                options.AddPolicy("Repair", policy =>
                      policy.RequireRole(RolesReadonly.DB_Admin, RolesReadonly.Repairman));

                options.AddPolicy("EReq", policy =>
                      policy.RequireRole(RolesReadonly.DB_Admin, RolesReadonly.Repairman, RolesReadonly.Storekeeper));
            });

            services.Configure<MvcOptions>(opts => {
                opts.ModelBindingMessageProvider.SetValueMustNotBeNullAccessor(value => "Пожалуйста введите значение:");
                opts.ModelBindingMessageProvider.SetMissingBindRequiredValueAccessor(value => $"Заполните пожалуйста, {value}");
            });

            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                    .AddCookie(options => 
                                        {
                                            options.LoginPath = new Microsoft.AspNetCore.Http.PathString("/Account/Login");
                                            options.ExpireTimeSpan = System.TimeSpan.FromHours(10);
                                        })
                    ;
        }
        public void Configure(IApplicationBuilder app, DataContext context, IAntiforgery antiforgery)
        {
            app.UseDeveloperExceptionPage();
            app.UseStaticFiles();
            app.UseRouting();

            app.Use(async (context, next) =>
            {
                if (!context.Request.Path.StartsWithSegments("/api"))
                {
                    context.Response.Cookies.Append("XSRF-TOKEN",
                    antiforgery.GetAndStoreTokens(context).RequestToken,
                    new CookieOptions { HttpOnly = false });
                }
                await next();
            });

            app.UseAuthentication();    
            app.UseAuthorization();

           /* SeedData.RestartDatabase(context);
            SeedData.SeedWithData(context);
            SeedData.SeedWithProcedure(context);
            SeedData.SeedAccounts(context);*/

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapDefaultControllerRoute();
                endpoints.MapRazorPages();
            });

        }
    }
}