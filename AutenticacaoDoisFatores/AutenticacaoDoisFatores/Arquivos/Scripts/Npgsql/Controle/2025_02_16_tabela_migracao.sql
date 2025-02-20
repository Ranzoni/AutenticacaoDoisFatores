CREATE TABLE IF NOT EXISTS "__MigracoesAdf" (
	"NomeArquivo" varchar(150) NOT NULL,
	"DataExecucao" timestamp NOT NULL,
	CONSTRAINT "PK__MigracoesAdf" PRIMARY KEY ("NomeArquivo")
);