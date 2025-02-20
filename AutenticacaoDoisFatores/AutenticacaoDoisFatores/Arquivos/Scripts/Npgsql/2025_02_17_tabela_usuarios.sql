CREATE TABLE "Usuarios" (
	"Id" uuid NOT NULL,
	"Nome" varchar(50) NOT NULL,
	"NomeUsuario" varchar(20) NOT NULL,
	"Email" varchar(256) NOT NULL,
	"Senha" varchar(50) NOT NULL,
	"Ativo" bool DEFAULT false NOT NULL,
	"DataUltimoAcesso" timestamp NULL,
	"DataCadastro" timestamp NOT NULL,
	"DataAlteracao" timestamp NULL,
	CONSTRAINT "PK_Usuarios" PRIMARY KEY ("Id")
);
CREATE UNIQUE INDEX "IX_Usuarios_NomeUsuario" ON "Usuarios" USING btree ("NomeUsuario");
CREATE UNIQUE INDEX "IX_Usuarios_Email" ON "Usuarios" USING btree ("Email");