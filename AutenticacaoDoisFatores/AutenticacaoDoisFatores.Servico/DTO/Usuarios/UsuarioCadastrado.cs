using AutenticacaoDoisFatores.Dominio.Compartilhados.Entidades;

namespace AutenticacaoDoisFatores.Servico.DTO.Usuarios
{
    public class UsuarioCadastrado : EntidadeComAuditoria
    {
        public string Nome { get; }
        public string NomeUsuario { get; }
        public string Email { get; }
        public bool Ativo { get; }

        public UsuarioCadastrado(Guid id, string nome, string nomeUsuario, string email, bool ativo, DateTime dataCadastro, DateTime? dataAlteracao)
        {
            Id = id;
            Nome = nome;
            NomeUsuario = nomeUsuario;
            Email = email;
            Ativo = ativo;
            DataCadastro = dataCadastro;
            DataAlteracao = dataAlteracao;
        }
    }
}
