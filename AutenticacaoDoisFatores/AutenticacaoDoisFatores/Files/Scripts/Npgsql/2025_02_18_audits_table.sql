CREATE TABLE "Audits" (
	"Id" uuid NOT NULL,
	"Action" varchar(256) NOT NULL,
	"Details" jsonb NULL,
	CONSTRAINT "PK_Audits" PRIMARY KEY ("Id")
);