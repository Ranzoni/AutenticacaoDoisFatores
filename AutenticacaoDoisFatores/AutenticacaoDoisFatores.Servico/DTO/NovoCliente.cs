﻿namespace AutenticacaoDoisFatores.Servico.DTO
{
    public class NovoCliente(string nome, string email, string nomeDominio)
    {
        public string Nome { get; } = nome;
        public string Email { get; } = email;
        public string NomeDominio { get; } = nomeDominio;
    }
}
