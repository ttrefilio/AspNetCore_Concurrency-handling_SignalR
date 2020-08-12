using AutoMapper;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using Test.Application.Areas.Identity.Data;
using Test.Application.Areas.Identity.Services;
using Test.Application.Data;
using Test.Application.SignalR.Hubs;
using Test.Application.SignalR.Services;
using Test.Data.Concurrency;
using Test.Data.Contexts;
using Test.Data.Repository;


namespace Test.Application
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;            
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            
            services.AddDbContext<AppDbContext>();
            //services.AddDbContext<TestDbContext>(options =>
            //{
            //    options.UseSqlServer(Configuration.GetConnectionString("TestConnection"));
            //});

            #region Identity Configuration
            services.AddDbContext<IdentityContext>();

            services.AddIdentity<User, IdentityRole>(option =>
            {
                option.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromSeconds(10);
            })
                .AddEntityFrameworkStores<IdentityContext>()
                .AddDefaultTokenProviders();

            services.AddTransient<IEmailSender, EmailSender>();
            services.AddTransient<UserManager<User>>();
            #endregion



            services.AddAutoMapper(typeof(Startup));            
            services.AddControllersWithViews();
            
            services.AddRazorPages();
            services.AddSignalR();
            services.AddCors();
            services.AddHttpContextAccessor();

            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            services.AddScoped<EventRepository>();           
            services.AddScoped<LockRepository>();
            services.AddScoped<SignalREventService>();


        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }     


            app.UseHttpsRedirection();
            app.UseStaticFiles();
            
            app.UseAuthentication();

            app.UseRouting();

            
            app.UseAuthorization();
            app.UseAuthorization();

            app.UseCors(options => options.AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin());

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Events}/{action=Index}/{id?}");                

                endpoints.MapHub<EventHub>("/eventhub");
                endpoints.MapRazorPages();

            });
        }
    }
}
