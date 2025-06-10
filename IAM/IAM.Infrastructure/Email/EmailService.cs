using IAM.Application.AuthenticationService.Interfaces;
using IAM.Application.AuthenticationService.ViewModels;
using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace IAM.Infrastructure.Email
{
    public class EmailService : IEmailService
    {

        private readonly static string _gmailEmail = "skydock3@gmail.com";
        private readonly static string _gmailPassword = "umue fqyr jqtb hhgs ";



        public async Task Send(EmailVM emailVM)
        {
            try
            {
                var email = new MimeMessage();
                email.From.Add(MailboxAddress.Parse(_gmailEmail));
                email.To.Add(MailboxAddress.Parse(emailVM.ToEmail));
                email.Subject = emailVM.Subject;
                email.Body = new TextPart("plain") { Text = emailVM.Message };

                using var client = new SmtpClient();

                await client.ConnectAsync("smtp.gmail.com", 587, MailKit.Security.SecureSocketOptions.StartTls);
                await client.AuthenticateAsync(_gmailEmail, _gmailPassword);
                await client.SendAsync(email);
                await client.DisconnectAsync(true);
            }
            catch (AuthenticationException ex)
            {
                throw new Exception("Failed to authenticate with Gmail. " +
                    "Please verify your email and password, and ensure you've enabled " +
                    "either 'Less secure apps' or created an 'App password' if using 2FA.", ex);
            }
            catch (SmtpCommandException ex)
            {
                throw new Exception($"Error sending email: {ex.Message} (Status: {ex.StatusCode})", ex);
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to send email: {ex.Message}", ex);
            }
        }
    }
}
