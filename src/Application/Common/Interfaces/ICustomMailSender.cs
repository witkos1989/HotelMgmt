namespace Application.Common.Interfaces
{
	public interface ICustomMailSender
	{
        Task SendAlreadyExistEmail(string email, CancellationToken cancellationToken);

        Task SendActivationLinkEmail(string confirmationLink, string email, CancellationToken cancellationToken);

        Task SendTwoStepVerificationCode(string token, string email, CancellationToken cancellationToken);

        Task SendResetPasswordMail(string callback, string email, CancellationToken cancellationToken);
    }
}