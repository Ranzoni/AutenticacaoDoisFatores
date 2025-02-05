using AutenticacaoDoisFatores.Dominio.Entidades;
using AutenticacaoDoisFatores.Servico.DTO.Clientes;
using AutoMapper;

namespace AutenticacaoDoisFatores.Servico.Mapeadores
{
    public class MapeadorDeCliente : Profile
    {
        public MapeadorDeCliente()
        {
            CreateMap<NovoCliente, Cliente>();
            CreateMap<Cliente, ClienteCadastrado>();
        }
    }
}
