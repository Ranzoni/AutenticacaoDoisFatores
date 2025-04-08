using AutenticacaoDoisFatores.Dominio.Filtros;

namespace AutenticacaoDoisFatores.Servico.DTO.Usuarios
{
    public class FiltrosParaBuscarUsuario : BuscaDeEntidadeComAuditoria
    {
        public string? Nome { get; set; }
        public string? NomeUsuario { get; set; }
        public string? Email { get; set; }
        public bool? Ativo { get; set; }
        public DateTime? DataUltimoAcessoDe { get; set; }
        public DateTime? DataUltimoAcessoAte { get; set; }

        public static explicit operator FiltroDeUsuarios(FiltrosParaBuscarUsuario filtro)
        {
            return new FiltroDeUsuarios
            (
                nome: filtro.Nome,
                nomeUsuario: filtro.NomeUsuario,
                email: filtro.Email,
                ativo: filtro.Ativo,
                dataUltimoAcessoDe: filtro.DataUltimoAcessoDe,
                dataUltimoAcessoAte: filtro.DataUltimoAcessoAte,
                ehAdmin: false,
                dataCadastroDe: filtro.DataCadastroDe,
                dataCadastroAte: filtro.DataCadastroAte,
                dataAlteracaoDe: filtro.DataAlteracaoDe,
                dataAlteracaoAte: filtro.DataAlteracaoAte,
                pagina: filtro.Pagina!.Value,
                qtdPorPagina: filtro.QtdPorPagina!.Value
            );
        }
    }
}
