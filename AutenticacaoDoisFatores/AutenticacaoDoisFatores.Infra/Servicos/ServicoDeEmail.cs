using AutenticacaoDoisFatores.Dominio.Compartilhados;
using AutenticacaoDoisFatores.Dominio.Excecoes;
using AutenticacaoDoisFatores.Dominio.Servicos;
using Microsoft.Extensions.Configuration;
using System.Net;
using System.Net.Mail;

namespace AutenticacaoDoisFatores.Infra.Servicos
{
    public class ServicoDeEmail : IServicoDeEmail
    {
        private readonly string _servidorEmail = "";
        private readonly string _usuarioEmail = "";

        public ServicoDeEmail(IConfiguration configuration)
        {
            var servidorEmail = configuration["Email:Servidor"];
            if (servidorEmail is null || servidorEmail.EstaVazio())
            {
                ExcecoesEmail.EnderecoEmailEnvioNaoConfigurado();
                return;
            }

            var usuarioEmail = Environment.GetEnvironmentVariable("ADF_USUARIO_EMAIL");
            if (usuarioEmail is null || usuarioEmail.EstaVazio())
            {
                ExcecoesEmail.EnderecoEmailEnvioNaoConfigurado();
                return;
            }

            _servidorEmail = servidorEmail;
            _usuarioEmail = usuarioEmail;
        }

        public void Enviar(string para, string titulo, string mensagem)
        {
            var credenciais = new NetworkCredential(_usuarioEmail, "lwhj zydr ekbu dqaa");

            var smtp = new SmtpClient(_servidorEmail)
            {
                Port = 587,
                EnableSsl = true,
                UseDefaultCredentials = false,
                Credentials = credenciais,
            };

            var email = new MailMessage(_usuarioEmail, para)
            {
                Subject = titulo,
                Body = mensagem,
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
