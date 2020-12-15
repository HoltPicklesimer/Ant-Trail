using System;
using BugTracker2.Areas.Identity.Data;
using BugTracker2.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

[assembly: HostingStartup(typeof(BugTracker2.Areas.Identity.IdentityHostingStartup))]
namespace BugTracker2.Areas.Identity
{
    public class IdentityHostingStartup : IHostingStartup
    {
        public void Configure(IWebHostBuilder builder)
        {
            builder.ConfigureServices((context, services) =>
            {
                services.AddDbContext<BugTracker2Context>(options =>
                    options.UseSqlServer(
                        context.Configuration.GetConnectionString("BugTracker2ContextConnection")));

                services.AddDefaultIdentity<User>(options =>
                {
                    options.SignIn.RequireConfirmedAccount = false;
                })
                    .AddEntityFrameworkStores<BugTracker2Context>();
            });
        }
    }
}