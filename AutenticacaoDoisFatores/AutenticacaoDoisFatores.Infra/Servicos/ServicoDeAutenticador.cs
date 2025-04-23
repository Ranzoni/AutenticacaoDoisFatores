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
        private readonly byte[] _chaveSecreta = [];

        public ServicoDeAutenticador()
        {
            var chaves = Environment.GetEnvironmentVariable("ADF_CHAVE_APP_AUTENTICADOR");
            if (chaves is null || chaves.EstaVazio())
            {
                ExcecoesAppAutenticador.ChaveSecretaNaoEncontrada();
                return;
            }

            var chavesEmArray = chaves.Split(";");
            _app = chavesEmArray[0];
            var chaveSecretaEmString = chavesEmArray[1];
            _chaveSecreta = Encoding.UTF8.GetBytes(chaveSecretaEmString);
        }

        public string GerarQrCode(string email)
        {
            return $"otpauth://totp/{_app}:{email}?secret={Base32Encoding.ToString(_chaveSecreta)}&issuer={_app}";
        }

        public bool CodigoEhValido(string codigo)
        {
            var totp = new Totp(_chaveSecreta);
            return totp.VerifyTotp(codigo, out _);
        }
    }
}
