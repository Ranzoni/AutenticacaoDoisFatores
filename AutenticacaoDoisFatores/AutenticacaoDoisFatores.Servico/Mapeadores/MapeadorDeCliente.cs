using AutenticacaoDoisFatores.Dominio.Entidades;
using AutenticacaoDoisFatores.Servico.DTO;
using AutoMapper;

namespace AutenticacaoDoisFatores.Servico.Mapeadores
{
    internal class MapeadorDeCliente : Profile
    {
        public MapeadorDeCliente()
        {
            CreateMap<NovoCliente, Cliente>();
            CreateMap<Cliente, ClienteCadastrado>();
        }
    }
}
