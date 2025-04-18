using AutenticacaoDoisFatores.Dominio.Entidades;

namespace AutenticacaoDoisFatores.Servico.CasosDeUso.Usuarios.Autenticadores
{
    internal interface ITipoDeAutenticador
    {
        Task<object?> ExecutarAsync(Usuario usuario);
    }
}
