ALTER TABLE "mcore_cmd_state" DROP CONSTRAINT "command_qualified";

DROP INDEX "index_id";

CREATE SEQUENCE "mcore_cmd_state_id_seq" START WITH 1 INCREMENT BY 1 NO MINVALUE NO MAXVALUE NO CYCLE;
ALTER TABLE "mcore_cmd_state" ALTER COLUMN "id" TYPE int2;
ALTER TABLE "mcore_cmd_state" ALTER COLUMN "id" SET NOT NULL;
ALTER TABLE "mcore_cmd_state" ALTER COLUMN "id" SET DEFAULT (nextval('"mcore_cmd_state_id_seq"'));
ALTER SEQUENCE "mcore_cmd_state_id_seq" OWNED BY "mcore_cmd_state"."id"
ALTER TABLE "mcore_cmd_state" ADD CONSTRAINT "PK_mcore_cmd_state" PRIMARY KEY ("command_qualified");

INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
VALUES ('20180801000135_DataAnnotations', '2.0.2-rtm-10011');