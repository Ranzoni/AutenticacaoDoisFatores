using System.ComponentModel;

namespace AutenticacaoDoisFatores.Dominio.Compartilhados.Permissoes
{
    public enum TipoDePermissao
    {
        [Description("Criar um usuário")]
        CriarUsuario = 1,
        [Description("Ativar usuário")]
        AtivarUsuario = 2,
        [Description("Inativar usuário")]
        DesativarUsuario = 3,
        [Description("Definir as permissões de um usuário")]
        DefinirPermissoes = 4,
        [Description("Excluir usuário")]
        ExcluirUsuario = 5
    }
}
