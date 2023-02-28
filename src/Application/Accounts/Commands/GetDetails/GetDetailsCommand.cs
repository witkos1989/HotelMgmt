using System.Security.Claims;
using Application.Common.DTOs;
using Application.Common.Interfaces;
using Application.Common.Models;
using MediatR;

namespace Application.Accounts.Commands
{
	public record GetDetailsCommand : IRequest<AccountDetailsDTO?>
	{
        public string? UserId { get; init; }
    }

    public class GetDetailsCommandHandler : IRequestHandler<GetDetailsCommand, AccountDetailsDTO?>
    {
        private readonly IIdentityService _identityService;

        public GetDetailsCommandHandler(IIdentityService identityService)
        {
            _identityService = identityService;
        }

        public async Task<AccountDetailsDTO?> Handle(GetDetailsCommand command, CancellationToken cancellationToken)
        {
            var user = await _identityService.GetUserByIdAsync(command.UserId, cancellationToken);

            return user;
        }
    }
}