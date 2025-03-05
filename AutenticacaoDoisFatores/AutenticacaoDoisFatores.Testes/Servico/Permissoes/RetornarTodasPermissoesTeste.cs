using AutenticacaoDoisFatores.Dominio.Compartilhados;
using AutenticacaoDoisFatores.Dominio.Compartilhados.Permissoes;
using AutenticacaoDoisFatores.Servico.CasosDeUso.Permissoes;
using AutenticacaoDoisFatores.Servico.DTO.Permissoes;

namespace AutenticacaoDoisFatores.Testes.Servico.Permissoes
{
    public class RetornarTodasPermissoesTeste
    {
        [Fact]
        internal void DeveRetornarTodasPermissoes()
        {
            #region Preparação do teste

            var permissoesEsperadas = Enum.GetValues<TipoDePermissao>()
                .Select(tipo => new PermissaoDisponivel(tipo.Descricao() ?? "", tipo));

            #endregion

            var retorno = RetornarTodasPermissoes.Executar();

            #region Verificação do teste

            Assert.Equal(permissoesEsperadas.Count(), retorno.Count());
            for (var i = 0; i <= permissoesEsperadas.Count() - 1; i++)
            {
                var permissaoEsperada = permissoesEsperadas.ToArray()[i];
                var itemDoRetorno = retorno.ToArray()[i];

                Assert.Equal(permissaoEsperada.Nome, itemDoRetorno.Nome);
                Assert.Equal(permissaoEsperada.Valor, itemDoRetorno.Valor);
            }

            #endregion
        }
    }
}
