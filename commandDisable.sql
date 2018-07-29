CREATE TABLE IF NOT EXISTS "__EFMigrationsHistory" (
    "MigrationId" varchar(150) NOT NULL,
    "ProductVersion" varchar(32) NOT NULL,
    CONSTRAINT "PK___EFMigrationsHistory" PRIMARY KEY ("MigrationId")
);

CREATE TABLE "mcore_cmd_state" (
    "command_qualified" text NOT NULL,
    "id" smallserial NOT NULL,
    CONSTRAINT "command_qualified" PRIMARY KEY ("command_qualified"),
    CONSTRAINT "id" UNIQUE ("id")
);

CREATE UNIQUE INDEX "index_id" ON "mcore_cmd_state" ("id");

INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
VALUES ('20180729081030_Initial', '2.0.2-rtm-10011');
