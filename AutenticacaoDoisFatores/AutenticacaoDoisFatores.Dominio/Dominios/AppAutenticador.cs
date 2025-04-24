using AutenticacaoDoisFatores.Dominio.Compartilhados;
using AutenticacaoDoisFatores.Dominio.Entidades;
using AutenticacaoDoisFatores.Dominio.Excecoes;
using AutenticacaoDoisFatores.Dominio.Servicos;

namespace AutenticacaoDoisFatores.Dominio.Dominios
{
    public partial class AppAutenticador(IServicoDeAutenticador servico)
    {
        private readonly IServicoDeAutenticador _servico = servico;

        public string? GerarQrCode(Usuario usuario)
        {
            if (usuario is null)
                ExcecoesUsuario.UsuarioNaoEncontrado();

            return _servico.GerarQrCode(usuario!.Email, usuario!.ChaveSecreta);
        }

        public bool CodigoEhValido(string codigo, Usuario usuario)
        {
            if (codigo.EstaVazio())
                ExcecoesAppAutenticador.CodigoNaoInformado();

            return _servico.CodigoEhValido(codigo, usuario!.ChaveSecreta);
        }
    }
}
