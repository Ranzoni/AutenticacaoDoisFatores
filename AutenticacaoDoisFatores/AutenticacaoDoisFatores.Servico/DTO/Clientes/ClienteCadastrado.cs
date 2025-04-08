using AutenticacaoDoisFatores.Dominio.Compartilhados.Entidades;
using AutenticacaoDoisFatores.Dominio.Entidades;

namespace AutenticacaoDoisFatores.Servico.DTO.Clientes
{
    public class ClienteCadastrado : EntidadeComAuditoria
    {
        public string Nome { get; }
        public string Email { get; }
        public string NomeDominio { get; }
        public bool Ativo { get; }

        public ClienteCadastrado(Guid id, string nome, string email, string nomeDominio, bool ativo, DateTime dataCadastro, DateTime? dataAlteracao)
        {
            Id = id;
            Nome = nome;
            Email = email;
            NomeDominio = nomeDominio;
            Ativo = ativo;
            DataCadastro = dataCadastro;
            DataAlteracao = dataAlteracao;
        }

        public static explicit operator ClienteCadastrado(Cliente cliente)
        {
            return new ClienteCadastrado
            (
                id: cliente.Id,
                nome: cliente.Nome,
                email: cliente.Email,
                nomeDominio: cliente.NomeDominio,
                ativo: cliente.Ativo,
                dataCadastro: cliente.DataCadastro,
                dataAlteracao: cliente.DataAlteracao
            );
        }
    }
}
