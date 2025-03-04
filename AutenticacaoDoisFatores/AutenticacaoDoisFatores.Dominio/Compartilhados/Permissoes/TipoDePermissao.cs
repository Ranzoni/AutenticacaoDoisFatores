using System.ComponentModel;

namespace AutenticacaoDoisFatores.Dominio.Compartilhados.Permissoes
{
    public enum TipoDePermissao
    {
        [Description("Criar um usuário")]
        CriarUsuario,
        [Description("Inativar usuário")]
        AtivarUsuario
    }
}
