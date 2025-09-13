namespace CleanArchitecture.Application.Common.Abstractions;

public interface IEmailSender
{
    Task SendAsync(
        string to,
        string subject,
        string? textBody = null,
        string? htmlBody = null,
        CancellationToken cancellationToken = default);
}
