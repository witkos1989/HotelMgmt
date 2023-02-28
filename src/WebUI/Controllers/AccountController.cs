using System.Text.RegularExpressions;
using Application.Common.Interfaces;
using Application.Accounts.Commands;
using Application.Common.Models;
using FluentValidation;
using FluentValidation.Results;
using Infrastructure.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebUI.Services;

namespace WebUI.Controllers
{
    [Route("account")]
    public class AccountController : BaseController
    {
        private readonly ICurrentUserService _currentUserService;
        private readonly IServiceScopeFactory _serviceScopeFactory;

        public AccountController(ICurrentUserService currentUserService,
            IServiceScopeFactory serviceScopeFactory)
        {
            _currentUserService = currentUserService;
            _serviceScopeFactory = serviceScopeFactory;
        }

        [Route("login")]
        [HttpGet]
        [AllowAnonymous]
        [ImportModelState]
        public IActionResult Login(string? returnUrl)
        {
            ViewData["ReturnUrl"] = returnUrl;

            if (UserSignedIn())
            {
                return Redirect("~/");
            }

            return View();
        }

        [Route("login")]
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        [ExportModelState]
        public async Task<IActionResult> Login(LoginCommand command, string? returnUrl)
        {
            ViewData["ReturnUrl"] = returnUrl;

            if (returnUrl == "/")
            {
                returnUrl = "~/";
            }

            using (var scope = _serviceScopeFactory.CreateScope())
            {
                var loginValidator = scope.ServiceProvider.GetRequiredService<IValidator<LoginCommand>>();

                var validation = await ValidateAsync<IValidator<LoginCommand>, LoginCommand>(loginValidator, command);

                if (validation)
                {
                    return RedirectToAction("Login", returnUrl);
                }
            }

            var success = await Mediator.Send(command);

            if (AddErrors(success.Item1))
            {
                return RedirectToAction("Login", returnUrl);
            }

            if (success.Item2)
            {
                return RedirectToAction(nameof(LoginTwoStep), new { command.Name, returnUrl });
            }

            if (!String.IsNullOrEmpty(returnUrl))
            {
                return Redirect(returnUrl);
            }

            return Redirect("~/");
        }

        [Route("logout")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await Mediator.Send(new LogoutCommand());

            return RedirectToAction("Login", "Account");
        }

        [Route("logintwostep")]
        [HttpGet]
        [ImportModelState]
        public async Task<IActionResult> LoginTwoStep(string name, string? returnUrl)
        {
            if (UserSignedIn())
            {
                return Redirect("~/");
            }

            var result = await Mediator.Send(new GetTwoStepLoginCommand() { Name = name });

            if (AddErrors(result))
            {
                return View("Login", returnUrl);
            }

            ViewData["ReturnUrl"] = returnUrl;

            return View();
        }

        [Route("logintwostep")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ExportModelState]
        public async Task<IActionResult> LoginTwoStep(TwoStepLoginCommand command, string? returnUrl)
        {

            using (var scope = _serviceScopeFactory.CreateScope())
            {
                var twoStepValidator = scope.ServiceProvider.GetRequiredService<IValidator<TwoStepLoginCommand>>();

                var validation = await ValidateAsync<IValidator<TwoStepLoginCommand>, TwoStepLoginCommand>(twoStepValidator, command);

                if (validation)
                {
                    return View(command);
                }
            }

            var result = await Mediator.Send(command);

            if (AddErrors(result))
            {
                return View(command);
            }

            if (!String.IsNullOrEmpty(returnUrl))
            {
                if (returnUrl == "/")
                {
                    return Redirect("~/");
                }

                return Redirect(returnUrl);
            }

            return Redirect("~/");
        }

        [Route("register")]
        [HttpGet]
        [AllowAnonymous]
        [ImportModelState]
        public IActionResult Register(string? returnUrl)
        {
            if (UserSignedIn())
            {
                return Redirect("~/");
            }

            ViewData["ReturnUrl"] = returnUrl;

            return View();
        }

        [Route("register")]
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        [ExportModelState]
        public async Task<IActionResult> Register(RegisterCommand command, string? returnUrl)
        {
            bool validation;

            ViewData["ReturnUrl"] = returnUrl;

            try
            {
                using var scope = _serviceScopeFactory.CreateScope();

                var registerValidator = scope.ServiceProvider.GetRequiredService<IValidator<RegisterCommand>>();

                validation = await ValidateAsync<IValidator<RegisterCommand>, RegisterCommand>(registerValidator, command);
            }
            catch (RegexMatchTimeoutException)
            {
                ModelState.AddModelError("Email", "Niepoprawny format");

                return RedirectToAction("Register", returnUrl);
            }

            if (validation)
            {
                return RedirectToAction("Register", returnUrl);
            }

            command.Url = Url.Action("confirmemail", "account", null, Request.Scheme);

            var result = await Mediator.Send(command);

            if (AddErrors(result))
            {
                return RedirectToAction("Register", returnUrl);
            }

            return View(nameof(SuccessRegistration));
        }

        [Route("successregistration")]
        [HttpGet]
        public IActionResult SuccessRegistration()
        {
            if (UserSignedIn())
            {
                return Redirect("~/");
            }

            return View();
        }

