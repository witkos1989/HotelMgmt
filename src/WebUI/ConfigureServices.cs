using Application.Common.Interfaces;
using Infrastructure.Persistence;
using WebUI.Filters;
using WebUI.Services;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Mvc;

namespace Microsoft.Extensions.DependencyInjection;

public static class ConfigureServices
{
    public static IServiceCollection AddWebUIServices(this IServiceCollection services)
    {
        services.AddDatabaseDeveloperPageExceptionFilter();

        services.AddSingleton<ICurrentUserService, CurrentUserService>();

        services.AddHttpContextAccessor();

        services.AddMvc(options => options.ModelValidatorProviders.Clear());

        services.AddHealthChecks()
            .AddDbContextCheck<ApplicationDbContext>();

        services.AddControllersWithViews(options =>
            options.Filters.Add<ApiExceptionFilterAttribute>());

        services.AddFluentValidationClientsideAdapters();

        services.AddFluentValidationAutoValidation();

        services.AddRazorPages();

        services.ConfigureApplicationCookie(options =>
        {
            options.LoginPath = "/account/login";
            options.LogoutPath = "/account/logout";
        });

        return services;
    }
}