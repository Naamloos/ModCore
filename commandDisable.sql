CREATE TABLE "mcore_cmd_state" (
    "command_qualified" TEXT NOT NULL CONSTRAINT "command_qualified" PRIMARY KEY,
    "id" smallint NOT NULL,
    CONSTRAINT "id" UNIQUE ("id")
);

CREATE UNIQUE INDEX "index_id" ON "mcore_cmd_state" ("id");

INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
VALUES ('20180728075100_CommandDisableReal', '2.0.2-rtm-10011');