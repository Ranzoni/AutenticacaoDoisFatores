using AutenticacaoDoisFatores.Domain.Shared;
using AutenticacaoDoisFatores.Domain.Exceptions;
using AutenticacaoDoisFatores.Domain.Services;
using Microsoft.Extensions.Configuration;
using System.Net;
using System.Net.Mail;

namespace AutenticacaoDoisFatores.Infra.Services
{
    public class EmailService : IEmailService
    {
        private readonly string _servidorEmail = "";
        private readonly string _usuarioEmail = "";

        public EmailService(IConfiguration configuration)
        {
            var servidorEmail = configuration["Email:Servidor"];
            if (servidorEmail is null || servidorEmail.IsNullOrEmptyOrWhiteSpaces())
            {
                EmailException.SendingEmailNotFound();
                return;
            }

            var usuarioEmail = Environment.GetEnvironmentVariable("ADF_USUARIO_EMAIL");
            if (usuarioEmail is null || usuarioEmail.IsNullOrEmptyOrWhiteSpaces())
            {
                EmailException.SendingEmailNotFound();
                return;
            }

            _servidorEmail = servidorEmail;
            _usuarioEmail = usuarioEmail;
        }

        public void Send(string to, string subject, string message)
        {
            var credentials = new NetworkCredential(_usuarioEmail, "lwhj zydr ekbu dqaa");

            var smtp = new SmtpClient(_servidorEmail)
            {
                Port = 587,
                EnableSsl = true,
                UseDefaultCredentials = false,
                Credentials = credentials,
            };

            var email = new MailMessage(_usuarioEmail, to)
            {
                Subject = subject,
                Body = message,
                IsBodyHtml = true
            };

            try
            {
                smtp.Send(email);
            }
            catch (Exception ex)
            {
                throw new SmtpException(ex.Message);
            }
        }
    }
}
