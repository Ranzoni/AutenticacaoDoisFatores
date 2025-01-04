using AutenticacaoDoisFatores.Dominio.Compartilhados.Entidades;

namespace AutenticacaoDoisFatores.Servico.DTO
{
    public class ClienteCadastrado : EntidadeComAuditoria
    {
        public string Nome { get; }
        public string Email { get; }
        public string NomeDominio { get; }
        public Guid ChaveAcesso { get; }
        public bool Ativo { get; }

        public ClienteCadastrado(Guid id, string nome, string email, string nomeDominio, Guid chaveAcesso, bool ativo, DateTime dataCadastro, DateTime? dataAlteracao)
        {
            Id = id;
            Nome = nome;
            Email = email;
            NomeDominio = nomeDominio;
            ChaveAcesso = chaveAcesso;
            Ativo = ativo;
            DataCadastro = dataCadastro;
            DataAlteracao = dataAlteracao;
        }
    }
}
