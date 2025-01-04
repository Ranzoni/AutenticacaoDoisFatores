﻿using AutenticacaoDoisFatores.Dominio.Compartilhados;
using AutenticacaoDoisFatores.Dominio.Compartilhados.Mensagens;

namespace AutenticacaoDoisFatores.Dominio.Excecoes
{
    public class ExcecoesCliente(MensagensCliente mensagem) : ApplicationException(mensagem.Descricao())
    {
        internal static void NomeInvalido()
        {
            throw new ExcecoesCliente(MensagensCliente.NomeInvalido);
        }

        internal static void EmailInvalido()
        {
            throw new ExcecoesCliente(MensagensCliente.EmailInvalido);
        }

        internal static void NomeDominioInvalido()
        {
            throw new ExcecoesCliente(MensagensCliente.NomeDominioInvalido);
        }

        internal static void EmailJaCadastrado()
        {
            throw new ExcecoesCliente(MensagensCliente.EmailJaCadastrado);
        }

        internal static void NomeDominioJaCadastrado()
        {
            throw new ExcecoesCliente(MensagensCliente.NomeDominioJaCadastrado);
        }
    }
}
