using AutenticacaoDoisFatores.Servico.DTO.Usuarios;

namespace AutenticacaoDoisFatores.Servico.Construtores
{
    public class ConstrutorDeEnvioEmailParaUsuario
    {
        private string _titulo = "";
        private string _mensagem = "";
        private string _htmlEmail = "";

        public ConstrutorDeEnvioEmailParaUsuario ComTitulo(string titulo)
        {
            _titulo = titulo;

            return this;
        }

        public ConstrutorDeEnvioEmailParaUsuario ComMensagem(string mensagem)
        {
            _mensagem = mensagem;

            return this;
        }

        public ConstrutorDeEnvioEmailParaUsuario ComHtmlEmail(string htmlEmail)
        {
            _htmlEmail = htmlEmail;

            return this;
        }

        public EnvioEmailParaUsuario Construir()
        {
            return new EnvioEmailParaUsuario
            (
                titulo: _titulo,
                mensagem: _mensagem,
                htmlEmail: _htmlEmail
            );
        }
    }
}
