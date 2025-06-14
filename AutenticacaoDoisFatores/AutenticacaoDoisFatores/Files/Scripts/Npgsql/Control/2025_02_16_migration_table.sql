CREATE TABLE IF NOT EXISTS "__AdfMigrations" (
	"FileName" varchar(150) NOT NULL,
	"ExecutedAt" timestamp NOT NULL,
	CONSTRAINT "PK__AdfMigrations" PRIMARY KEY ("FileName")
);