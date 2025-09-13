using CleanArchitecture.Application.Common.Abstractions;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;

namespace CleanArchitecture.Infrastructure.Email;

public sealed class SmtpEmailSender : IEmailSender
{
    private readonly EmailSettings _settings;

    public SmtpEmailSender(IOptions<EmailSettings> options)
    => _settings = options.Value;

    public async Task SendAsync(
    string to,
    string subject,
    string? textBody = null,
    string? htmlBody = null,
    CancellationToken cancellationToken = default)
    {
        var message = new MimeMessage();
        message.From.Add(new MailboxAddress(_settings.FromName, _settings.FromAddress));
        message.To.Add(MailboxAddress.Parse(to));
        message.Subject = subject;

        var body = new BodyBuilder
        {
            TextBody = textBody,
            HtmlBody = htmlBody
        };
        message.Body = body.ToMessageBody();

        using var client = new SmtpClient();

        // Sertifika doğrulamayı KAPATMAYIN. Sadece gerekli ise doğru CA/sertifika ile çözün.
        // client.ServerCertificateValidationCallback = (s, c, h, e) => true; // ❌ güvenlik riski

        var socket = _settings.UseStartTls
            ? SecureSocketOptions.StartTls
            : SecureSocketOptions.Auto;

        await client.ConnectAsync(_settings.Host, _settings.Port, socket, cancellationToken);
        await client.AuthenticateAsync(_settings.UserName, _settings.Password, cancellationToken);
        await client.SendAsync(message, cancellationToken);
        await client.DisconnectAsync(true, cancellationToken);
    }
}