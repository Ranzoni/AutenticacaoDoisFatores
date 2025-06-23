using System.ComponentModel;

namespace AutenticacaoDoisFatores.Domain.Shared.Permissions
{
    public enum PermissionType
    {
        [Description("Criar um usuário")]
        CreateUser,
        [Description("Ativar usuário")]
        ActivateUser,
        [Description("Inativar usuário")]
        InactivateUser,
        [Description("Trocar a senha de um usuário")]
        ChangeUserPassword,
        [Description("Definir as permissões de um usuário")]
        SetPermissions,
        [Description("Excluir usuário")]
        RemoveUser,
        [Description("Visualizar os usuários")]
        ViewUsers
    }
}
