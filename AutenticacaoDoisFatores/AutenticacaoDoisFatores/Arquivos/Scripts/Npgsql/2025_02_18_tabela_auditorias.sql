CREATE TABLE "Auditorias" (
	"Id" uuid NOT NULL,
	"Acao" varchar(256) NOT NULL,
	"Detalhes" jsonb NULL,
	CONSTRAINT "PK_Auditorias" PRIMARY KEY ("Id")
);