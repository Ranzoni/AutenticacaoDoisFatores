using AutenticacaoDoisFatores.Dominio.Compartilhados;
using AutenticacaoDoisFatores.Dominio.Compartilhados.Mensagens;
using AutenticacaoDoisFatores.Dominio.Dominios;
using AutenticacaoDoisFatores.Dominio.Entidades;
using AutenticacaoDoisFatores.Dominio.Validadores;
using AutenticacaoDoisFatores.Servico.Compartilhados;
using AutenticacaoDoisFatores.Servico.DTO.Clientes;
using AutenticacaoDoisFatores.Servico.Excecoes;
using Mensageiro;

namespace AutenticacaoDoisFatores.Servico.CasosDeUso.Clientes
{
    public class CriarCliente(DominioDeClientes dominio, DominioDeUsuarios usuarios, INotificador notificador, EnvioDeEmail email)
    {
        private readonly DominioDeClientes _dominio = dominio;
        private readonly DominioDeUsuarios _usuarios = usuarios;
        private readonly INotificador _notificador = notificador;
        private readonly EnvioDeEmail _email = email;

        public async Task<ClienteCadastrado?> CriarAsync(NovoCliente novoCliente, string linkBaseConfirmacaoCadastro)
        {
            var cadastroEhValido = await NovoClienteEhValidoAsync(novoCliente, linkBaseConfirmacaoCadastro);
            if (!cadastroEhValido)
                return null;

            var cliente = await CadastrarClienteAsync(novoCliente);
            await CadastrarUsuarioAdminAsync(novoCliente);

            EnviarEmail(cliente.Id, novoCliente, linkBaseConfirmacaoCadastro);

            return (ClienteCadastrado)cliente;
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

            if (!Seguranca.ComposicaoSenhaEhValida(novoCliente.SenhaAdm))
                _notificador.AddMensagem(MensagensValidacaoUsuario.SenhaInvalida);

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

        private async Task<Cliente> CadastrarClienteAsync(NovoCliente novoCliente)
        {
            var cliente = (Cliente)novoCliente;

            await _dominio.CriarClienteAsync(cliente);
            await _dominio.CriarDominioDoClienteAsync(cliente.Id);

            return cliente;
        }

        private async Task CadastrarUsuarioAdminAsync(NovoCliente novoCliente)
        {
            var usuarioAdm = (Usuario)novoCliente;
            await _usuarios.CriarUsuarioComDominioAsync(usuarioAdm, novoCliente.NomeDominio);
        }

        private void EnviarEmail(Guid id, NovoCliente novoCliente, string linkBaseConfirmacaoCadastro)
        {
            var tokenConfirmacaoDeCadastro = Seguranca.GerarTokenDeConfirmacaoDeCliente(id);
            _email.EnviarConfirmacaoDeCadastroDeCliente(novoCliente.Email, novoCliente.ChaveDescriptografada(), linkBaseConfirmacaoCadastro, tokenConfirmacaoDeCadastro);
        }
    }
}
