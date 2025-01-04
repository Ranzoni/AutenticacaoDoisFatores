using AutenticacaoDoisFatores.Dominio.Entidades;

namespace AutenticacaoDoisFatores.Dominio.Construtores
{
    public class ConstrutorDeCliente
    {
        private Guid _id;
        private string _nome = "";
        private string _email = "";
        private string _nomeDominio = "";
        private Guid _chaveAcesso;
        private bool _ativo;
        private DateTime _dataCadastro;
        private DateTime? _dataAlteracao;

        public ConstrutorDeCliente ComId(Guid id)
        {
            _id = id;

            return this;
        }

        public ConstrutorDeCliente ComNome(string nome)
        {
            _nome = nome;

            return this;
        }

        public ConstrutorDeCliente ComEmail(string email)
        {
            _email = email;

            return this;
        }

        public ConstrutorDeCliente ComNomeDominio(string nomeDominio)
        {
            _nomeDominio = nomeDominio;

            return this;
        }

        public ConstrutorDeCliente ComChaveAcesso(Guid chaveAcesso)
        {
            _chaveAcesso = chaveAcesso;

            return this;
        }

        public ConstrutorDeCliente ComAtivo(bool ativo)
        {
            _ativo = ativo;

            return this;
        }

        public ConstrutorDeCliente ComDataCadastro(DateTime dataCadastro)
        {
            _dataCadastro = dataCadastro;

            return this;
        }

        public ConstrutorDeCliente ComDataAlteracao(DateTime dataAlteracao)
        {
            _dataAlteracao = dataAlteracao;

            return this;
        }

        public Cliente ConstruirNovoCliente()
        {
            var cliente = new Cliente(nome: _nome, email: _email, nomeDominio: _nomeDominio);

            return cliente;
        }

        public Cliente ConstruirClienteCompleto()
        {
            var cliente = new Cliente
            (
                id: _id,
                nome: _nome,
                email: _email,
                nomeDominio: _nomeDominio,
                chaveAcesso: _chaveAcesso,
                ativo: _ativo,
                dataCadastro: _dataCadastro,
                dataAlteracao: _dataAlteracao
            );

            return cliente;
        }
    }
}
