//using System;
//using Microsoft.AspNetCore.Hosting;
//using Microsoft.AspNetCore.Identity;
//using Microsoft.AspNetCore.Identity.UI;
//using Microsoft.EntityFrameworkCore;
//using Microsoft.Extensions.Configuration;
//using Microsoft.Extensions.DependencyInjection;
//using Test.Application.Areas.Identity.Data;
//using Test.Data.Contexts;

//[assembly: HostingStartup(typeof(Test.Application.Areas.Identity.IdentityHostingStartup))]
//namespace Test.Application.Areas.Identity
//{
//    public class IdentityHostingStartup : IHostingStartup
//    {
//        public void Configure(IWebHostBuilder builder)
//        {
//            builder.ConfigureServices((context, services) =>
//            {
//                services.AddDbContext<TestApplicationContext>(options =>
//                    options.UseSqlServer(
//                        context.Configuration.GetConnectionString("TestApplicationContextConnection")));

//                services.AddDefaultIdentity<ConnectionId>(options => options.SignIn.RequireConfirmedAccount = true)
//                    .AddEntityFrameworkStores<TestApplicationContext>();
//            });
//        }
//    }
//}