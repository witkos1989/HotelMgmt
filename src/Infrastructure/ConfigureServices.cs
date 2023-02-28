using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using static Infrastructure.TokenProviders.EmailConfirmationTokenProvider<Infrastructure.Identity.ApplicationUser>;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Application.Common.Interfaces;
using Infrastructure.Identity;
using Infrastructure.Persistence;
using Infrastructure.Persistence.Interceptors;
using Infrastructure.Services;
using Infrastructure.TokenProviders;

namespace Microsoft.Extensions.DependencyInjection;

public static class ConfigureServices
{
    public static IServiceCollection AddInfrastructureServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddScoped<AuditableEntitySaveChangesInterceptor>();

        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlite(configuration.GetConnectionString("DefaultConnection"),
            m => m.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName)));

        services.AddDatabaseDeveloperPageExceptionFilter();

        services.AddDefaultIdentity<ApplicationUser>(options =>
        {
            options.SignIn.RequireConfirmedAccount = true;
            options.SignIn.RequireConfirmedEmail = true;
            options.Tokens.EmailConfirmationTokenProvider = "emailconfirmation";
        }).AddEntityFrameworkStores<ApplicationDbContext>()
          .AddDefaultTokenProviders()
          .AddTokenProvider<EmailConfirmationTokenProvider<ApplicationUser>>("emailconfirmation");

        services.AddTransient<IDateTime, DateTimeService>();
        services.AddTransient<IIdentityService, IdentityService>();
        services.AddTransient<ISigningManager, SigningManager>();

        var emailString = configuration
            .GetSection("EmailConfiguration")
            .Get<EmailConfiguration>();
        if (emailString is not null)
        {
            services.AddSingleton<IEmailConfiguration>(emailString);
        }
        services.AddTransient<IEmailService, EmailService>();
        services.AddTransient<ICustomMailSender, CustomMailSender>();

        services.Configure<EmailConfirmationTokenProviderOptions>(
            option => option.TokenLifespan = TimeSpan.FromDays(2));
        services.Configure<DataProtectionTokenProviderOptions>(
            opt => opt.TokenLifespan = TimeSpan.FromHours(2));
        services.AddSession(options =>
        {
            options.IdleTimeout = TimeSpan.FromMinutes(1);
            options.Cookie.IsEssential = true;
        });

        return services;
    }
}