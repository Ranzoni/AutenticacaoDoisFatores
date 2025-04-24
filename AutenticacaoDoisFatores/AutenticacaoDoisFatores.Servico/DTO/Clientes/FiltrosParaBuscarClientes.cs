using AutenticacaoDoisFatores.Dominio.Filtros;

namespace AutenticacaoDoisFatores.Servico.DTO.Clientes
{
    public class FiltrosParaBuscarClientes : BuscaDeEntidadeComAuditoria
    {
        public string? Nome { get; set; }
        public string? Email { get; set; }
        public string? NomeDominio { get; set; }
        public bool? Ativo { get; set; }

        public static explicit operator FiltroDeClientes(FiltrosParaBuscarClientes filtrosParaBuscarClientes)
        {
            return new FiltroDeClientes
            (
                nome: filtrosParaBuscarClientes.Nome,
                email: filtrosParaBuscarClientes.Email,
                nomeDominio: filtrosParaBuscarClientes.NomeDominio,
                ativo: filtrosParaBuscarClientes.Ativo,
                dataCadastroDe: filtrosParaBuscarClientes.DataCadastroDe,
                dataCadastroAte: filtrosParaBuscarClientes.DataCadastroAte,
                dataAlteracaoDe: filtrosParaBuscarClientes.DataAlteracaoDe,
                dataAlteracaoAte: filtrosParaBuscarClientes.DataAlteracaoAte,
                pagina: filtrosParaBuscarClientes.Pagina!.Value,
                qtdPorPagina: filtrosParaBuscarClientes.QtdPorPagina!.Value
            );
        }
    }
}
