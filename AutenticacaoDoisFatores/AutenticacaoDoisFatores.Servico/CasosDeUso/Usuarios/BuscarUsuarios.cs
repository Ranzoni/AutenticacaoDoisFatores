using AutenticacaoDoisFatores.Dominio.Dominios;
using AutenticacaoDoisFatores.Dominio.Filtros;
using AutenticacaoDoisFatores.Servico.DTO.Usuarios;

namespace AutenticacaoDoisFatores.Servico.CasosDeUso.Usuarios
{
    public class BuscarUsuarios(DominioDeUsuarios dominio)
    {
        private readonly DominioDeUsuarios _dominio = dominio;

        public async Task<UsuarioCadastrado?> BuscarUnicoAsync(Guid id)
        {
            var usuario = await _dominio.BuscarUnicoAsync(id);
            return (UsuarioCadastrado?)usuario!;
        }

        public async Task<IEnumerable<UsuarioCadastrado>> BuscarVariosAsync(FiltrosParaBuscarUsuario filtrosParaBuscar)
        {
            var filtros = (FiltroDeUsuarios)filtrosParaBuscar;

            var usuarios = await _dominio.BuscarVariosAsync(filtros);

            var usuariosCadastrados = usuarios.Select(u => (UsuarioCadastrado)u);
            return usuariosCadastrados;
        }
    }
}
