using AutenticacaoDoisFatores.Dominio.Compartilhados.Permissoes;

namespace AutenticacaoDoisFatores.Servico.DTO.Permissoes
{
    public class PermissaoDisponivel(string nome, TipoDePermissao valor)
    {
        public string Nome { get; } = nome;
        public TipoDePermissao Valor { get; } = valor;
    }
}
