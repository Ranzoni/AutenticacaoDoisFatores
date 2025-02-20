﻿using AutenticacaoDoisFatores.Dominio.Compartilhados.Entidades;
using AutenticacaoDoisFatores.Dominio.Validadores;

namespace AutenticacaoDoisFatores.Dominio.Entidades
{
    public class Cliente : EntidadeComAuditoria
    {
        public string Nome { get; private set; } = "";
        public string Email { get; private set; } = "";
        public string NomeDominio { get; private set; } = "";
        public string ChaveAcesso { get; private set; } = "";
        public bool Ativo { get; private set; } = false;

        private Cliente() : base(true) { }

        public Cliente(string nome, string email, string nomeDominio, string chaveAcesso)
        {
            Nome = nome;
            Email = email;
            NomeDominio = nomeDominio;
            ChaveAcesso = chaveAcesso;

            this.Validar();
        }

        public Cliente(Guid id, string nome, string email, string nomeDominio, string chaveAcesso, bool ativo, DateTime dataCadastro, DateTime? dataAlteracao)
            : base(true)
        {
            Id = id;
            Nome = nome;
            Email = email;
            NomeDominio = nomeDominio;
            ChaveAcesso = chaveAcesso;
            Ativo = ativo;
            DataCadastro = dataCadastro;
            DataAlteracao = dataAlteracao;

            this.Validar();
        }

        public void AlterarChaveAcesso(string chaveAcesso)
        {
            ChaveAcesso = chaveAcesso;

            AtualizarDataAlteracao();
        }

        public void Ativar(bool valor)
        {
            Ativo = valor;

            AtualizarDataAlteracao();
        }
    }
}
