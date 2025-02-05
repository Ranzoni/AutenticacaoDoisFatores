using AutenticacaoDoisFatores.Dominio.Entidades;
using AutenticacaoDoisFatores.Servico.DTO.Usuarios;
using AutoMapper;

namespace AutenticacaoDoisFatores.Servico.Mapeadores
{
    public class MapeadorDeUsuario : Profile
    {
        public MapeadorDeUsuario()
        {
            CreateMap<NovoUsuario, Usuario>();
            CreateMap<Usuario, UsuarioCadastrado>();
        }
    }
}