        [Route("confirmemail")]
        [HttpGet]
        public async Task<IActionResult> ConfirmEmail(string token, string email)
        {
            if (UserSignedIn())
            {
                return Redirect("~/");
            }

            var result = await Mediator.Send(new ConfirmMailCommand(email, token));

            if (result.Succeeded)
            {
                return View(nameof(ConfirmEmail));
            }

            return View("Error", "Home");
        }

        [Route("forgotpassword")]
        [HttpGet]
        [ImportModelState]
        public IActionResult ForgotPassword()
        {
            if (UserSignedIn())
            {
                return Redirect("~/");
            }

            return View();
        }

        [Route("forgotpassword")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ExportModelState]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordCommand command)
        {
            using (var scope = _serviceScopeFactory.CreateScope())
            {
                var forgotPasswordValidator = scope.ServiceProvider.GetRequiredService<IValidator<ForgotPasswordCommand>>();

                var validation = await ValidateAsync<IValidator<ForgotPasswordCommand>, ForgotPasswordCommand>(forgotPasswordValidator, command);

                if (validation)
                {
                    return RedirectToAction("ForgotPassword");
                }
            }

            command.Url = Url.Action("resetpassword", "account", null, Request.Scheme);

            var result = await Mediator.Send(command);

            if (AddErrors(result))
            {
                return RedirectToAction("ForgotPassword");
            }

            return RedirectToAction(nameof(ForgotPasswordConfirmation));
        }

        [Route("forgotpasswordconfirmation")]
        [HttpGet]
        public IActionResult ForgotPasswordConfirmation()
        {
            if (UserSignedIn())
            {
                return Redirect("~/");
            }

            return View();
        }

        [Route("resetpassword")]
        [HttpGet]
        [ImportModelState]
        public IActionResult ResetPassword(string token, string email)
        {
            if (UserSignedIn())
            {
                return Redirect("~/");
            }

            var model = new ResetPasswordCommand { Token = token, Email = email };

            return View(model);
        }

        [Route("resetpassword")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ExportModelState]
        public async Task<IActionResult> ResetPassword(ResetPasswordCommand command)
        {
            using (var scope = _serviceScopeFactory.CreateScope())
            {
                var resetPasswordValidator = scope.ServiceProvider.GetRequiredService<IValidator<ResetPasswordCommand>>();

                var validation = await ValidateAsync<IValidator<ResetPasswordCommand>, ResetPasswordCommand>(resetPasswordValidator, command);

                if (validation)
                {
                    return RedirectToAction("ResetPassword", command.Token, command.Email);
                }
            }

            var result = await Mediator.Send(command);

            if (AddErrors(result))
            {
                return RedirectToAction("ResetPassword", command.Token, command.Email);
            }

            return RedirectToAction(nameof(ResetPasswordConfirmation));
        }

        [Route("resetpasswordconfirmation")]
        [HttpGet]
        public IActionResult ResetPasswordConfirmation()
        {
            if (UserSignedIn())
            {
                return Redirect("~/");
            }

            return View();
        }

        [Route("details")]
        [HttpGet]
        [Authorize]
        [ImportModelState]
        public async Task<IActionResult> Details()
        {
            if (_currentUserService.UserId is null)
            {
                return RedirectToAction("Login", "Account", "/account/details");
            }

            var result = await Mediator.Send(new GetDetailsCommand() { UserId = _currentUserService.UserId });

            if (result == null)
            {
                ModelState.AddModelError(string.Empty, "Nie można odczytać danych użytkownika");
            }

            return View(result);
        }

        [Route("profile")]
        [HttpPost]
        [Authorize]
        [ExportModelState]
        public async Task<IActionResult> Profile(ProfileCommand command)
        {
            using (var scope = _serviceScopeFactory.CreateScope())
            {
                var profileValidator = scope.ServiceProvider.GetRequiredService<IValidator<ProfileCommand>>();

                var validation = await ValidateAsync<IValidator<ProfileCommand>, ProfileCommand>(profileValidator, command);

                if (validation)
                {
                    return RedirectToAction("Details");
                }
            }

            command.LoggedUserId = _currentUserService.UserId;

            var result = await Mediator.Send(command);

            if (AddErrors(result))
            {
                return RedirectToAction("Details");
            }

            TempData["Success"] = "Dane zostały pomyślnie zapisane";

            return RedirectToAction("Details");
        }

        [Route("password")]
        [HttpPost]
        [Authorize]
        [ExportModelState]
        public async Task<IActionResult> Password(PasswordCommand command)
        {
            TempData["SecondView"] = "true";

            using (var scope = _serviceScopeFactory.CreateScope())
            {
                var passwordValidator = scope.ServiceProvider.GetRequiredService<IValidator<PasswordCommand>>();

                var validation = await ValidateAsync<IValidator<PasswordCommand>, PasswordCommand>(passwordValidator, command);

                if (validation)
                {
                    return RedirectToAction("Details");
                }
            }

            command.LoggedUserId = User.Identity?.Name;

            var result = await Mediator.Send(command);

            if (AddErrors(result))
            {
                return RedirectToAction("Details");
            }

            TempData["Success"] = "Hasło zostało pomyślnie zmienione";

            return RedirectToAction("Details");
        }
    }
}