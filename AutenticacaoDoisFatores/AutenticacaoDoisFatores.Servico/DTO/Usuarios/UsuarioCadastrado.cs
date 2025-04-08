using AutenticacaoDoisFatores.Dominio.Compartilhados.Entidades;
using AutenticacaoDoisFatores.Dominio.Entidades;

namespace AutenticacaoDoisFatores.Servico.DTO.Usuarios
{
    public class UsuarioCadastrado : EntidadeComAuditoria
    {
        public string Nome { get; }
        public string NomeUsuario { get; }
        public string Email { get; }
        public bool Ativo { get; }
        public DateTime? DataUltimoAcesso { get; }

        public UsuarioCadastrado(Guid id, string nome, string nomeUsuario, string email, bool ativo, DateTime? dataUltimoAcesso, DateTime dataCadastro, DateTime? dataAlteracao)
        {
            Id = id;
            Nome = nome;
            NomeUsuario = nomeUsuario;
            Email = email;
            Ativo = ativo;
            DataUltimoAcesso = dataUltimoAcesso;
            DataCadastro = dataCadastro;
            DataAlteracao = dataAlteracao;
        }
    
        public static explicit operator UsuarioCadastrado(Usuario usuario)
        {
            return new UsuarioCadastrado
            (
                id: usuario.Id,
                nome: usuario.Nome,
                nomeUsuario: usuario.NomeUsuario,
                email: usuario.Email,
                ativo: usuario.Ativo,
                dataUltimoAcesso: usuario.DataUltimoAcesso,
                dataCadastro: usuario.DataCadastro,
                dataAlteracao: usuario.DataAlteracao
            );
        }
    }
}
