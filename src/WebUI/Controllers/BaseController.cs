using Application.Common.Models;
using FluentValidation;
using FluentValidation.Results;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using WebUI.Services;

namespace WebUI.Controllers
{
	public abstract class BaseController : Controller
	{
        private ISender _mediator = null!;

        protected ISender Mediator => _mediator ??= HttpContext.RequestServices.GetRequiredService<ISender>();

        protected async Task<bool> ValidateAsync<IValidator, T>(IValidator<T> validator, T command)
        {
            var result = new ValidationResult();

            result = await validator.ValidateAsync(command);

            if (!result.IsValid)
            {
                result.AddToModelState(this.ModelState);

                return true;
            }

            return false;
        }

        protected bool AddErrors(Result result)
        {
            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error);
                }

                return true;
            }

            return false;
        }

        protected bool UserSignedIn()
        {
            if (User.Identity is not null)
            {
                if (User.Identity.IsAuthenticated)
                {
                    return true;
                }
            }

            return false;
        }
    }
}