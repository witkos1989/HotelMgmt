using System.Reflection;
using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Application.Common.Security;
using MediatR;

namespace Application.Common.Behaviours
{
    public class AuthorizationBehaviour<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
        where TRequest : IRequest<TResponse>
    {
        private readonly ICurrentUserService _currentUserService;
        private readonly IIdentityService _identityService;

        public AuthorizationBehaviour(
            ICurrentUserService currentUserService,
            IIdentityService identityService
            )
        {
            _currentUserService = currentUserService;
            _identityService = identityService;
        }

        public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
        {
            var authorizeAttributes = request.GetType().GetCustomAttributes<AuthorizeAttribute>();

            if (authorizeAttributes.Any())
            {
                if (_currentUserService.UserId == null)
                {
                    throw new UnauthorizedAccessException();
                }

                await AuthorizeByRole(authorizeAttributes, _currentUserService.UserId, cancellationToken);

                await AuthorizeByPolicy(authorizeAttributes, _currentUserService.UserId, cancellationToken);
            }

            return await next();
        }

        private async Task AuthorizeByPolicy(IEnumerable<AuthorizeAttribute> authorizeAttributes, string userId, CancellationToken cancellationToken)
        {
            var authorizeAttributesWithPolicies = authorizeAttributes.Where(a => !string.IsNullOrWhiteSpace(a.Policy));

            if (authorizeAttributesWithPolicies.Any())
            {
                var selectedAuthorizeAttributesWithPolicies = authorizeAttributesWithPolicies.Select(a => a.Policy);

                foreach (var policy in selectedAuthorizeAttributesWithPolicies)
                {
                    var authorized = await _identityService.AuthorizeAsync(userId, policy, cancellationToken);

                    if (!authorized)
                    {
                        throw new ForbiddenAccessException();
                    }
                }
            }
        }

        private async Task AuthorizeByRole(IEnumerable<AuthorizeAttribute> authorizeAttributes, string userId, CancellationToken cancellationToken)
        {
            var authorizeAttributeWithRoles = authorizeAttributes.Where(a => !string.IsNullOrWhiteSpace(a.Roles));

            if (authorizeAttributeWithRoles.Any())
            {
                var authorized = false;

                var splittedAuthorizeAttributeRoles = authorizeAttributeWithRoles.Select(a => a.Roles.Split(','));

                foreach (var roles in splittedAuthorizeAttributeRoles)
                {
                    foreach (var role in roles)
                    {
                        var isInRole = await _identityService.IsInRoleAsync(userId, role.Trim(), cancellationToken);

                        if (isInRole)
                        {
                            authorized = true;
                            break;
                        }
                    }
                }

                if (!authorized)
                {
                    throw new ForbiddenAccessException();
                }
            }
        }
    }
}