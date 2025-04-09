using System.ComponentModel;

namespace AutenticacaoDoisFatores.Dominio.Compartilhados.Permissoes
{
    public enum TipoDePermissao
    {
        [Description("Criar um usuário")]
        CriarUsuario,
        [Description("Ativar usuário")]
        AtivarUsuario,
        [Description("Inativar usuário")]
        DesativarUsuario,
        [Description("Trocar a senha de um usuário")]
        TrocarSenhaUsuario,
        [Description("Definir as permissões de um usuário")]
        DefinirPermissoes,
        [Description("Excluir usuário")]
        ExcluirUsuario,
        [Description("Visualizar usuários")]
        VisualizacaoDeUsuarios
    }
}
