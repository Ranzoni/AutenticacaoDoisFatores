using AutenticacaoDoisFatores.Dominio.Compartilhados;
using AutenticacaoDoisFatores.Dominio.Excecoes;
using AutenticacaoDoisFatores.Dominio.Servicos;
using OtpNet;
using System.Text;

namespace AutenticacaoDoisFatores.Infra.Servicos
{
    public class ServicoDeAutenticador : IServicoDeAutenticador
    {
        private readonly string _app = "";

        public ServicoDeAutenticador()
        {
            var app = Environment.GetEnvironmentVariable("ADF_CHAVE_APP_AUTENTICADOR");
            if (app is null || app.EstaVazio())
            {
                ExcecoesAppAutenticador.ChaveSecretaNaoEncontrada();
                return;
            }

            _app = app;
        }

        public string GerarQrCode(string email, string chaveSecreta)
        {
            return $"otpauth://totp/{_app}:{email}?secret={chaveSecreta}&issuer={_app}";
        }

        public bool CodigoEhValido(string codigo, string chaveSecreta)
        {
            var totp = new Totp(Encoding.UTF8.GetBytes(chaveSecreta));
            return totp.VerifyTotp(codigo, out _);
        }
    }
}
