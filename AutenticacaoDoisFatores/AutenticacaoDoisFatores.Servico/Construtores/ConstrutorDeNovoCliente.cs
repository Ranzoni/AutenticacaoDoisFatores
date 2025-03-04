using AutenticacaoDoisFatores.Servico.DTO.Clientes;

namespace AutenticacaoDoisFatores.Servico.Construtores
{
    public class ConstrutorDeNovoCliente
    {
        private string _nome = "";
        private string _email = "";
        private string _nomeDominio = "";
        private string _senhaAdm = "";

        public ConstrutorDeNovoCliente ComNome(string nome)
        {
            _nome = nome;

            return this;
        }

        public ConstrutorDeNovoCliente ComEmail(string email)
        {
            _email = email;

            return this;
        }

        public ConstrutorDeNovoCliente ComNomeDominio(string nomeDominio)
        {
            _nomeDominio = nomeDominio;

            return this;
        }

        public ConstrutorDeNovoCliente ComSenhaAdm(string senhaAdm)
        {
            _senhaAdm = senhaAdm;

            return this;
        }

        public NovoCliente Construir()
        {
            var novoCliente = new NovoCliente
                (
                    nome: _nome,
                    email: _email,
                    nomeDominio: _nomeDominio,
                    senhaAdm: _senhaAdm
                );

            return novoCliente;
        }
    }
}
