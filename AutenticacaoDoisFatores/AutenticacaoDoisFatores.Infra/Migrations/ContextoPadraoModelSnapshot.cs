﻿// <auto-generated />
using System;
using AutenticacaoDoisFatores.Infra.Contexto;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace AutenticacaoDoisFatores.Infra.Migrations
{
    [DbContext(typeof(ContextoPadrao))]
    partial class ContextoPadraoModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "9.0.0")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("AutenticacaoDoisFatores.Dominio.Entidades.Cliente", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<bool>("Ativo")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("boolean")
                        .HasDefaultValue(false);

                    b.Property<string>("ChaveAcesso")
                        .IsRequired()
                        .HasMaxLength(2000)
                        .HasColumnType("character varying(2000)");

                    b.Property<DateTime?>("DataAlteracao")
                        .HasColumnType("timestamp without time zone");

                    b.Property<DateTime>("DataCadastro")
                        .HasColumnType("timestamp without time zone");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasMaxLength(256)
                        .HasColumnType("character varying(256)");

                    b.Property<string>("Nome")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("character varying(50)");

                    b.Property<string>("NomeDominio")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("character varying(100)");

                    b.HasKey("Id");

                    b.HasIndex("Email")
                        .IsUnique();

                    b.HasIndex("NomeDominio")
                        .IsUnique();

                    b.ToTable("Clientes");
                });

            modelBuilder.Entity("AutenticacaoDoisFatores.Infra.Compartilhados.Auditoria", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<string>("Acao")
                        .IsRequired()
                        .HasColumnType("varchar(256)");

                    b.Property<DateTime>("Data")
                        .HasColumnType("timestamp");

                    b.Property<object>("Detalhes")
                        .HasColumnType("jsonb");

                    b.Property<Guid>("IdEntidade")
                        .HasColumnType("uuid");

                    b.Property<string>("Tabela")
                        .IsRequired()
                        .HasColumnType("varchar(100)");

                    b.HasKey("Id");

                    b.ToTable("Auditorias");
                });
#pragma warning restore 612, 618
        }
    }
}
