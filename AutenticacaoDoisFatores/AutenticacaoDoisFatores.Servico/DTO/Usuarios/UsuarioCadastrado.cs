using AutenticacaoDoisFatores.Dominio.Compartilhados.Entidades;
using AutenticacaoDoisFatores.Dominio.Compartilhados.Usuarios;
using AutenticacaoDoisFatores.Dominio.Entidades;

namespace AutenticacaoDoisFatores.Servico.DTO.Usuarios
{
    public class UsuarioCadastrado : EntidadeComAuditoria
    {
        public string Nome { get; }
        public string NomeUsuario { get; }
        public string Email { get; }
        public long? Celular { get; }
        public bool Ativo { get; }
        public DateTime? DataUltimoAcesso { get; }
        public TipoDeAutenticacao? TipoDeAutenticacao { get; }

        public UsuarioCadastrado(Guid id, string nome, string nomeUsuario, string email, long? celular, bool ativo, DateTime? dataUltimoAcesso, TipoDeAutenticacao? tipoDeAutenticacao, DateTime dataCadastro, DateTime? dataAlteracao)
        {
            Id = id;
            Nome = nome;
            NomeUsuario = nomeUsuario;
            Email = email;
            Celular = celular;
            Ativo = ativo;
            DataUltimoAcesso = dataUltimoAcesso;
            TipoDeAutenticacao = tipoDeAutenticacao;
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
                celular: usuario.Celular,
                ativo: usuario.Ativo,
                dataUltimoAcesso: usuario.DataUltimoAcesso,
                tipoDeAutenticacao: usuario.TipoDeAutenticacao,
                dataCadastro: usuario.DataCadastro,
                dataAlteracao: usuario.DataAlteracao
            );
        }
    }
}
