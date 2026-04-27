using ASC.Web.Models;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;

namespace ASC.Web.Services;

public class AuthMessageSender : IEmailSender, ISmsSender
{
    private readonly EmailSettings _settings;
    private readonly ILogger<AuthMessageSender> _logger;

    public AuthMessageSender(IOptions<EmailSettings> options, ILogger<AuthMessageSender> logger)
    {
        _settings = options.Value;
        _logger = logger;
    }

    public async Task SendEmailAsync(string email, string subject, string htmlMessage)
    {
        var fromEmail = string.IsNullOrWhiteSpace(_settings.SenderEmail) ? _settings.Mail : _settings.SenderEmail;
        var username = string.IsNullOrWhiteSpace(_settings.Username) ? _settings.Mail : _settings.Username;
        var smtpHost = string.IsNullOrWhiteSpace(_settings.SmtpServer) ? _settings.Host : _settings.SmtpServer;

        if (string.IsNullOrWhiteSpace(fromEmail) ||
            string.IsNullOrWhiteSpace(username) ||
            string.IsNullOrWhiteSpace(smtpHost) ||
            string.IsNullOrWhiteSpace(_settings.Password))
        {
            _logger.LogWarning("Email settings are not configured. Skip sending email to {Email}.", email);
            return;
        }

        var message = new MimeMessage();
        var displayName = string.IsNullOrWhiteSpace(_settings.SenderName) ? fromEmail : _settings.SenderName;
        message.From.Add(new MailboxAddress(displayName, fromEmail));
        message.To.Add(MailboxAddress.Parse(email));
        message.Subject = subject;
        message.Body = new TextPart("html")
        {
            Text = htmlMessage
        };

        using var client = new SmtpClient();
        await client.ConnectAsync(smtpHost, _settings.Port, _settings.UseSsl ? SecureSocketOptions.SslOnConnect : SecureSocketOptions.StartTls);
        await client.AuthenticateAsync(username, _settings.Password);
        await client.SendAsync(message);
        await client.DisconnectAsync(true);
    }

    public Task SendSmsAsync(string number, string message)
    {
        _logger.LogInformation("SMS sender is not configured. Skip sending SMS to {Number}.", number);
        return Task.CompletedTask;
    }
}
