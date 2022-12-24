using MailKit.Net.Smtp;
using Microsoft.Extensions.Options;
using MimeKit.Text;
using MimeKit;
using MedicalAppointment.Application.Email;

namespace MedicalAppointment.Application.Service; 
public class EmailService  {
    private readonly EmailConfig _emailSend;

    public EmailService(IOptions<EmailConfig> emailSend) {
        _emailSend = emailSend.Value;
    }

    public Task SendEmailAsync(EmailSend emailSend) {
        return Execute(emailSend);
    }

    private Task Execute(EmailSend emailSend) {
        try {
            // create message
            MimeMessage email = new MimeMessage();
            email.Sender = MailboxAddress.Parse(_emailSend.Sender_EMail);
            if (!string.IsNullOrEmpty(_emailSend.Sender_Name)) {
                email.Sender.Name = _emailSend.Sender_Name;
            }
            email.From.Add(email.Sender);
            email.To.Add(MailboxAddress.Parse(emailSend.ToAddress));
            email.Subject = emailSend.Subject;
            email.Body = new TextPart(TextFormat.Html) { Text = emailSend.Body };

            // send email
            using (var smtp = new SmtpClient()) {
                smtp.Connect(_emailSend.Host_Address, _emailSend.Host_Port, _emailSend.Host_SecureSocketOptions);
                smtp.Authenticate(_emailSend.Host_Username, _emailSend.Host_Password);
                smtp.Send(email);
                smtp.Disconnect(true);
            }

            return Task.FromResult(true);
        } catch (Exception ex) {
            throw new Exception($"Erro ao tentar enviar email, {ex}");
        }
    }

}
