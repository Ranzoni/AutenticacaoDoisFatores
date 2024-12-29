using AutenticacaoDoisFatores.Dominio.Entidades;

namespace AutenticacaoDoisFatores.Dominio.Construtores
{
    public class ConstrutorDeCliente
    {
        private string _nome = "";
        private string _email = "";

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

        public Cliente ConstruirNovoCliente()
        {
            var cliente = new Cliente(nome: _nome, email: _email);

            return cliente;
        }
    }
}
