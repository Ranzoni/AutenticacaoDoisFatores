using AutenticacaoDoisFatores.Dominio.Compartilhados;
using AutenticacaoDoisFatores.Dominio.Compartilhados.Mensagens;
using AutenticacaoDoisFatores.Dominio.Dominios;
using AutenticacaoDoisFatores.Dominio.Entidades;
using AutenticacaoDoisFatores.Dominio.Validadores;
using AutenticacaoDoisFatores.Servico.Compartilhados;
using AutenticacaoDoisFatores.Servico.DTO;
using AutenticacaoDoisFatores.Servico.Excecoes;
using AutoMapper;
using Mensageiro;

namespace AutenticacaoDoisFatores.Servico.CasosDeUso.Clientes
{
    public class CriarCliente(IMapper mapper, DominioDeClientes dominio, INotificador notificador, Email email)
    {
        private readonly IMapper _mapper = mapper;
        private readonly DominioDeClientes _dominio = dominio;
        private readonly INotificador _notificador = notificador;
        private readonly Email _email = email;

        public async Task<ClienteCadastrado?> ExecutarAsync(NovoCliente novoCliente, string linkBaseConfirmacaoCadastro)
        {
            var cadastroEhValido = await NovoClienteEhValidoAsync(novoCliente, linkBaseConfirmacaoCadastro);
            if (!cadastroEhValido)
                return null;

            var cliente = await CriarClienteAsync(novoCliente);

            EnviarEmail(novoCliente, linkBaseConfirmacaoCadastro);

            var clienteCadastrado = _mapper.Map<ClienteCadastrado>(cliente);
            return clienteCadastrado;
        }

        private async Task<bool> NovoClienteEhValidoAsync(NovoCliente novoCliente, string linkConfirmacaoCadastro)
        {
            if (!ValidadorDeCliente.NomeEhValido(novoCliente.Nome))
                _notificador.AddMensagem(MensagensValidacaoCliente.NomeInvalido);

            if (!ValidadorDeCliente.EmailEhValido(novoCliente.Email))
                _notificador.AddMensagem(MensagensValidacaoCliente.EmailInvalido);

            if (!ValidadorDeCliente.NomeDominioEhValido(novoCliente.NomeDominio))
                _notificador.AddMensagem(MensagensValidacaoCliente.NomeDominioInvalido);

            if (!ValidadorDeCliente.ChaveAcessoEhValida(novoCliente.ChaveAcesso))
                _notificador.AddMensagem(MensagensValidacaoCliente.ChaveAcessoInvalida);

            if (linkConfirmacaoCadastro.EstaVazio())
                ExcecoesCriacaoCliente.LinkConfirmacaoCadastroNaoInformado();

            var emailJaCadastrado = await _dominio.EmailEstaCadastradoAsync(novoCliente.Email);
            if (emailJaCadastrado)
                _notificador.AddMensagem(MensagensValidacaoCliente.EmailJaCadastrado);

            var nomeDominioJaCadastrado = await _dominio.NomeDominioEstaCadastradoAsync(novoCliente.NomeDominio);
            if (nomeDominioJaCadastrado)
                _notificador.AddMensagem(MensagensValidacaoCliente.NomeDominioJaCadastrado);

            var clienteEhValido = !_notificador.ExisteMensagem();
            return clienteEhValido;
        }

        private async Task<Cliente> CriarClienteAsync(NovoCliente novoCliente)
        {
            var cliente = _mapper.Map<Cliente>(novoCliente);

            await _dominio.CriarClienteAsync(cliente);
            await _dominio.CriarDominioDoClienteAsync(cliente.Id);

            return cliente;
        }

        private void EnviarEmail(NovoCliente novoCliente, string linkBaseConfirmacaoCadastro)
        {
            var tokenConfirmacaoDeCadastro = Seguranca.GerarTokenDeConfirmacaoDeCliente(novoCliente.Email);
            var linkConfirmacaoCadastro = $"{linkBaseConfirmacaoCadastro}/{tokenConfirmacaoDeCadastro}";
            _email.EnviarConfirmacaoDeCadastroDeCliente(novoCliente.Email, novoCliente.ChaveDescriptografada(), linkConfirmacaoCadastro);
        }
    }
}
