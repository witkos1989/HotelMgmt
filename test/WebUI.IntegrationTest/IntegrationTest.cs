using Infrastructure.Identity;
using Infrastructure.Persistence;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace WebUI.IntegrationTest
{
    public class IntegrationTest<TProgram>
        : WebApplicationFactory<TProgram> where TProgram : class
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureServices(services =>
            {
                services.RemoveAll(typeof(ApplicationDbContext));
                services.AddDbContext<ApplicationDbContext>(options => { options.UseInMemoryDatabase("TestDb"); });
                services.AddIdentityCore<ApplicationUser>(options =>
                {
                    options.SignIn.RequireConfirmedAccount = false;
                    options.SignIn.RequireConfirmedEmail = false;
                }).AddEntityFrameworkStores<ApplicationDbContext>(); ;
            });

            builder.UseEnvironment("Development");
        }
    }
}