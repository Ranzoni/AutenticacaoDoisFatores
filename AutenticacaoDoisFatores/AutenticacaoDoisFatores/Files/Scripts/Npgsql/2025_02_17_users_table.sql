CREATE TABLE "Users" (
	"Id" uuid NOT NULL,
	"Name" varchar(50) NOT NULL,
	"Username" varchar(20) NOT NULL,
	"Email" varchar(256) NOT NULL,
	"Password" varchar(50) NOT NULL,
	"Active" bool DEFAULT false NOT NULL,
	"LastAccess" timestamp NULL,
	"CreatedAt" timestamp NOT NULL,
	"UpdatedAt" timestamp NULL,
	CONSTRAINT "PK_Users" PRIMARY KEY ("Id")
);
CREATE UNIQUE INDEX "IX_Users_Username" ON "Users" USING btree ("Username");
CREATE UNIQUE INDEX "IX_Users_Email" ON "Users" USING btree ("Email");